using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gpio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("GPIO tool for VV-Soft Generic USB HID IO v1.00");
            Console.WriteLine();
            if (args.Length > 0)
            {
                int? port = null;
                int portId;
                if (args.Length>1 && int.TryParse(args[1], out portId)) { port = portId; }
                switch (args[0].ToLowerInvariant())
                {
                    case "get":
                        GetInput(port);
                        break;
                    case "set":
                        if (args.Length < (port.HasValue ? 3 : 2))
                        {
                            Console.WriteLine("Invalid arguments");
                            Console.WriteLine();
                            ShowUsage();
                            break;
                        }
                        SetOutput(port, args[port.HasValue ? 2 : 1]);
                        break;
                    default:
                        Console.WriteLine("Invalid arguments");
                        Console.WriteLine();
                        ShowUsage();
                        break;
                }
            }
            else
            {
                ShowUsage();
            }
        }

        private static void SetOutput(int? port, string values)
        {
            var dev = new IODevice();
            if (dev.Device != null)
            {
                if (port.HasValue && (port < 1 || port > dev.Outputs))
                {
                    Console.WriteLine("No output {0}", port);
                    return;
                }
                int first = port ?? 1, last = port ?? dev.Inputs;
                var newValues = new OutputValue[dev.Outputs+3];     //max 3 for padding

                for (int i = 0; i < values.Length && i + first <= last; i++)
                {
                    OutputValue newValue;
                    if (!Enum.TryParse<OutputValue>(values.Substring(i, 1), true, out newValue)) { newValue = OutputValue.N; }
                    int pos = first + i - 1;
                    if (dev.OutputTypes[pos] == OutputType.HL && newValue == OutputValue.Z) { newValue = OutputValue.N; }
                    if (dev.OutputTypes[pos] == OutputType.HZ && newValue == OutputValue.L) { newValue = OutputValue.Z; }
                    if (dev.OutputTypes[pos] == OutputType.LZ && newValue == OutputValue.H) { newValue = OutputValue.Z; }
                    newValues[pos] = newValue;
                }
                var req = new byte[2 + (dev.Outputs + 3) / 4];
                req[0] = 0;
                req[1] = (byte)DeviceCommand.SetOutputs;
                int op = 0;
                for (int ix = 2; ix<req.Length; ix++)
                {
                    for (int j=0;j<4;j++)
                    {
                        req[ix] = (byte)(req[ix] << 2 | (byte)newValues[op++]);
                    }
                }
                dev.Stream.Write(req);
                var response = dev.Stream.Read();
                if (response.Length < 2 || response[1] != (byte)DeviceCommand.SetOutputs)
                {
                    Console.WriteLine("Invalid response");
                    return;
                }
                Console.Write("Set outputs to: ");
                for (int i=0; i<dev.Outputs;i++) { Console.Write(newValues[i]); }
                Console.WriteLine();
            }
        }

        private static void GetInput(int? port)
        {
            var dev = new IODevice();
            if (dev.Device != null)
            {
                if (port.HasValue && (port<1 || port>dev.Inputs))
                {
                    Console.WriteLine("No input {0}", port);
                    return;
                }
                int first = port ?? 1, last = port ?? dev.Inputs;

                dev.Stream.Write(new byte[] { 0, (byte)DeviceCommand.GetInputs });
                var response = dev.Stream.Read();
                if (response.Length < 2 + (dev.Inputs + 7) / 8)
                {
                    Console.WriteLine("Got invalid response");
                }

                Console.Write(first == last ? "Input {0}: " : "Inputs {0}-{1}: ", first, last);
                for (int i = first - 1; i < last; i++)
                {
                    int ix = 2 + i / 8;
                    Console.Write((response[ix] & (128 >> (i & 7))) == 0 ? "L" : "H");
                }
                Console.WriteLine();
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("gpio                 Show usage");
            Console.WriteLine("gpio get             Get values of inputs");
            Console.WriteLine("gpio get <n>         Get value of input n");
            Console.WriteLine("gpio set <values>    Set values of outputs starting from 1");
            Console.WriteLine("gpio set <n> <value> Set value of output n");
            Console.WriteLine();
            Console.WriteLine("gpio is not recommended for multiple generic IO boards connected simulaneously, as it will use the first device it sees");
            Console.WriteLine();
            Console.WriteLine("Possible values are:");
            Console.WriteLine("H = High");
            Console.WriteLine("L = Low");
            Console.WriteLine("Z = Floating / High impedance");
            Console.WriteLine("N = No change");
            Console.WriteLine();
            Console.WriteLine("(Z and N are used only with outputs)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("gpio get 2");
            Console.WriteLine("gpio set 2 L");
            Console.WriteLine("gpio set HLNLZN");
        }
    }
    internal class IODevice
    {
        public int Inputs { get; internal set; }
        public int Outputs { get; internal set; }
        public OutputType[] OutputTypes { get; internal set; }
        public HidStream Stream { get; internal set; }
        public HidDevice Device { get; private set; }

        public IODevice()
        {
            var loader = new HidDeviceLoader();
            Device = loader.GetDevices(0x1209, 0x2345).FirstOrDefault();
            if (Device == null)
            {
                Console.WriteLine("No device with VID=0x1209, PID=0x2345");
                return;
            }

            Console.WriteLine("Manufacturer: {0}", Device.Manufacturer);
            Console.WriteLine("Product: {0}", Device.ProductName);
            Stream = Device.Open();

            /*var getCap = new byte[Device.MaxInputReportLength];
            getCap[1] = (byte)DeviceCommand.GetCapabilities;

            Stream.Write(getCap);*/
            
            Stream.Write(new byte[] { 0, (byte)DeviceCommand.GetCapabilities });
            var response = Stream.Read();
            if (response.Length<3 || response[1] != (byte)DeviceCommand.GetCapabilities)
            {
                Console.WriteLine("Incompatible device with VID=0x1209, PID=0x2345");
                Device = null;
                return;
            }
            Inputs = response[2];
            Console.WriteLine("Inputs: {0}", Inputs);
            Outputs = 0; //Default
            if (response.Length > 3)
            {
                Outputs = response[3];
                Console.WriteLine("Outputs: {0}", Outputs);
            }
            OutputTypes = new OutputType[Outputs];
            var AllOutputs = (OutputType)(response.Length > 4 ? response[4] : 0);
            if ((byte)AllOutputs < 4)
            {
                for (int i=0;i<Outputs; i++) { OutputTypes[i] = AllOutputs; }
                Console.WriteLine("All outputs are type: {0}", AllOutputs);
            } else
            {
                Console.Write("Output types are:");
                for (int i = 0; i < Outputs; i++) {
                    var ix = 5 + (i / 4);
                    var outputType = OutputType.HL; //Default
                    if (response.Length> ix)
                    {
                        outputType = (OutputType)((response[ix] >> ((3-(i & 3)) >> 1)) & 3);
                    }
                    OutputTypes[i] = outputType;
                    Console.Write(" {0}={1}", i + 1, outputType);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
    internal enum DeviceCommand : byte
    {
        GetCapabilities = 0x01,
        GetInputs = 0x02,
        SetOutputs = 0x03
    }
    internal enum OutputType : byte
    {
        HL = 0,
        HLZ = 1,
        LZ = 2,
        HZ = 3
    }
    internal enum OutputValue : byte
    {
        N = 0,  //No change
        Z = 1,  //High impedance
        L = 2,  //Low
        H = 3,  //High
    }

}

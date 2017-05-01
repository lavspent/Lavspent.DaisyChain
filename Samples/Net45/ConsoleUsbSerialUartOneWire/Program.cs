using Lavspent.Backport;
using Lavspent.DaisyChain.Devices.MaximIntegrated.Ds18B20;
using Lavspent.DaisyChain.OneWire;
using Lavspent.TaskEnumerableExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;
//using System.IO.Ports;
using System.Threading.Tasks;


namespace ConsoleUsbSerialUartOneWire
{



    public class SerialPortOneWireBus : IOneWireBus, IDisposable
    {
        private string _portName;
        private SerialPort _serialPort;
        private Stream _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        public SerialPortOneWireBus(string portName)
        {
            _portName = portName;

            ResetSerialDevice(9600);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        private void ResetSerialDevice(int baudRate)
        {
            if (_serialPort == null)
            {
                // first time, create and open serial port
                _serialPort = new SerialPort(
                    _portName,
                    baudRate,
                    System.IO.Ports.Parity.None,
                    8,
                    System.IO.Ports.StopBits.One
                    );
                _serialPort.Handshake = System.IO.Ports.Handshake.None;
                _serialPort.WriteTimeout = 1000;
                _serialPort.ReadTimeout = 1000;

                _serialPort.Open();

                _stream = _serialPort.BaseStream;
            }
            else
            {
                // just ajust baudrate if already up and running
                _serialPort.BaudRate = baudRate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task ResetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // reset serial device
            ResetSerialDevice(9600);

            // send reset pulse
            _stream.WriteByte(0xF0); /* todo: magic number */
            _stream.Flush();

            // get response
            int responseInt = _stream.ReadByte();

            if (responseInt == -1)
            {
                throw new EndOfStreamException("Serial device not present.");
            }

            byte response = (byte)responseInt;

            if (response == 0xF0)
            {
                // We read our own reset message, assume no 1-wire device is present.
                throw new Exception("1-Wire device not found.");
            }
            else
            {
                // 1-wire device present, switch to data speed
                ResetSerialDevice(115200);
            }

            return TaskEx.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _serialPort?.Dispose();
            _serialPort = null;
        }

        public Task<IEnumerable<OneWireAddress>> GetDeviceAddressesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // todo: imeplment cancellation

            List<OneWireAddress> addresses = new List<OneWireAddress>();
            OneWireAddress address;
            byte node = 0;
            bool lastDevice;
            while (SearchNext(ref node, out address, out lastDevice))
            {
                addresses.Add(address);

                if (lastDevice)
                    break;
            }

            return Task.FromResult<IEnumerable<OneWireAddress>>(addresses);
        }

        public Task MatchRomAsync(byte[] rom, CancellationToken cancellationToken)
        {
            Write(OneWireCommands.MatchRom);

            for (int i = 0; i < 8; i++)
            {
                Write(rom[i]);
            }

            return TaskEx.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<byte> ReadAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1];
            await ReadAsync(buffer, 1, cancellationToken);
            return buffer[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Task ReadAsync(byte[] buffer, int length, CancellationToken cancellationToken)
        {
            for (int i = 0; i < length; i++)
            {
                byte byteValue = 0;
                for (int j = 0; j < 8; j++)
                {
                    byte bitValue = ReadBit();
                    byteValue |= (byte)(bitValue << j);
                }

                buffer[i] = byteValue;
            }
            return TaskEx.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            Write(data);
            return TaskEx.CompletedTask;
        }

        public void Write(params byte[] data)
        {
            // loop through bytes
            for (int i = 0; i < data.Length; i++)
            {
                byte byteValue = data[i];

                // loop through bits
                for (int j = 0; j < 8; j++)
                {
                    byte bitValue = (byte)((byteValue >> j) & 0x01);
                    WriteBit(bitValue);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void WriteBit(byte data)
        {
            WriteReadBit(data);
        }

        public Task WriteBitAsync(byte data, CancellationToken cancellationToken)
        {
            WriteBit(data);
            return Task.Run(() => { });
        }

        private byte WriteReadBit(byte bit)
        {
            bit = bit == 0 ? (byte)0x00 : (byte)0xff;
            _stream.WriteByte(bit);
            _stream.Flush();

            int responseInt = _stream.ReadByte();
            if (responseInt == -1)
                throw new Exception("End of stream encountered.");

            byte response = (byte)responseInt;

            return response == 0xFF ? (byte)1 : (byte)0;
        }

        private byte ReadBit()
        {
            return WriteReadBit(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<byte> ReadBitAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ReadBit());
        }

        public Task SkipRomAsync(CancellationToken cancellationToken)
        {
            Write(OneWireCommands.SkipRom);
            return TaskEx.CompletedTask;
        }


        private bool SearchNext(ref byte lastDiscrepancy, out OneWireAddress address, out bool lastDevice)
        {
            address = null;
            byte[] tmpAddress = new byte[8];
            byte lastZero = 0;
            ResetAsync(); // this runs synchronously

            Write(OneWireCommands.SearchRom);

            for (byte i = 0; i < 64; i++)
            {
                byte byteIndex = (byte)(i / 8);
                byte bitNumber = (byte)(i + 1); ;
                byte bitMask = (byte)(1 << (i % 8));

                var bit = ReadBit();
                var bitComp = ReadBit();

                if (bit == 1 && bitComp == 1)
                {
                    // no devices found
                    lastDevice = true;
                    return false;
                }

                byte searchDirection = 0;
                if (bit != bitComp)
                {
                    searchDirection = bit;
                }
                else
                {
                    if (bitNumber < lastDiscrepancy)
                    {
                        searchDirection = ((tmpAddress[byteIndex] & bitMask) > 0) ? (byte)1 : (byte)0;
                    }
                    else
                    {
                        searchDirection = (bitNumber == lastDiscrepancy) ? (byte)1 : (byte)0;
                    }

                    if (searchDirection == 0)
                    {
                        lastZero = bitNumber;
                    }
                }

                if (searchDirection == 1)
                    tmpAddress[byteIndex] |= bitMask;
                else
                    tmpAddress[byteIndex] &= (byte)~bitMask;


                WriteBit(searchDirection);
            }

            lastDiscrepancy = lastZero;
            lastDevice = lastZero == 0;

            address = tmpAddress;
            return true;

        }
    }


    internal class ProcessConnection
    {


        public static ConnectionOptions ProcessConnectionOptions()

        {

            ConnectionOptions options = new ConnectionOptions();

            options.Impersonation = ImpersonationLevel.Impersonate;

            options.Authentication = AuthenticationLevel.Default;

            options.EnablePrivileges = true;

            return options;

        }



        public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)

        {

            ManagementScope connectScope = new ManagementScope();

            connectScope.Path = new ManagementPath(@"\\" + machineName + path);

            connectScope.Options = options;

            connectScope.Connect();

            return connectScope;

        }

    }



    public class COMPortInfo

    {

        public string Name { get; set; }

        public string Description { get; set; }



        public COMPortInfo() { }



        public static List<COMPortInfo> GetCOMPortsInfo()

        {

            List<COMPortInfo> comPortInfoList = new List<COMPortInfo>();



            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();

            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");



            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");

            ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);



            using (comPortSearcher)

            {

                string caption = null;

                foreach (ManagementObject obj in comPortSearcher.Get())

                {

                    if (obj != null)

                    {

                        object captionObj = obj["Caption"];

                        if (captionObj != null)

                        {

                            caption = captionObj.ToString();

                            if (caption.Contains("(COM"))

                            {

                                COMPortInfo comPortInfo = new COMPortInfo();

                                comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")",

                                                                     string.Empty);

                                comPortInfo.Description = caption;

                                comPortInfoList.Add(comPortInfo);

                            }

                        }

                    }

                }

            }

            return comPortInfoList;

        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            /*
             * https://msdn.microsoft.com/en-us/library/windows/hardware/ff553426(v=vs.85).aspx
             */
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""
            );
            foreach (ManagementObject queryObj in searcher.Get())
            {
                string caption = queryObj["Caption"] as String;
                string name = queryObj["Name"] as String;
            }


            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
            {
                Console.WriteLine(string.Format("{0} – {1}", comPort.Name, comPort.Description));
            }
            /*

                        ManagementObjectCollection collection;
                        using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                            collection = searcher.Get();

                        foreach (var device in collection)
                        {
                            var s1 = (string)device.GetPropertyValue("DeviceID");
                            var s2 = (string)device.GetPropertyValue("PNPDeviceID");
                            var s3 = (string)device.GetPropertyValue("Description");
                            int i = 5;
                        }

                        collection.Dispose();
                        */


            IOneWireBus oneWireBus = new SerialPortOneWireBus("COM7");

            // get addresses and devices
            var addresses = (await oneWireBus.GetDeviceAddressesAsync());
            var devices = addresses.Select((addr) => new Ds18B20(oneWireBus, addr));

            while (true)
            {
                // start conversion for all devices on bus
                await Ds18B20.ConvertTemperatureAsync(oneWireBus);

                // wait 750 ms
                await Task.Delay(750);

                Console.Clear();

                // loop devices and read temperature
                foreach (var device in devices)
                {
                    var temperature = await device.GetTemeperatureReadingAsync();
                    Console.WriteLine(temperature.Celcius);
                }
            }
        }
    }
}

using Lavspent.Backport;
using Lavspent.DaisyChain.Devices.MaximIntegrated.Ds18B20;
using Lavspent.DaisyChain.OneWire;
using Lavspent.DaisyChain.Serial;
using Lavspent.DaisyChain.Stream;
using Lavspent.TaskEnumerableExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
//using System.IO.Ports;
using System.Threading.Tasks;


namespace ConsoleUsbSerialUartOneWire
{
    /*private interface ISerialPortController
    {
        ISerialController
    }*/

    /*public class SerialPortController
    {

    }*/



    public class SerialPortOneWireBus : IOneWireBus, IDisposable
    {
        private ISerialController _serialController;
        private string _portName;
        private ISerial _serialPort;
        private IStream _stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        private SerialPortOneWireBus(ISerialController serialController, string portName)
        {
            _serialController = serialController ?? throw new ArgumentNullException(nameof(serialController));
            _portName = portName;
        }

        public async Task<SerialPortOneWireBus> OpenAsync(ISerialController serialController, string portName)
        {
            SerialPortOneWireBus serialPortOneWireBus = new SerialPortOneWireBus(serialController, portName);
            await serialPortOneWireBus.ResetSerialDeviceAsync(9600);

            return serialPortOneWireBus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baudRate"></param>
        /// <returns></returns>
        private async Task ResetSerialDeviceAsync(int baudRate)
        {
            if (_serialPort == null)
            {
                // first time, create and open serial port

                _serialPort = await _serialController.OpenSerialAsync(
                    _portName,
                    baudRate,
                    Lavspent.DaisyChain.Serial.Parity.None,
                    8,
                    Lavspent.DaisyChain.Serial.StopBits.One
                    );

                _serialPort.Handshake = Lavspent.DaisyChain.Serial.Handshake.None;

                _stream = _serialPort.GetStream();
                _stream.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                _stream.ReadTimeout = TimeSpan.FromMilliseconds(1000);
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
        public async Task ResetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // reset serial device
            await ResetSerialDeviceAsync(9600);

            // send reset pulse
            await _stream.WriteAsync(0xF0); /* todo: magic number */
            await _stream.FlushAsync();

            // get response
            int responseInt = await _stream.ReadAsync();

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
                await ResetSerialDeviceAsync(115200);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _serialPort?.Dispose();
            _serialPort = null;
        }

        public async Task<IEnumerable<OneWireAddress>> GetDeviceAddressesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // todo: imeplment cancellation

            List<OneWireAddress> addresses = new List<OneWireAddress>();
            OneWireAddress address;
            byte node = 0;
            bool lastDevice;
            while (await SearchNextAsync(ref node, out address, out lastDevice, cancellationToken))
            {
                addresses.Add(address);

                if (lastDevice)
                    break;
            }

            return addresses;
        }

        public async Task MatchRomAsync(byte[] rom, CancellationToken cancellationToken = default(CancellationToken))
        {
            await WriteAsync(new byte[] { OneWireCommands.MatchRom }, cancellationToken);

            for (int i = 0; i < 8; i++)
            {
                await WriteAsync(new byte[] { rom[i] }, cancellationToken);
            }
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
        public async Task ReadAsync(byte[] buffer, int length, CancellationToken cancellationToken)
        {
            for (int i = 0; i < length; i++)
            {
                byte byteValue = 0;
                for (int j = 0; j < 8; j++)
                {
                    byte bitValue = await ReadBitAsync();
                    byteValue |= (byte)(bitValue << j);
                }

                buffer[i] = byteValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public async Task WriteAsync(byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            // loop through bytes
            for (int i = 0; i < data.Length; i++)
            {
                byte byteValue = data[i];

                // loop through bits
                for (int j = 0; j < 8; j++)
                {
                    byte bitValue = (byte)((byteValue >> j) & 0x01);
                    await WriteBitAsync(bitValue, cancellationToken);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Task WriteBitAsync(byte data, CancellationToken cancellationToken)
        {
            return WriteReadBitAsync(data, cancellationToken);
        }

        private async Task<byte> WriteReadBitAsync(byte bit, CancellationToken cancellationToken)
        {
            bit = bit == 0 ? (byte)0x00 : (byte)0xff;
            await _stream.WriteAsync(bit, cancellationToken);
            await _stream.FlushAsync(cancellationToken);

            int responseInt = await _stream.ReadAsync(cancellationToken);
            if (responseInt == -1)
                throw new Exception("End of stream encountered.");

            byte response = (byte)responseInt;

            return response == 0xFF ? (byte)1 : (byte)0;
        }

        private Task<byte> ReadBitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return WriteReadBitAsync(1, cancellationToken);
        }


        public Task SkipRomAsync(CancellationToken cancellationToken)
        {
            return WriteAsync(new byte[] { OneWireCommands.SkipRom });
        }


        private async Task<bool> SearchNextAsync(ref byte lastDiscrepancy, out OneWireAddress address, out bool lastDevice, CancellationToken cancellationToken)
        {
            address = null;
            byte[] tmpAddress = new byte[8];
            byte lastZero = 0;
            await ResetAsync(cancellationToken); // this runs synchronously

            await WriteAsync(new byte[] { OneWireCommands.SearchRom }, cancellationToken);

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


                WriteBitAsync(searchDirection);
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

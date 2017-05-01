
using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Devices.AnalogDevices.Adxl345;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleFirmataI2cAccelerometer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }


        static async Task MainAsync(string[] args)
        {
            var serial = await SerialPortController.OpenSerialAsync("COM6", 57600, Parity.None, 8, StopBits.One);

            // Initialize a Firmata client connected to that stream
            // (we assume that there is a Firmata server in hte other end).
            FirmataClient firmataClient = await FirmataClient.OpenAsync(serial);

            // Grab the I2cController provided by the firmata server
            var i2cController = await firmataClient.GetI2cControllerAsync();

            // Set up connection to i2c device at address 0x53
            var i2cDevice = i2cController.GetDevice(new I2cConnectionSettings(0x53));

            // 
            var adxl345 = await Adxl345.Create(i2cDevice);

            byte deviceId = await adxl345.GetDeviceIdAsync();

            // Loop forever reading the compass
            IAccelerometerReading reading;
            while (true)
            {
                reading = await adxl345.GetAccelerometerReadingAsync();
                Console.Clear();
                Console.WriteLine($"{reading.X,7:N2} {reading.Y,7:N2} {reading.Z,7:N2}");
            }

            //
            // Unreachable code (due to never ending loop)
            //

            // Close the serial device
            //await serial.CloseAsync();

            // dispose the SerialPort
            //serialPort.Dispose();
            //serial.Dispose();
        }
    }
}



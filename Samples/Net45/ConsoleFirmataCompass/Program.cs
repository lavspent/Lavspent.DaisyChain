using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Devices.Honeywell.Hmc5883L;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.Serial;

using System;
using System.Threading.Tasks;


namespace ConsoleFirmataCompass
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }


        static async Task MainAsync(string[] args)
        {
            // Setup serial comminucation on COM6
            var serialPort = await SerialPortController.OpenSerialAsync("COM6", 57600, Parity.None, 8, StopBits.One);

            // Initialize a Firmata client connected to that stream
            // (we assume that there is a Firmata server in hte other end).
            FirmataClient firmataClient = await FirmataClient.OpenAsync(serialPort);

            // Grab the I2cController provided by the firmata server
            var i2cController = await firmataClient.GetI2cControllerAsync();

            // Set up connection to i2c device at address 0x1E
            var i2cCompassDevice = await i2cController.OpenDeviceAsync(new I2cConnectionSettings(0x1E));

            // Treat this i2c device as a Hmc5883L compass chip.
            var hmc5883L = new Hmc5883L(i2cCompassDevice);
            await hmc5883L.SetMeasuringModeAsync(Hmc5883MeasuringModes.Continuous);

            // Loop forever reading the compass
            var compassReading = new CompassReading();
            while (true)
            {
                var reading = await hmc5883L.GetCompassReadingAsync();
                Console.Clear();
                Console.WriteLine($"{reading.AxisX,7:N2} {reading.AxisY,7:N2} {reading.AxisZ,7:N2} {reading.HeadingMagneticNorth,7:N2}");
            }

            //
            // Unreachable code (due to never ending loop)
            //

            // Close the serial device
            //await serial.CloseAsync();

            // dispose the SerialPort
            //serialPort.Dispose();
        }



    }
}

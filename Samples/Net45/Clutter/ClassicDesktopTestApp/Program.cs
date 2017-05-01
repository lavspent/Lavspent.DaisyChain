using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Devices.Holtek.Ht16K33;
using Lavspent.DaisyChain.Devices.Honeywell.Hmc5883L;
using Lavspent.DaisyChain.Devices.TexasInstruments.Tca9548A;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ClassicDesktopTestApp
{
    class Program
    {
        static FirmataClient _firmataClient;

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {

            // Setup serial comminucation on COM3
            var serialPort = await SerialPortController.OpenSerialAsync("COM6", 57600, Parity.None, 8, StopBits.One);

            // Initialize a Firmata client connected to that stream
            // (we assume that there is a Firmata server in hte other end).
            _firmataClient = await FirmataClient.OpenAsync(serialPort);

            // Grab the I2cController provided by the firmata server
            II2cController i2cController = await _firmataClient.GetI2cControllerAsync();

            // Set up connection to i2c device at address 0x1E
            II2cDevice i2cCompassDevice = i2cController.GetDevice(new I2cConnectionSettings(0x1E));

            // Treat this i2c device as a Hmc5883L compass chip.
            var hmc5883L = new Hmc5883L(i2cCompassDevice);
            await hmc5883L.SetMeasuringModeAsync(Hmc5883MeasuringModes.Continuous);

            ICompassReading compassReading = new CompassReading();
            int i = 0;
            while (true)
            {
                var reading = await hmc5883L.GetCompassReadingAsync();
                Console.Clear();
                Console.WriteLine($"{i} {reading.AxisX,7:N2} {reading.AxisY,7:N2} {reading.AxisZ,7:N2} {reading.HeadingMagneticNorth,7:N2}");
            }

            //serialPort.Close();
            //serialPort.Dispose();
        }

        private static void _onConnected(Task t)
        {
            // We can't block current thread, then everything will freeze
            // Push a new task to the pool
            Task.Run(async () =>
            {
                // Grab the I2cController from the Firmata proxy
                II2cController i2cController = await _firmataClient.GetI2cControllerAsync();


                II2cDevice i2cCompassDevice = i2cController.GetDevice(new I2cConnectionSettings(0x1E));
                var hmc5883L = new Hmc5883L(i2cCompassDevice);
                await hmc5883L.SetMeasuringModeAsync(Hmc5883MeasuringModes.Continuous);

                while (true)
                {
                    var reading = await hmc5883L.GetCompassReadingAsync();
                    Console.WriteLine($"{reading.AxisX} {reading.AxisY} {reading.AxisZ} {reading.HeadingMagneticNorth}");
                    //Task.Delay(500).Wait();
                }
            });
        }


        private static void __onConnected(Task t)
        {
            // We can't block current thread, then everything will freeze
            // Push a new task to the pool
            Task.Run(async () =>
            {
                // Grab the I2cController from the Firmata proxy
                II2cController i2cController = await _firmataClient.GetI2cControllerAsync();

                // Get det I2cDevice with address 0x71 from the controller
                II2cDevice i2cDevice71 = i2cController.GetDevice(new I2cConnectionSettings(0x71));

                // Wrap it as a  Tca9548A I2c mux
                Tca9548A tca9548A = new Tca9548A(i2cDevice71);

                // Grab the I2cController for output 6 on the MUX
                II2cController i2cMux6Controller = tca9548A.GetI2cController(6);

                // Grab I2cDevice with address 0x70 from that controller
                II2cDevice i2cDevice = i2cMux6Controller.GetDevice(new I2cConnectionSettings(0x70));

                // Wrap it a a HT16K33 display driver
                Ht16K33 ht16K33 = new Ht16K33(i2cDevice);

                // Initialize and send som data to the display
                ht16K33.OscillatorEnabled = false;

                ht16K33.OscillatorEnabled = true;

                ht16K33.DisplayEnabled = false;
                ht16K33.Dimming = 15;
                ht16K33.Blink = Ht16K33.BlinkEnum.OneHz;

                byte[] b = new byte[] {
                    0x00,
                    0x80, 0x00, // 1
                    0x01, 0x00, // 2
                    0x02, 0x00,
                    0x04, 0x00,
                    0x08, 0x00,
                    0x10, 0x00,
                    0x20, 0x00,
                    0x40, 0x00
                    };

                i2cDevice.WriteAsync(b).Wait();

                ht16K33.DisplayEnabled = true;


                Task.Delay(200).Wait();

                // Grab the I2cController for output 2 on the MUX
                II2cController i2cMux2Controller = tca9548A.GetI2cController(2);

                // Grab I2cDevice with address 0x70 from that controller
                II2cDevice i2cDevice2 = i2cMux2Controller.GetDevice(new I2cConnectionSettings(0x70));

                // Wrap it a a HT16K33 display driver
                Ht16K33 ht16K33_ = new Ht16K33(i2cDevice2);

                // Initialize and send som data to the display
                ht16K33_.OscillatorEnabled = true;
                ht16K33_.DisplayEnabled = false;
                ht16K33_.Dimming = 15;
                ht16K33_.Blink = Ht16K33.BlinkEnum.OneHz;

                b = new byte[] {
                    0x00,
                    0x3F, 0x00, // 1
                    0x06, 0x00, // 2
                    0x02, 0x00,
                    0x5B, 0x00,
                    0x4F, 0x00,
                    0x00, 0x00,
                    0x00, 0x00,
                    0x00, 0x00
                    };

                i2cDevice2.WriteAsync(b).Wait();

                ht16K33_.DisplayEnabled = true;


                II2cDevice i2cCompassDevice = i2cMux2Controller.GetDevice(new I2cConnectionSettings(0x1E));
                var hmc5883L = new Hmc5883L(i2cCompassDevice);
                await hmc5883L.SetMeasuringModeAsync(Hmc5883MeasuringModes.Continuous);

                while (true)
                {
                    var reading = await hmc5883L.GetCompassReadingAsync();
                    Console.WriteLine($"{reading.AxisX} {reading.AxisY} {reading.AxisZ} {reading.HeadingMagneticNorth}");
                    //Task.Delay(500).Wait();
                }
            });
        }

    }



}

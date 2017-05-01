using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Devices.Generic;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Misc;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleFirmataInterruptGpioEncoder
{
    class Program
    {

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }


        static async Task MainAsync(string[] args)
        {
            // open serial (port)
            var serial = await SerialPortController.OpenSerialAsync("COM6", 57600, Parity.None, 8, StopBits.One);

            // Initialize a firmata client (we assume there is a firmata host/server on the other side)
            var firmataClient = await FirmataClient.OpenAsync(serial);

            // Get the gpio controller
            var gpioController = await firmataClient.GetGpioControllerAsync();

            // open gpios
            IGpio gpio2 = await gpioController.OpenGpioAsync(2);
            IGpio gpio3 = await gpioController.OpenGpioAsync(3);

            // set mode to input
            await gpio2.SetDriveModeAsync(GpioDriveMode.Input);
            await gpio3.SetDriveModeAsync(GpioDriveMode.Input);

            // open the encoder connected to those gpios
            var encoder = await GenericEncoder.OpenAsync(gpio2, gpio3);

            // subscribe to events from the encoder
            encoder.Subscribe(
                new Observer<EncoderReading>(
                    (encoderReading) =>
                    {
                        Console.WriteLine($"{encoderReading.Direction}");
                    }));


            // Loop forever 
            while (true)
            {
                await Task.Delay(1000);
            }

            // Unreachable code (due to never ending loop)
            //firmataClient.Dispose();
            //serial.Dispose();
        }

    }
}

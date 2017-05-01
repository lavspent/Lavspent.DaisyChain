using Lavspent.DaisyChain.Adc;
using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Devices.Generic;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Misc;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleFirmataRotaryEncoder
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

            // Get the adc channel controller
            var adcController = await firmataClient.GetAdcControllerAsync();

            // Get the adc channels
            IAdcChannel adc0 = await adcController.OpenAdcChannelAsync(0);
            IAdcChannel adc1 = await adcController.OpenAdcChannelAsync(1);

            // turn those adc channels into digital ones
            IGpio gpio0 = adc0.AsGpio();
            IGpio gpio1 = adc1.AsGpio();

            // open encoder connected to those gpios
            var encoder = await GenericEncoder.OpenAsync(gpio0, gpio1);

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

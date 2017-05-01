using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Misc;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleConfigurableFirmataRotaryEncoder
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

            // Initialize a firmata client. We assume there is a ConfigurableFirmata/or other firmata with support for Encoders on the other side.
            var firmataClient = await FirmataClient.OpenAsync(serial);

            // get encoder controller
            var encoderController = await firmataClient.GetEncoderControllerAsync();

            // get encoder #0 connected to pin 2 and 3.
            var encoder = await encoderController.OpenEncoderAsync(0, 2, 3);

            // subscribe to events from the encoder
            encoder.Subscribe(
                new Observer<EncoderReading>(
                    (encoderReading) =>
                    {
                        Console.WriteLine($"{encoderReading.Direction} {encoderReading.Ticks}");
                    }));


            // Loop forever 
            while (true)
            {
                await Task.Delay(1000);
            }

            //encoder.Dispose();
            //firmataClient.Dispose();
            //serial.Dispose();
        }
    }
}

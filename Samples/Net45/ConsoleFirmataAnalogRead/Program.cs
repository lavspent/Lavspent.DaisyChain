using Lavspent.DaisyChain.Adc;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Misc;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleFirmataAnalogRead
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

            // Get the analog gpio controller
            var analogGpioController = await firmataClient.GetAdcControllerAsync();

            // Get the analog pin
            IAdcChannel analogPin = await analogGpioController.OpenAdcChannelAsync(0);

            // Subscribe to pin updates
            analogPin.Subscribe(
                new Observer<IAdcChannelValue>(
                    (analogGpioValue) =>
                    {
                        // and write any updates to the console
                        Console.WriteLine($"Pin value: {analogGpioValue.Value}");
                    },
                    null,
                    null
                ));

            // loop forever
            while (true)
            {
                await Task.Delay(500);
            }

            // Unreachable 
            //firmataClient.Dispose();
            //serial.Dispose();
        }
    }
}

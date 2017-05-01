using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleFirmataGpio
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

            // Initialize firmata proxy
            var firmataClient = await FirmataClient.OpenAsync(serialPort);

            // Get the GpioController
            var gpioController = await firmataClient.GetGpioControllerAsync();

            // Get the gpio "pin" 13.
            IGpio gpio = await gpioController.OpenGpioAsync(13);

            // Loop forever flipping gpio (pin) 13.
            // On an arduino this is will flicker it's onboard led.
            while (true)
            {
                await gpio.WriteAsync(GpioValue.Low);
                await Task.Delay(new Random().Next(100));
                await gpio.WriteAsync(GpioValue.High);
                await Task.Delay(new Random().Next(100));
            }

            // unreachable
            //gpio.Dispose();
            //firmataClient.Dispose();
            //serialPort.Dispose();
        }
    }
}
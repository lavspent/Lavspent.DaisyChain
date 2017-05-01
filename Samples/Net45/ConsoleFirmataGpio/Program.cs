using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Serial;
using System.Threading.Tasks;

namespace ConsoleFirmataGpio
{
    //class Program2
    //{
    //    static void Main(string[] args)
    //    {
    //        MainAsync(args).Wait();
    //    }

    //    static async Task MainAsync(string[] args)
    //    {
    //        // Setup serial comminucation on COM6
    //        var serialPort = await SerialPortController.OpenSerialAsync("COM4", 57600, Parity.None, 8, StopBits.One);

    //        // Initialize Firmata client
    //        var firmataClient = await FirmataClient.OpenAsync(serialPort);

    //        // Get it's GpioController
    //        var gpioController = await firmataClient.GetGpioControllerAsync();

    //        // Open Gpio "pin" 13 on the GpioController
    //        IGpio gpio = await gpioController.OpenGpioAsync(13);

    //        // Loop forever flipping gpio (pin) 13.
    //        // On an Arduino this will flicker it's onboard led.
    //        while (true)
    //        {
    //            await gpio.WriteAsync(GpioValue.Low);
    //            await Task.Delay(1000);
    //            await gpio.WriteAsync(GpioValue.High);
    //            await Task.Delay(1000);
    //        }
    //    }
    //}

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            // On serial port, open a firmata client, get the gpio controller and then open pin 13
            var gpio =
                await SerialPortController.OpenSerialAsync("COM4", 57600, Parity.None, 8, StopBits.One)
                .OpenFirmataClientAsync()
                .GetGpioControllerAsync()
                .OpenGpioAsync(13);

            // Loop forever flipping gpio (pin) 13.
            // On an Arduino this will flicker it's onboard led.
            while (true)
            {
                await gpio.WriteAsync(GpioValue.Low);
                await Task.Delay(1000);
                await gpio.WriteAsync(GpioValue.High);
                await Task.Delay(1000);
            }
        }
    }
}
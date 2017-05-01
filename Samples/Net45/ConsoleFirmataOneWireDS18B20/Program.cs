using Lavspent.DaisyChain.Devices.MaximIntegrated.Ds18B20;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.OneWire;
using Lavspent.DaisyChain.Serial;
using Lavspent.TaskEnumerableExtensions;
using System;
using System.Linq;
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
            // get firmata client
            var firmataClient = await
                SerialPortController.OpenSerialAsync("COM4", 57600, Parity.None, 8, StopBits.One)
                .OpenFirmataClientAsync();

            // get 1-wire bus on pin 3
            var oneWireBus = await
                firmataClient
                .GetOneWireBusControllerAsync()
                .OpenOneWireBusAsync(3);

            // enumerate addresses
            var addresses = await oneWireBus.GetDeviceAddressesAsync();

            // open temp sensor
            var ds18b20 = await Ds18B20.OpenDeviceAsync(oneWireBus, addresses.First());

            // open gpio on pin 13
            var gpioController = await firmataClient.GetGpioControllerAsync();
            var gpio = await gpioController.OpenGpioAsync(13);

            while (true)
            {
                await gpio.WriteAsync(Lavspent.DaisyChain.Gpio.GpioValue.High);
                await ds18b20.ConvertTemperatureAsync();
                await Task.Delay(500);
                await gpio.WriteAsync(Lavspent.DaisyChain.Gpio.GpioValue.Low);
                await Task.Delay(500);
                var result = await ds18b20.GetTemeperatureReadingAsync();
                Console.WriteLine(result.Celcius);
            }

            //            // loop forever
            //            while (true)
            //            {
            //                await Task.Delay(1000);
            //            }

            // unreachable
            //firmataClient.Dispose();
            //serialPort.Dispose();
        }
    }
}
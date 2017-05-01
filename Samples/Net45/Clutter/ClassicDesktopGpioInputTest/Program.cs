using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Serial;
using System;
using System.IO.Ports;
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
            // Setup serial comminucation on COM6
            var serialPort = new SerialPort("COM6", 57600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            var serial = serialPort.AsDaisyChainSerial();
            await serial.OpenAsync();

            Console.WriteLine("Serial port open.");

            // Initialize firmata proxy
            _firmataClient = await FirmataClient.OpenAsync(serial);

            IGpio gpio = await _firmataClient.GetGpioControllerAsync().OpenGpioAsync(13);
            await gpio.WriteAsync(GpioValue.Low);

            while (true)
            {
                await Task.Delay(500);
                await gpio.WriteAsync(GpioValue.Low);
                await Task.Delay(500);
                await gpio.WriteAsync(GpioValue.High);
            }

            //    Console.WriteLine("Firmata client connected.");

            //    await _firmataClient.ReportAnalogPinAsync(0, true);

            //    var analogGpioController = _firmataClient.GetAnalogGpioController();
            //    IAnalogGpio analogPin = await analogGpioController.OpenAnalogGpioAsync(0);

            //    analogPin.Subscribe(
            //        new Observer<IAnalogGpioValue>(
            //            (analogGpioValue) =>
            //            {
            //                Console.WriteLine($"OOB: {analogGpioValue.Value}");
            //            },
            //            null,
            //            null
            //        ));

            //    while (true)
            //    {
            //        var value = await analogPin.ReadAsync();
            //        Console.WriteLine($"Pin value: {value.Value}, ratio: {value.Ratio}");
            //        await Task.Delay(500);
            //    }

            //    serialPort.Close();
            //    serialPort.Dispose();
        }
    }
}

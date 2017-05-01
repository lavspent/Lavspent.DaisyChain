using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Stream;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FirmataServiceClientSocketTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task<int> MainAsync(string[] args)
        {
            await ClientLoop();
            return 0;
        }

        static TcpClient tcpClient = null;
        static FirmataClient firmataClient = null;
        static async Task ClientLoop()
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 12001);
            IStream stream = tcpClient.GetStream().AsDaisyChainStream();

            firmataClient = await FirmataClient.OpenAsync(stream);

            Console.WriteLine($"{firmataClient.Firmware} {firmataClient.MajorVersion}.{firmataClient.MinorVersion}");

            var cap = firmataClient.PinCapabilities;
            Console.WriteLine(cap.ToString());

            IGpioController gpioController = await firmataClient.GetGpioControllerAsync();
            IGpio gpio = await gpioController.OpenGpioAsync(1);
            await gpio.WriteAsync(GpioValue.High);
            await gpio.WriteAsync(GpioValue.Low);
            await gpio.WriteAsync(GpioValue.High);

            Console.ReadLine();
            return;
        }

    }
}

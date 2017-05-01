using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Stream;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FirmataServiceServerSocketTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }


        static async Task<int> MainAsync(string[] args)
        {
            await ServerLoop();
            return 0;
        }

        static MyFirmataServer myFirmataServer = null;

        static async Task ServerLoop()
        {
            IPAddress addr = IPAddress.Any;
            IPEndPoint ep = new IPEndPoint(addr, 12001);
            TcpListener tcpListener = new TcpListener(ep);
            tcpListener.Start();

            Console.WriteLine("Listening...");

            //while (true)
            //{
            var tcpClient = await tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("Connected!");

            // get and wrap stream
            IStream stream = tcpClient.GetStream().AsDaisyChainStream();
            myFirmataServer = new MyFirmataServer();
            await myFirmataServer.ConnectAsync(stream);
        }
    }



    class MyFirmataServer : AbstractFirmataServer
    {
        public MyFirmataServer()
            : base()
        {
        }
    }
}

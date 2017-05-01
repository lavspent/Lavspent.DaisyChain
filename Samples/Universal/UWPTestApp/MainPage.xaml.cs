using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTestApp
{
    /// <summary>
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SerialDevice _serialDevice;
        FirmataClient _firmataClient;

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // open up serial device
            string aqsFilter = SerialDevice.GetDeviceSelector();
            var x = await DeviceInformation.FindAllAsync(aqsFilter);
            var y = x[0];

            _serialDevice = await SerialDevice.FromIdAsync(y.Id);
            _serialDevice.Handshake = SerialHandshake.None;
            _serialDevice.BaudRate = 57600;
            _serialDevice.IsDataTerminalReadyEnabled = true;
            _serialDevice.DataBits = 8;
            _serialDevice.Parity = SerialParity.None;
            _serialDevice.StopBits = SerialStopBitCount.One;
            _serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(1);

            // convert to a daisy chain serial, open a firmata client on it, 
            // get the gpio controller and then open pin 2
            IGpio pin =
                await _serialDevice
                .AsDaisyChainSerial()
                .OpenFirmataClientAsync()
                .GetGpioControllerAsync()
                .OpenGpioAsync(2);

            // loop switching the pin high and low
            while (true)
            {
                await pin.WriteAsync(GpioValue.High);
                await Task.Delay(500);
                await pin.WriteAsync(GpioValue.Low);
                await Task.Delay(500);
            }
        }
    }
}

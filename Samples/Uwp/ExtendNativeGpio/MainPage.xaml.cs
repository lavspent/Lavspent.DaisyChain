using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Mocks.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExtendNativeGpio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // current default gpio controller (on a desktop that is null)
            var defaultGpioController = Windows.Devices.Gpio.GpioController.GetDefault(); //?.AsDaisyChainGpioController();

            // create mock
            MockGpioController mockGpioController = new MockGpioController();

            // wrap as native GpioControllerProvider
            var nativeMockGpioControllerProvider = mockGpioController.AsUwpGpioControllerProvider();

            // replace the native controller providers
            Windows.Devices.LowLevelDevicesController.DefaultProvider =
                new Windows.Devices.LowLevelDevicesAggregateProvider(
                    null, null,
                    nativeMockGpioControllerProvider,
                    null, null);

            // get a native version of our mock
            defaultGpioController = Windows.Devices.Gpio.GpioController.GetDefault();

            var gpioPin = defaultGpioController.OpenPin(0);

            // subscribe to it
            gpioPin.ValueChanged += GpioPin_ValueChanged;

            // write to it
            gpioPin.Write(Windows.Devices.Gpio.GpioPinValue.High);

            return;
        }

        private void GpioPin_ValueChanged(Windows.Devices.Gpio.GpioPin sender, Windows.Devices.Gpio.GpioPinValueChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Edge);
            return;
        }
    }

}

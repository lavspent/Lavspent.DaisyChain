using Android.App;
using Android.OS;
using Android.Widget;
using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Stream;
using System.Net.Sockets;

namespace XamarinAndriodTest
{
    [Activity(Label = "XamarinAndriodTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += Button_Click;
            //                += delegate { button.Text = string.Format("{0} clicks!", count++); };

        }

        private async void Button_Click(object sender, System.EventArgs e)
        {
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Text = "Initializing...";

            TcpClient client = new TcpClient();

            button.Text = "Connecting";

            //client.Connect("127.0.0.1", 12001);
            //client.Connect("10.0.2.2", 12001);
            client.Connect("169.254.80.80", 12001);


            var stream = client.GetStream().AsDaisyChainStream();

            button.Text = "Stream obtained. Connecting Firmata...";

            FirmataClient firmataClient = await FirmataClient.OpenAsync(stream);

            button.Text = "Firmata connected.";

            var gpioController = await firmataClient.GetGpioControllerAsync();
            var gpio = await gpioController.OpenGpioAsync(1);
            await gpio.WriteAsync(GpioValue.High);
            await gpio.WriteAsync(GpioValue.Low);
            await gpio.WriteAsync(GpioValue.High);

            button.Text = "Pin written to.";

            return;
        }
    }
}


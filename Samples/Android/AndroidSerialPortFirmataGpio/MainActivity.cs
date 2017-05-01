using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Widget;
using Hoho.Android.UsbSerial.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AndroidSerialPortFirmataGpio
{
    [Activity(Label = "AndroidSerialPortFirmataGpio", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += Button_Click;
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            UsbManager usbManager;
            usbManager = GetSystemService(Context.UsbService) as UsbManager;


            var list = await FindAllDriversAsync(usbManager);

            return;
            /*//UsbSerialPortAdapter adapter;
            BroadcastReceiver detachedReceiver;
            IUsbSerialPort selectedPort;


            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Text = string.Format("{0} clicks!", count++);*/
        }

        internal static Task<IList<IUsbSerialDriver>> FindAllDriversAsync(UsbManager usbManager)
        {
            // using the default probe table
            // return UsbSerialProber.DefaultProber.FindAllDriversAsync (usbManager);

            // adding a custom driver to the default probe table
            var table = UsbSerialProber.DefaultProbeTable;
            table.AddProduct(0x1b4f, 0x0008, Java.Lang.Class.FromType(typeof(CdcAcmSerialDriver))); // IOIO OTG
            var prober = new UsbSerialProber(table);
            return prober.FindAllDriversAsync(usbManager);
        }

    }
}


using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EnumerateDevices
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        DeviceWatcher watcher;
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            string serialDeviceAqs = SerialDevice.GetDeviceSelector();
            var deviceInformationCollection = await DeviceInformation.FindAllAsync(serialDeviceAqs);


            SerialDevice serialDevice = null;


            /*            watcher = DeviceInformation.CreateWatcher();
                        watcher.Added += Watcher_Added;
                        watcher.Start();
            */
            //            var deviceInformationCollection = await DeviceInformation.FindAllAsync();
            var numDevices = deviceInformationCollection.Count;


            if (numDevices > 0)
            {
                for (var i = 0; i < numDevices; i++)
                {
                    var deviceInformation = deviceInformationCollection[i];
                    Debug.WriteLine($"{deviceInformation.Name}     {deviceInformation.Kind}");

                    serialDevice = await SerialDevice.FromIdAsync(deviceInformation.Id);
                    break;
                }
            }
            else
            {
                Debug.WriteLine("No devices found");
            }


            if (serialDevice != null)
            {
                Debugger.Break();
                // device found
            }
            else
            {
                // device not found
                Debugger.Break();
            }
        }

        private void Watcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Debug.WriteLine($"Added: {args.Name}     {args.Kind}");
        }
    }
}

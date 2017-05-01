/*
    Copyright(c) 2017 Petter Labråten. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a
    copy of this software and associated documentation files (the "Software"),
    to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense,
    and/or sell copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    DEALINGS IN THE SOFTWARE.
*/

using Lavspent.DaisyChain.Gpio;
using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Azure
{
    public class AzureIotHubGpioController : IGpioController
    {
        private string _deviceConnectionString;
        private DeviceClient _deviceClient;

        private string _remoteDeviceId;
        private string _sharedSecret;

        public AzureIotHubGpioController(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient;
            //PinCount = pinCount;
        }

        private async Task ConnectAsync(string deviceConnectionString, string remoteDeviceId, string sharedSecret)
        {
            _deviceConnectionString = deviceConnectionString;
            _remoteDeviceId = remoteDeviceId;
            _sharedSecret = sharedSecret;

            /*
            _deviceClient = DeviceClient.CreateFromConnectionString(_deviceConnectionString);
            if (_deviceClient == null)
                throw new Exception("Failed to create Azure DeviceClient.");
             */

            // send request to get controller info
            await _deviceClient.SendEventAsync(new Message(new byte[] { 1, 2, 3 })).ConfigureAwait(false);

            while (true)
            {
                var message = await _deviceClient.ReceiveAsync().ConfigureAwait(false);
                if (message != null)
                {
                    Debugger.Break();
                }
                else
                    await Task.Delay(1000).ConfigureAwait(false);
            }


            //return;
            //return Task.CompletedTask;
        }

        //public static async Task<IGpioController> OpenAsync(DeviceClient deviceClient, string deviceConnectionString)
        //{
        //    AzureIotHubGpioController controller = new AzureIotHubGpioController(deviceClient);

        //    // todo: wrong
        //    await controller.ConnectAsync(deviceConnectionString, null, null).ConfigureAwait(false);
        //    return controller;
        //}

        public int GpioCount
        {
            get;
            private set;
        }

        public Task<IGpio> OpenGpioAsync(int pinNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}

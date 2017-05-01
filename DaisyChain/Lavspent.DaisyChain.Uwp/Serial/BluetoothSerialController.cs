/*
    Copyright(c) 2017 Petter Labråten/LAVSPENT.NO. All rights reserved.

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

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace Lavspent.DaisyChain.Serial
{
    public class BluetoothSerialController : ISerialController
    {
        public Task<ISerial> OpenSerialAsync(PortName portName, int baud, Parity parity, byte dataBits, StopBits stopBits)
        {
            throw new NotImplementedException();
        }

        public static async Task test()
        {
            string aqs = RfcommDeviceService.GetDeviceSelector(Windows.Devices.Bluetooth.Rfcomm.RfcommServiceId.SerialPort);
            var list = await DeviceInformation.FindAllAsync(aqs);
            var c = list.Count;
            if (c > 0)
            {
                var first = list[0];
                Debugger.Break();
            }

            return;
        }
    }
}

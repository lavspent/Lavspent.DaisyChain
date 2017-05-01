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

using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.Gpio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata.Gpio
{
    internal class FirmataGpioController : IGpioController
    {
        private FirmataClient _firmataClient;

        public FirmataGpioController(FirmataClient firmataClient)
        {
            _firmataClient = firmataClient;
        }

        public int GpioCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // todo: fix pinNumber int/byte
        public async Task<IGpio> OpenGpioAsync(int pinNumber, CancellationToken cancellationToken)
        {
            await ReportDigitalPinAsync((byte)pinNumber, true, cancellationToken);

            IGpio gpio;

            //TODO: Do sharing and pin count checking
            gpio = new FirmataGpio(this, pinNumber);

            return gpio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable RegisterDigitalMessageObserver(IObserver<DigitalMessage> observer)
        {
            return _firmataClient.RegisterResponseObserver(DigitalMessage.MessageDescriptor, observer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="pinMode"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task SetDigitalPinModeAsync(byte pin, PinMode pinMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _firmataClient.SendMessageAsync(
                SetPinMode.MessageDescriptor,
                new byte[] { pin, (byte)pinMode },
                cancellationToken
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task SetDigitalPinValueAsync(byte pin, GpioValue value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _firmataClient.SendMessageAsync(
                new SetDigitalPinValue()
                {
                    Pin = pin,
                    Value = value
                },
                cancellationToken
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<GpioValue> ReadDigitalPinValueAsync(int pin, CancellationToken cancellationToken)
        {
            // todo: implement
            return System.Threading.Tasks.Task.FromResult<GpioValue>(GpioValue.High);
        }

        public Task ReportDigitalPinAsync(byte pin, bool enable, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _firmataClient.SendMessageAsync(
                new ReportDigitalPin()
                {
                    Pin = pin,
                    Enable = enable
                },
                cancellationToken);
        }
    }
}

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

using Lavspent.DaisyChain.Adc;
using System;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata.Adc
{
    internal class FirmataAdcController : IAdcController
    {
        private FirmataClient _firmataClient;

        public FirmataAdcController(FirmataClient firmataClient)
        {
            _firmataClient = firmataClient;
        }

        public int ChannelCount
        {
            get
            {
                // todo: implement 
                //return _firmataClient.PinCapabilities;
                return 1;
            }
        }

        public int MaxValue
        {
            get
            {
                return (1 << ResolutionInBits) - 1;
            }
        }

        public int MinValue
        {
            get
            {
                return 0;
            }
        }

        public int ResolutionInBits
        {
            get
            {
                // todo: Firmata supportes more than 10, right?
                return 10;
            }
        }


        // todo: fix int/byte pinNumber
        public async Task<IAdcChannel> OpenAdcChannelAsync(int pinNumber)
        {
            // Turn on analog reporting
            // todo: fix this pin number
            await _firmataClient.ReportAnalogPinAsync((byte)pinNumber, true);

            // todo: as long as the FirmataAnalogGpio exitsts we should have a 
            // way to stop anyone form turnong of reporting on this pin.

            // todo: check pin number, sharing mode etc.
            IAdcChannel analogGpio = new FirmataAdcChannel(_firmataClient, pinNumber);
            return analogGpio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelMode"></param>
        /// <returns></returns>
        public bool IsChannelModeSupported(AdcChannelMode channelMode)
        {
            switch (channelMode)
            {
                case AdcChannelMode.SingleEnded:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AdcChannelMode ChannelMode
        {
            get
            {
                return AdcChannelMode.SingleEnded;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

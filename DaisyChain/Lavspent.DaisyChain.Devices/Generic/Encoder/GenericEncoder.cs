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

using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Misc;
using System;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.Generic
{
    /// <summary>
    /// </summary>
    public class GenericEncoder : IEncoder, IDisposable
    {
        private IGpio _clkGpio;
        private IGpio _dtGpio;

        private IDisposable _clkGpioSubscription;
        private IDisposable _dtGpioSubscription;

        private GpioEdge _lastDtEdge;

        private Source<EncoderReading> _encoderSource;

        /// <summary>
        /// Constructor is private. To initialize an instance we need async calls, which cannot be performed in
        /// a constructor. Initialization is in InitAsync. To create instace call CreateAsync.
        /// </summary>
        private GenericEncoder()
        {
            // Handle all "constructing" in InitAsync
        }

        private async Task InitAsync(IGpio clkGpio, IGpio dtGpio)
        {
            _encoderSource = new Source<EncoderReading>();

            _clkGpio = clkGpio;
            _dtGpio = dtGpio;

            // determine last edge on the dt pin
            _lastDtEdge = (await _dtGpio.ReadAsync().ConfigureAwait(false)) == GpioValue.Low ? GpioEdge.FallingEdge : GpioEdge.RisingEdge;

            // observe dt pins
            _dtGpioSubscription = _dtGpio.Subscribe(
                new Observer<GpioEdge>(
                (edge) =>
                {
                    // store dt edge
                    _lastDtEdge = edge;
                }
                ));

            // observe clk pin
            _clkGpioSubscription = _clkGpio.Subscribe(
                new Observer<GpioEdge>(
                (edge) =>
                {
                    EncoderReading.DirectionEnum direction;

                    if (edge == _lastDtEdge)
                    {
                        // dt pin changed first,  counter clockwize
                        direction = EncoderReading.DirectionEnum.CounterClockwise;
                    }
                    else
                    {
                        // clock pin changed first, clockwize
                        direction = EncoderReading.DirectionEnum.Clockwise;
                    }

                    _encoderSource.Next(new EncoderReading() { Direction = direction, Ticks = 1 });
                }
                ));

        }

        public static async Task<IEncoder> OpenAsync(IGpio clkGpio, IGpio dtGpio)
        {
            var genericEncoder = new GenericEncoder();
            await genericEncoder.InitAsync(clkGpio, dtGpio);
            return genericEncoder;
        }

        public void Dispose()
        {
            _clkGpioSubscription?.Dispose();
            _dtGpioSubscription?.Dispose();
        }

        /// <summary>
        /// Subscribe to encoder changes.
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<EncoderReading> observer)
        {
            return _encoderSource.Subscribe(observer);
        }
    }
}

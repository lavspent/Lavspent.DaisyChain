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

using Lavspent.DaisyChain.OneWire;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.MaximIntegrated.Ds18B20
{
    public enum Ds18B20ResolutionEnum : byte
    {
        NineBits = 0,       // 0.5 degrees
        TenBits = 32,       // 0.25 degrees
        ElevenBits = 64,    // 0.125 degrees
        TwelveBits = 96     // 0.0625 degrees
    }

    //[OneWireDeviceFactory(typeof(Ds18B20Factory))]
    public class Ds18B20 : IOneWireDevice, IThermometer
    {
        private IOneWireBus oneWireBus;
        private OneWireAddress address;
        Ds18B20ResolutionEnum resolution;

        public static Task<Ds18B20> OpenDeviceAsync(IOneWireBus oneWireBus, OneWireAddress address, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(new Ds18B20(oneWireBus, address));
        }

        public Ds18B20(IOneWireBus oneWireBus, OneWireAddress address)
        {
            this.oneWireBus = oneWireBus;
            this.address = address;
            this.resolution = Ds18B20ResolutionEnum.TwelveBits; // chip default
        }

        public async Task SetResolutionAsync(Ds18B20ResolutionEnum resolution)
        {
            await oneWireBus.ResetAsync().ConfigureAwait(false);
            await MatchRomAsync().ConfigureAwait(false);
            this.resolution = resolution;
            await oneWireBus.WriteAsync(
                new byte[] { (byte)Ds18B20Commands.WriteScratchpad, 0, 0, (byte)(31 | (byte)resolution) }
                ).ConfigureAwait(false);
        }

        public byte[] Address
        {
            get
            {
                return address;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task MatchRomAsync()
        {
            return oneWireBus.MatchRomAsync(address);
        }

        /// <summary>
        /// Start temperature conversion on all sensors on a bus.
        /// </summary>
        /// <param name="oneWireBus"></param>
        /// <returns></returns>
        public static async Task ConvertTemperatureAsync(IOneWireBus oneWireBus)
        {
            await oneWireBus.ResetAsync().ConfigureAwait(false);
            await oneWireBus.SkipRomAsync().ConfigureAwait(false);
            await oneWireBus.WriteAsync(Ds18B20Commands.ConvertTemperature).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task ConvertTemperatureAsync()
        {
            await oneWireBus.ResetAsync().ConfigureAwait(false);
            await MatchRomAsync().ConfigureAwait(false);
            await oneWireBus.WriteAsync(Ds18B20Commands.ConvertTemperature).ConfigureAwait(false);
        }


        public async Task<byte[]> ReadScratchPadAsync()
        {
            byte[] readScratchPadReadBuffer = new byte[9];

            await oneWireBus.ResetAsync().ConfigureAwait(false);
            await MatchRomAsync().ConfigureAwait(false);
            await oneWireBus.WriteAsync((byte)Ds18B20Commands.ReadScratchPad).ConfigureAwait(false);
            await Task.Delay(100); // todo: wny this?
            await oneWireBus.ReadAsync(readScratchPadReadBuffer, readScratchPadReadBuffer.Length).ConfigureAwait(false);

            return readScratchPadReadBuffer;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reading"></param>
        /// <returns></returns>
        public async Task<ITemperatureReading> GetTemeperatureReadingAsync()
        {
            byte[] scratchPad = await ReadScratchPadAsync().ConfigureAwait(false);

            // todo: validation

            return new TemperatureReading()
            {
                Celcius = ((scratchPad[1] * 256) + scratchPad[0]) / 16.0
            };
        }
    }
}

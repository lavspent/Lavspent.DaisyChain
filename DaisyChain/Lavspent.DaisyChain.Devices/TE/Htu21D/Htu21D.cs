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

using Lavspent.DaisyChain.I2c;
using System;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.TE.Htu21D
{
    internal class Htu21DCommands
    {
        public const byte SampleTemperatureHold = 0xE3;
        public const byte SampleHumidityHold = 0xE5;
    }

    public class Htu21D : IThermometer
    {
        private II2cDevice _i2cDevice;

        public Htu21D(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }


        private async Task<ushort> ReadRawHumidityAsync()
        {
            ushort humidity = 0;
            byte[] humidityData = new byte[3];

            await _i2cDevice.WriteReadAsync(new byte[] { Htu21DCommands.SampleHumidityHold }, humidityData);

            //
            humidity = (ushort)((humidityData[0] << 8) | (humidityData[1] & 0xFC));

            // todo: validate

            return humidity;
        }

        public async Task<float> ReadHumidityAsync()
        {
            ushort rawHumidityData = await ReadRawHumidityAsync();
            double relativeHumidity = ((125.0 * rawHumidityData) / 65536) - 6.0;
            return Convert.ToSingle(relativeHumidity);
        }

        /// <summary>
        /// Gets the raw temperature value from the IC.
        /// </summary>
        private async Task<ushort> ReadRawTemperatureAsync()
        {
            ushort temperature = 0;
            byte[] temperatureData = new byte[3];

            //
            await _i2cDevice.WriteReadAsync(new byte[] { Htu21DCommands.SampleTemperatureHold }, temperatureData);

            //
            temperature = (ushort)((temperatureData[0] << 8) | (temperatureData[1] & 0xFC));

            // todo: validate

            return temperature;
        }


        public async Task<ITemperatureReading> GetTemeperatureReadingAsync()
        {
            var reading = new TemperatureReading();

            ushort rawTemperatureData = await ReadRawTemperatureAsync();
            reading.Celcius = ((175.72d * rawTemperatureData) / 65536d) - 46.85d;

            // todo: validation

            return reading;
        }
    }
}
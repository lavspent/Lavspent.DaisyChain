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

using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices
{
    /// <summary>
    /// 
    /// </summary>
    public class OffsetCalibratedGyrometer : IGyrometer
    {
        private IGyrometer _gyrometer;
        private double _offsetX, _offsetY, _offsetZ;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gyrometer"></param>
        public OffsetCalibratedGyrometer(IGyrometer gyrometer)
        {
            _gyrometer = gyrometer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="sampleDelay"></param>
        /// <returns></returns>
        public async Task Calibrate(int samples, int sampleDelay)
        {

            double offsetX = 0.0d, offsetY = 0.0d, offsetZ = 0.0;

            for (int i = 0; i < samples; i++)
            {
                await Task.Delay(sampleDelay).ConfigureAwait(false);

                var reading = await _gyrometer.GetGyrometerReadingAsync().ConfigureAwait(false);
                offsetX += reading.AngularVelocityX;
                offsetY += reading.AngularVelocityY;
                offsetZ += reading.AngularVelocityZ;
            }

            _offsetX = -offsetX / samples;
            _offsetY = -offsetY / samples;
            _offsetZ = -offsetZ / samples;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IGyrometerReading> GetGyrometerReadingAsync()
        {
            var reading = await _gyrometer.GetGyrometerReadingAsync().ConfigureAwait(false);

            // adjust for offset
            reading.AngularVelocityX += _offsetX;
            reading.AngularVelocityY += _offsetY;
            reading.AngularVelocityZ += _offsetZ;

            return reading;
        }
    }
}
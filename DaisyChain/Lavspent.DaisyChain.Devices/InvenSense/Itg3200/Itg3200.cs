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

namespace Lavspent.DaisyChain.Devices.InvenSense.Itg3200
{
    /// <summary>
    /// 
    /// </summary>
    public class Itg3200 : IGyrometer
    {
        II2cDevice _i2cDevice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2cDevice"></param>
        public Itg3200(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        private async Task<short> ReadRegisterHLAsync(byte registerHigh, byte registerLow)
        {
            byte valueHigh = await _i2cDevice.ReadRegisterAsync(registerHigh).ConfigureAwait(false);
            byte valueLow = await _i2cDevice.ReadRegisterAsync(registerLow).ConfigureAwait(false);
            return (short)(valueHigh << 8 | valueLow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync()
        {
            await _i2cDevice.WriteRegisterAsync(Itg3200Registers.PWR_M, 0x80).ConfigureAwait(false);
            await _i2cDevice.WriteRegisterAsync(Itg3200Registers.SMPL, 0x00).ConfigureAwait(false);
            await _i2cDevice.WriteRegisterAsync(Itg3200Registers.DLPF, 0x18).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<double> GetTemperatureAsync()
        {
            double registerValue = await ReadRegisterHLAsync(Itg3200Registers.TMP_H, Itg3200Registers.TMP_L).ConfigureAwait(false);
            return 35d + (registerValue + 13200d) / 280d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IGyrometerReading> GetGyrometerReadingAsync()
        {
            var gyrometerReading = new GyrometerReading();

            // read registers
            double angularVelocityX =
                await ReadRegisterHLAsync(Itg3200Registers.GX_H, Itg3200Registers.GX_L).ConfigureAwait(false) / 14.375d;
            double angularVelocityY =
                await ReadRegisterHLAsync(Itg3200Registers.GY_H, Itg3200Registers.GY_L).ConfigureAwait(false) / 14.375d;
            double angularVelocityZ =
                await ReadRegisterHLAsync(Itg3200Registers.GZ_H, Itg3200Registers.GZ_L).ConfigureAwait(false) / 14.375d;

            gyrometerReading.AngularVelocityX = angularVelocityX;
            gyrometerReading.AngularVelocityY = angularVelocityY;
            gyrometerReading.AngularVelocityZ = angularVelocityZ;

            // todo: Can we validate this somehow?

            return gyrometerReading;
        }
    }
}
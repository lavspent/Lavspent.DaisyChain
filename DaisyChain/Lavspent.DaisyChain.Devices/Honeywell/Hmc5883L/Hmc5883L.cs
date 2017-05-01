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

namespace Lavspent.DaisyChain.Devices.Honeywell.Hmc5883L
{
    public class Hmc5883L : ICompass
    {
        II2cDevice _i2cDevice;

        public Hmc5883L(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<ICompassReading> GetCompassReadingAsync()
        {
            byte[] readBuffer = new byte[6];

            await _i2cDevice.WriteReadAsync(new byte[] { 0x03 }, readBuffer).ConfigureAwait(false);

            double axisX = (short)((readBuffer[0] << 8) | readBuffer[1]);
            double axisY = (short)((readBuffer[2] << 8) | readBuffer[3]);
            double axisZ = (short)((readBuffer[4] << 8) | readBuffer[5]);

            axisX *= 0.92;
            axisY *= 0.92;
            axisZ *= 0.92;

            double heading = 0.0;
            if (axisY > 0)
                heading = 90.0 - Math.Atan(axisX / axisY) * 180.0 / Math.PI;
            else if (axisY < 0)
                heading = 270.0 - Math.Atan(axisX / axisY) * 180.0 / Math.PI;
            else if (axisY == 0 && axisX < 0)
                heading = 180.0;
            else
                heading = 0.0;

            double headingDeg = heading;// * 180 / Math.PI;

            var compassReading = new CompassReading();
            compassReading.AxisX = axisX;
            compassReading.AxisY = axisY;
            compassReading.AxisZ = axisZ;
            compassReading.HeadingMagneticNorth = headingDeg;

            return compassReading;
        }

        public Task SetMeasuringModeAsync(Hmc5883MeasuringModes measuringMode)
        {
            return _i2cDevice.WriteRegisterAsync(Hmc5883LRegisters.Mode, (byte)measuringMode);
        }
    }

    public class Hmc5883LRegisters
    {
        //Address = 0x1E

        public const byte ConfigurationA = 0x00;
        public const byte ConfigurationB = 0x01;
        public const byte Mode = 0x02;
        public const byte DataXH = 0x03;
        public const byte DataXL = 0x04;
        public const byte DataZH = 0x05;
        public const byte DataZL = 0x06;
        public const byte DataYH = 0x07;
        public const byte DataYL = 0x08;
        public const byte Status = 0x09;
        public const byte IdentificationA = 0x0A;
        public const byte IdentificationB = 0x0B;
        public const byte IdentificationC = 0x0C;
    }

    public enum Hmc5883MeasuringModes : byte
    {
        Continuous = 0x00,
        SingleShot = 0x01,
    }
}

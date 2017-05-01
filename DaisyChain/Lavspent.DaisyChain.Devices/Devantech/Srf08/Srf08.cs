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
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.Devantech.Srf08
{
    public class Srf08
    {
        // TODO: Clean up these
        public const byte Address = 0x70;
        private const byte  SoftwareRevisionRegister = 0x00;
        private const byte LightSensorRegister = 0x01;
        private const byte Command = 0x00;

        public enum MeasuringMode
        {
            Ranging,
            ANN
        }
   
        public enum MeasuringUnits : byte
        {
            Inches = 0,
            Centimeters,
            Microseconds,
        };

        private II2cDevice _i2cDevice;

        public Srf08(II2cDevice i2cDevivce)
        {
            _i2cDevice = i2cDevivce;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte> ReadSoftwareRevisionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _i2cDevice.ReadRegisterAsync(SoftwareRevisionRegister, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<byte> ReadLightSensorAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _i2cDevice.ReadRegisterAsync(SoftwareRevisionRegister, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measuringMode"></param>
        /// <param name="measuringUnits"></param>
        /// <returns></returns>
        private static byte BuildCommand(MeasuringMode measuringMode, MeasuringUnits measuringUnits)
        {
            byte command = 0x50;

            switch (measuringMode)
            {
                case MeasuringMode.Ranging:
                    command = 0;
                    break;
                case MeasuringMode.ANN:
                    command = 3;
                    break;
            }

            switch (measuringUnits)
            {
                case MeasuringUnits.Inches:
                    command += 0;
                    break;
                case MeasuringUnits.Centimeters:
                    command += 1;
                    break;
                case MeasuringUnits.Microseconds:
                    command += 2;
                    break;
            }

            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ushort> ReadRange(MeasuringMode measuringMode, MeasuringUnits measuringUnits, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte command = BuildCommand(measuringMode, measuringUnits);

            byte[] readBuffer = new byte[4];
            byte[] writeBuffer = new byte[] { (byte) Command, command };

            await _i2cDevice.WriteAsync(writeBuffer, cancellationToken);
            await Task.Delay(75);
            await _i2cDevice.ReadAsync(readBuffer, cancellationToken);

            ushort reading = (ushort)(readBuffer[3] << 8 + readBuffer[2]);
            return reading;
        }
    }
}
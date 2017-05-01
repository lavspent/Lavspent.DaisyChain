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

using Lavspent.AsyncInline;
using Lavspent.DaisyChain.I2c;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.AnalogDevices.Adxl345
{
    public enum Adxl345Registers : byte
    {
        DevId = 0x00,
        Reserved1 = 0x01,
        ThreshTap = 0x1d,
        OfsX = 0x1e,
        OfsY = 0x1f,
        OfsZ = 0x20,
        Dur = 0x21,
        Latent = 0x22,
        Window = 0x23,
        ThreshAct = 0x24,
        ThreshInact = 0x25,
        TimeInact = 0x26,
        ActInactCtl = 0x27,
        ThreshFF = 0x28,
        TimeFF = 0x29,
        TapAxes = 0x2a,
        ActTapStatus = 0x2b,
        BandwidthRate = 0x2c,
        PowerCtl = 0x2d,
        IntEnable = 0x2e,
        IntMap = 0x2f,
        IntSource = 0x30,
        DataFormat = 0x31,
        DataX0 = 0x32,
        DataX1 = 0x33,
        DataY0 = 0x34,
        DataY1 = 0x35,
        DataZ0 = 0x36,
        DataZ1 = 0x37,
        FifoCtl = 0x38,
        FifoStatus = 0x39
    }


    public enum BandwidthRateBits : byte
    {
        LowPowerEnabled = 4
    }

    public enum BandwidthRates : byte
    {
        Hz3200 = 0xf,
        Hz1600 = 0xe,
        Hz800 = 0xd,
        Hz400 = 0xc,
        Hz200 = 0xb,
        Hz100 = 0xa,
        Hz50 = 0x9,
        Hz25 = 0x8
        // TODO: There are more, left out for later excercice
    }

    public enum ActInactCtlBits : byte
    {
        InactivityZEnabled = 0,
        InactivityYEnabled = 1,
        InactivityXEnabled = 2,
        InactivityAcEnabled = 3,    // Opposite of Dc, doc page 24.
        ActivityZEnabled = 4,
        ActivityYEnabled = 5,
        ActivityXEnabled = 6,
        ActivityAcEnabled = 7,      // Opposite of Dc, doc page 24.
    }

    public enum DataFormatBits
    {
        InterruptInvert = 5,
        SpiBit = 6,
        TestBit = 7
    }


    public class Adxl345 : IAccelerometer
    {
        public const double Gravity = 9.80665d;
        public const double Mg2GMultiplier = 0.004d;


        II2cDevice _i2cDevice;
        private Adxl345(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        private async Task InitAsync()
        {
            await _i2cDevice.WriteRegisterAsync((byte)Adxl345Registers.PowerCtl, 0).ConfigureAwait(false);
            await _i2cDevice.WriteRegisterAsync((byte)Adxl345Registers.PowerCtl, 16).ConfigureAwait(false);
            await _i2cDevice.WriteRegisterAsync((byte)Adxl345Registers.PowerCtl, 8).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2cDevice"></param>
        /// <returns></returns>
        public async static Task<Adxl345> Create(II2cDevice i2cDevice)
        {
            Adxl345 adxl345 = new Adxl345(i2cDevice);
            await adxl345.InitAsync().ConfigureAwait(false);

            // chheck device id
            byte deviceId = await adxl345.GetDeviceIdAsync();
            if (deviceId != 0xE5)
            {
                throw new Exception($"Invalid device id {deviceId}, expected 0xE5.");
            }

            return adxl345;
        }

        public async Task<IAccelerometerReading> GetAccelerometerReadingAsync()
        {
            byte[] readBuffer = new byte[6];
            await _i2cDevice.WriteReadAsync(new byte[] { (byte)Adxl345Registers.DataX0 }, readBuffer).ConfigureAwait(false);

            short x = (short)(readBuffer[1] << 8 | readBuffer[0]);
            short y = (short)(readBuffer[3] << 8 | readBuffer[2]);
            short z = (short)(readBuffer[5] << 8 | readBuffer[4]);

            var reading = new AccelerometerReading()
            {
                X = x * 0.001953125, // Gravity * Mg2GMultiplier,
                Y = y * 0.001953125, //Gravity * Mg2GMultiplier,
                Z = z * 0.001953125 //Gravity * Mg2GMultiplier
            };

            return reading;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task<byte> GetDeviceIdAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _i2cDevice.ReadRegisterAsync((byte)Adxl345Registers.DevId);
        }

        public Task<byte> GetBandwidthRateRegisterAsync()
        {
            return _i2cDevice.ReadRegisterAsync((byte)Adxl345Registers.BandwidthRate);
        }

        public async Task<BandwidthRates> GetBandwidthRateAsync()
        {
            byte value = await GetBandwidthRateRegisterAsync().ConfigureAwait(false);
            return (BandwidthRates)(value & 0xf);
        }

        private Task<bool> ReadRegisterBitAsync(byte register, byte bit)
        {
            return _i2cDevice.ReadRegisterBitAsync(register, bit);
        }

        private Task WriteRegisterBitAsync(byte register, byte bit, bool value)
        {
            return _i2cDevice.WriteRegisterBitAsync(register, bit, value);
        }

        #region LowPowerEnabled

        public bool LowPowerEnabled
        {
            get => GetLowPowerEnabledAsync().WaitInline();
            set => SetLowPowerEnabledAsync(value).WaitInline();
        }

        public Task<bool> GetLowPowerEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.BandwidthRate, (byte)BandwidthRateBits.LowPowerEnabled);
        }

        public Task SetLowPowerEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.BandwidthRate, (byte)BandwidthRateBits.LowPowerEnabled, enabled);
        }

        #endregion

        #region ActivityXEnabled

        public bool ActivityXEnabled
        {
            get => GetActivityXEnabledAsync().WaitInline();
            set => SetActivityXEnabledAsync(value).WaitInline();
        }

        public Task<bool> GetActivityXEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.ActivityXEnabled);
        }

        public Task SetActivityXEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.ActivityXEnabled, enabled);
        }

        #endregion

        #region ActivityYEnabled

        public bool ActivityYEnabled
        {
            get => GetActivityYEnabledAsync().WaitInline();
            set => SetActivityYEnabledAsync(value).WaitInline();
        }

        public Task<bool> GetActivityYEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.ActivityYEnabled);
        }

        public Task SetActivityYEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.ActivityYEnabled, enabled);
        }

        #endregion

        #region ActivityZEnabled

        public bool ActivityZEnabled
        {
            get => GetActivityZEnabledAsync().WaitInline();
            set => SetActivityZEnabledAsync(value).WaitInline();
        }

        public Task<bool> GetActivityZEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.ActivityZEnabled);
        }

        public Task SetActivityZEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.ActivityZEnabled, enabled);
        }

        public Task<bool> GetInactivityXEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.InactivityXEnabled);
        }

        #endregion

        public Task SetInactivityXEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.InactivityXEnabled, enabled);
        }

        public Task<bool> GetInactivityYEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.InactivityYEnabled);
        }

        public Task SetInactivityYEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.InactivityYEnabled, enabled);
        }

        public Task<bool> GetInactivityZEnabledAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.InactivityZEnabled);
        }

        public Task SetInactivityZEnabledAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.ActInactCtl, (byte)ActInactCtlBits.InactivityZEnabled, enabled);
        }

        /*
                public Task<bool> GetAsync()
                {
                    return ReadRegisterBitAsync((byte)Adxl345Registers., (byte));
                }

                public Task SetAsync(bool enabled)
                {
                    return WriteRegisterBitAsync((byte)Adxl345Registers., (byte), enabled);
                }
                */

        public Task<bool> GetTestBitAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.DataFormat, (byte)DataFormatBits.TestBit);
        }

        public Task SetTestBitAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.DataFormat, (byte)DataFormatBits.TestBit, enabled);
        }

        public Task<bool> GetSpiBitAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.DataFormat, (byte)DataFormatBits.SpiBit);
        }

        public Task SetSpiBitAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.DataFormat, (byte)DataFormatBits.SpiBit, enabled);
        }

        #region InterruptInvertBit

        // test
        public bool InterruptInvertBit
        {
            get => GetInterruptInvertBitAsync().WaitInline();
            set => AsyncInline.AsyncInline.Run(() => SetInterruptInvertBitAsync(value));
        }

        public Task<bool> GetInterruptInvertBitAsync()
        {
            return ReadRegisterBitAsync((byte)Adxl345Registers.DataFormat, (byte)DataFormatBits.InterruptInvert);
        }

        public Task SetInterruptInvertBitAsync(bool enabled)
        {
            return WriteRegisterBitAsync((byte)Adxl345Registers.DataFormat, (byte)DataFormatBits.InterruptInvert, enabled);
        }

        #endregion
    }
}
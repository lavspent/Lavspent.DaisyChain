/*------------------------------------------------------------------------
  Adafruit Class Library for Windows Core IoT: PCA968 PWM chip.

  Written by Rick Lesniak for Adafruit Industries.

  Adafruit invests time and resources providing this open source code,
  please support Adafruit and open-source hardware by purchasing products
  from Adafruit!

  ------------------------------------------------------------------------
  This file is part of the Adafruit Windows IoT Class Library

  Adafruit Class Library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

  MIT license, all text above must be included in any redistribution.
  ------------------------------------------------------------------------*/

/******************************************************************************
   Ported to support DaisyChain by Petter Lanråten/Lavspent.no
******************************************************************************/


using Lavspent.DaisyChain.I2c;
using System;
using System.Threading.Tasks;


namespace Lavspent.DaisyChain.Adafruit
{
    public class Pca9685
    {
        #region Constants
        public const byte PCA9685_ADDRESS = 0x40;
        const byte PCA9685_MODE1 = 0x00;
        const byte PCA9685_MODE2 = 0x01;
        const byte PCA9685_SUBADR1 = 0x02;
        const byte PCA9685_SUBADR2 = 0x03;
        const byte PCA9685_SUBADR3 = 0x04;
        const byte PCA9685_PRESCALE = 0xFE;
        const byte LED0_ON_L = 0x06;
        const byte LED0_ON_H = 0x07;
        const byte LED0_OFF_L = 0x08;
        const byte LED0_OFF_H = 0x09;
        const byte ALL_LED_ON_L = 0xFA;
        const byte ALL_LED_ON_H = 0xFB;
        const byte ALL_LED_OFF_L = 0xFC;
        const byte ALL_LED_OFF_H = 0xFD;

        // Bits:
        const byte RESTART = 0x80;
        const byte SLEEP = 0x10;
        const byte ALLCALL = 0x01;
        const byte INVRT = 0x10;
        const byte OUTDRV = 0x04;

        #endregion

        private II2cDevice _i2cDeivce;


        #region Constructor
        private Pca9685(II2cDevice i2cDevice)
        {
            this._i2cDeivce = i2cDevice;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Reset
        /// Issue Reset command to PCA9685
        /// </summary>
        public async Task ResetAsync()
        {
            byte[] writeBuffer;
            // set defaults!
            // all outputs on Port A
            writeBuffer = new byte[] { PCA9685_MODE1, 0x0 };
            await WriteAsync(writeBuffer);

            await SetPWMFrequencyAsync(1000);  //default frequency
        }
        #endregion

        #region Operations
        public async Task SetPWMFrequencyAsync(double freq)
        {
            byte[] readBuffer;
            byte[] writeBuffer;

            freq *= 0.9;  // Correct for overshoot in the frequency setting

            double preScaleVal = 25000000;
            preScaleVal /= 4096;
            preScaleVal /= freq;
            preScaleVal -= 1;
            byte prescale = (byte)Math.Floor(preScaleVal + 0.5);

            //lock (Device)
            {
                writeBuffer = new byte[] { PCA9685_MODE1 };
                readBuffer = new byte[1];
                await WriteReadAsync(writeBuffer, readBuffer);
                byte oldmode = readBuffer[0];
                byte newmode = (byte)((oldmode & 0x7F) | 0x10); // sleep

                writeBuffer = new byte[] { PCA9685_MODE1, newmode };
                await WriteAsync(writeBuffer); // go to sleep

                writeBuffer = new byte[] { PCA9685_PRESCALE, prescale };
                await WriteAsync(writeBuffer); // set the prescaler

                writeBuffer = new byte[] { PCA9685_MODE1, oldmode };
                await WriteAsync(writeBuffer); // wake

                await Task.Delay(5);

                writeBuffer = new byte[] { PCA9685_MODE1, (byte)(oldmode | 0xa1) };
                await WriteAsync(writeBuffer);  // turn on auto mode
            }
        }

        /// <summary>
        /// Set PWM on a specified pin
        /// </summary>
        /// <param name="num"></param>
        /// <param name="on"></param>
        /// <param name="off"></param>
        public async Task SetPWMAsync(int num, ushort on, ushort off)
        {
            byte[] writeBuffer;
            writeBuffer = new byte[] { (byte)(LED0_ON_L + 4 * num), (byte)on, (byte)(on >> 8), (byte)off, (byte)(off >> 8) };
            await WriteAsync(writeBuffer);
        }

        /// <summary>
        /// Set PWM on all pins
        /// </summary>
        /// <param name="on"></param>
        /// <param name="off"></param>
        public async Task SetAllPWMAsync(ushort on, ushort off)
        {
            byte[] writeBuffer;
            writeBuffer = new byte[] { (byte)(ALL_LED_ON_L), (byte)on, (byte)(on >> 8), (byte)off, (byte)(off >> 8) };
            await WriteAsync(writeBuffer);
        }

        /// <summary>
        /// Sets pin without having to deal with on/off tick placement and properly handles
        /// a zero value as completely off.  Optional invert parameter supports inverting
        /// the pulse for sinking to ground.  Val should be a value from 0 to 4095 inclusive
        /// </summary>
        /// <param name="num"></param>
        /// <param name="val"></param>
        /// <param name="invert"></param>
        public async Task SetPinAsync(int num, ushort val, bool invert)
        {
            // Clamp value between 0 and 4095 inclusive.
            val = Math.Min(val, (ushort)4095);
            if (invert)
            {
                if (val == 0)
                {
                    // Special value for signal fully on.
                    await SetPWMAsync(num, 4096, 0);
                }
                else if (val == 4095)
                {
                    // Special value for signal fully off.
                    await SetPWMAsync(num, 0, 4096);
                }
                else
                {
                    await SetPWMAsync(num, 0, (ushort)(4095 - val));
                }
            }
            else
            {
                if (val == 4095)
                {
                    // Special value for signal fully on.
                    await SetPWMAsync(num, 4096, 0);
                }
                else if (val == 0)
                {
                    // Special value for signal fully off.
                    await SetPWMAsync(num, 0, 4096);
                }
                else
                {
                    await SetPWMAsync(num, 0, val);
                }
            }
        }
        #endregion

        private Task WriteAsync(byte[] buffer)
        {
            return _i2cDeivce.WriteAsync(buffer);
        }

        private Task WriteReadAsync(byte[] writeBuffer, byte[] readBuffer)
        {
            return _i2cDeivce.WriteReadAsync(writeBuffer, readBuffer);
        }

        private async static Task<Pca9685> OpenAsync(
            II2cController i2cController,
            byte address = PCA9685_ADDRESS,
            I2cBusSpeed busSpeed = I2cBusSpeed.StandardMode)
        {
            var i2cDevice = i2cController.GetDevice(new I2cConnectionSettings(address) { BusSpeed = busSpeed });
            var pca9685 = new Pca9685(i2cDevice);
            await pca9685.ResetAsync();
            return pca9685;
        }
    }
}
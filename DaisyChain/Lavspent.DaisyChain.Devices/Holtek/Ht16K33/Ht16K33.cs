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

namespace Lavspent.DaisyChain.Devices.Holtek.Ht16K33
{
    public class Ht16K33
    {
        private II2cDevice _i2cDevice;

        public static readonly byte DefaultAddress = 0x70;
        public static readonly byte DisplaySetupCmd = 0x80;
//        public static readonly byte BlinkDisplayOn = 0x01;
        public static readonly byte SystemSetup = 0x20;
        public static readonly byte Oscillator = 0x01;
        public static readonly byte DimmingCmd = 0xE0;

        public enum BlinkEnum
        {
            Off = 0x00,
            TwoHz = 0x02,
            OneHz = 0x04,
            HalfHz = 0x06
        }

        private bool _oscillatorEnabled = false;
        private byte _dimming = 0x0f;
        private byte _displaySetup = 0;
        private BlinkEnum _blink = BlinkEnum.Off;


        public Ht16K33(II2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice;
        }

        public bool OscillatorEnabled
        {
            get
            {
                return _oscillatorEnabled;
            }

            set
            {
                _i2cDevice.WriteAsync(new byte[] { (byte)(SystemSetup | (value ? 1 : 0)) }).Wait();
                _oscillatorEnabled = value;
            }
        }

        public bool DisplayEnabled
        {
            get
            {
                return (_displaySetup & 0x01) == 0x01;
            }

            set
            {
                _displaySetup = (byte) ((_displaySetup & 0x0e) | (value ? 1 : 0));
                _i2cDevice.WriteAsync(new byte[] { (byte)(DisplaySetupCmd | _displaySetup) }).Wait();
            }
        }

        public BlinkEnum Blink
        {
            get
            {
                return _blink;
            }

            set
            {
                _displaySetup = (byte)((_displaySetup & 0x01) | (byte) value);
                _i2cDevice.WriteAsync(new byte[] { (byte)(DisplaySetupCmd | _displaySetup) }).Wait();
            }
        }

        public byte Dimming
        {
            get
            {
                return _dimming;
            }

            set
            {
                value = (byte)(value & 0x0f);
                _i2cDevice.WriteAsync(new byte[] { (byte)(DimmingCmd | value) }).Wait();
                _dimming = value;
            }
        }

        //public void Init()
        //{
        //    // turn on oscillator
        //    OscillatorEnabled = true;

        //    // turn on display
        //    DisplayEnabled = true;

        //    // turn of blinking
        //    Blink = BlinkEnum.Off;

        //    // max shine
        //    Dimming = 15;
        //}
    }
}

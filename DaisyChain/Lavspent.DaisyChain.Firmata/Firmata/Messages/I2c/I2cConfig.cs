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


using System;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class I2cConfig : FirmataMessage
    {
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x78, 0xFF, true, 0, I2cConfig.Create);

        /// <summary>
        /// 
        /// </summary>
        public ushort DelayMicroseconds
        {
            get
            {
                return FirmataUtils.MidiDecodeUShort(base.Data);
            }

            set
            {
                FirmataUtils.MidiEncodeUShort(value).CopyTo(base.Data, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public I2cConfig()
            : base(MessageDescriptor, new byte[2])
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IFirmataMessage Create(FirmataMessageDescriptor commandInfo, uint command, byte[] data = null)
        {
            if (data == null || data.Length != 2)
                throw new ArgumentException("Must be two bytes long.", nameof(data));

            var i2cConfig = new I2cConfig();
            i2cConfig.DelayMicroseconds = FirmataUtils.MidiDecodeUShort(data);

            return i2cConfig;
        }
    }
}
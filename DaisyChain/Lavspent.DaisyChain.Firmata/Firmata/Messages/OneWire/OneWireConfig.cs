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
    public class OneWireConfig : FirmataMessage
    {
        /*
            0  START_SYSEX      (0xF0)
            1  OneWire Command  (0x73)
            2  config command   (0x41)
            3  pin              (0-127)
            4  power            (0x00|0x01) 0x00 = leave pin on state high after write to support
                                            parasitic power
                                            0x01 = don't leave pin on high after write
            5  END_SYSEX (0xF7)
         */

        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x7341, 0xFFFF, true, 0, OneWireConfig.Create);


        public byte Pin
        {
            get
            {
                return Data[0];
            }

            set
            {
                if (value < 0 || value > 127)
                    throw new ArgumentException("Pin must be in the range from 0 to 127.");

                Data[0] = value;
            }
        }

        public bool Power
        {
            get
            {
                return Data[1] == 0x00;
            }

            set
            {
                Data[1] = (byte)(value ? 0x00 : 0x01);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OneWireConfig()
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

            var oneWireConfig = new OneWireConfig();
            oneWireConfig.Pin = data[0];
            oneWireConfig.Power = data[1] == 0x00;

            return oneWireConfig;
        }
    }
}
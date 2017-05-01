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


using Lavspent.DaisyChain.Firmata.Messages.OneWire;
using System;
using System.Linq;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class OneWireReadReply : FirmataMessage
    {
        /*
            0  START_SYSEX          (0xF0)
            1  OneWire Command      (0x73)
            2  read reply command   (0x43)
            3  pin                  (0-127)
            4  bit 0-6   [optional] data bytes encoded using 8 times 7 bit for 7 bytes of 8 bit
            5  bit 7-13  [optional] correlationid[0] = byte[0]   + byte[1]<<7 & 0x7F
            6  bit 14-20 [optional] correlationid[1] = byte[1]>1 + byte[2]<<6 & 0x7F
            7  bit 21-27 [optional] data[0] = byte[2]>2 + byte[3]<<5 & 0x7F
            8  ....                 data[1] = byte[3]>3 + byte[4]<<4 & 0x7F
            n  ... as many bytes as needed (don't exceed MAX_DATA_BYTES though)
            n+1  END_SYSEX          (0xF7)
         */

        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x7343, 0xFFFF, true, 0, OneWireReadReply.Create);


        /// <summary>
        /// 
        /// </summary>
        public bool SearchAlarms
        {
            get
            {
                return Command == 0x7345;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public byte Pin
        {
            get
            {
                return base.Data[0];
            }

            set
            {
                if (value < 0 || value > 127)
                    throw new ArgumentException("Pin must be in the range from 0 to 127.");

                base.Data[0] = value;
            }
        }

        public byte[] Read;
        //{
        //    get
        //    {
        //    }

        //    set
        //    {
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        public OneWireReadReply()
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
            //if (data == null || data.Length >= 2)
            //    throw new ArgumentException("Must be at least two bytes long.", nameof(data));

            var oneWireReadReply = new OneWireReadReply();
            oneWireReadReply.Pin = data[0];

            var read = data.ToList().Skip(1).ToArray();
            var readDecoded = Utils.Convert7BitArrayTo8BitArray(read);

            oneWireReadReply.Read = readDecoded.Skip(2).ToArray();

            return oneWireReadReply;
        }
    }
}
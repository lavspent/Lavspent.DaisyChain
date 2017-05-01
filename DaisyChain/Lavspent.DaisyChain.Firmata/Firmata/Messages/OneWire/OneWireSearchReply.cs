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
using Lavspent.DaisyChain.OneWire;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class OneWireSearchReply : FirmataMessage
    {
        /*
            0  START_SYSEX      (0xF0)
            1  OneWire Command  (0x73)
            2  search reply command (0x42|0x45) 0x42 normal search reply
                                                0x45 reply to a SEARCH_ALARMS request
            3  pin              (0-127)
            4  bit 0-6   [optional] address bytes encoded using 8 times 7 bit for 7 bytes of 8 bit
            5  bit 7-13  [optional] 1.address[0] = byte[0]    + byte[1]<<7 & 0x7F
            6  bit 14-20 [optional] 1.address[1] = byte[1]>>1 + byte[2]<<6 & 0x7F
            7  ....                 ...
            11 bit 49-55            1.address[6] = byte[6]>>6 + byte[7]<<1 & 0x7F
            12 bit 56-63            1.address[7] = byte[8]    + byte[9]<<7 & 0x7F
            13 bit 64-69            2.address[0] = byte[9]>>1 + byte[10]<<6 &0x7F
            n  ... as many bytes as needed (don't exceed MAX_DATA_BYTES though)
            n+1  END_SYSEX      (0xF7)
         */

        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x7342, 0xFFFF, true, 0, OneWireSearchReply.Create);

        public static readonly FirmataMessageDescriptor AlarmMessageDescriptor =
            new FirmataMessageDescriptor(0x7345, 0xFFFF, true, 0, OneWireSearchReply.Create);

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
                return Data[0];
            }

            set
            {
                if (value < 0 || value > 127)
                    throw new ArgumentException("Pin must be in the range from 0 to 127.");

                Data[0] = value;
            }
        }

        // todo: fix
        private List<OneWireAddress> _addresses;
        public List<OneWireAddress> Addresses
        {
            get
            {
                return _addresses;
            }

            set
            {
                throw new NotImplementedException();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public OneWireSearchReply()
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

            var oneWireSearchReply = new OneWireSearchReply();
            oneWireSearchReply.Pin = data[0];

            var addressData = data.ToList().Skip(1).ToArray();
            oneWireSearchReply._addresses = ParseAddresses(addressData);

            return oneWireSearchReply;
        }

        private static List<OneWireAddress> ParseAddresses(byte[] data)
        {

            List<OneWireAddress> addresses = new List<OneWireAddress>();
            byte[] convertedData = Utils.Convert7BitArrayTo8BitArray(data);

            for (int i = 0; i < data.Length / 8; i++)
            {
                byte[] subset = new byte[8];
                Array.Copy(convertedData, i * 8, subset, 0, 8);
                addresses.Add(subset);
            }

            return addresses;
        }
    }
}
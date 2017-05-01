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

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class EncoderAttach : AbstractFirmataMessage
    {
        /* -----------------------------------------------------
         * 0 START_SYSEX                (0xF0)
         * 1 ENCODER_DATA               (0x61)
         * 2 ENCODER_ATTACH             (0x00)
         * 3 encoder #                  ([0 - MAX_ENCODERS-1])
         * 4 pin A #                    (first pin) 
         * 5 pin B #                    (second pin)
         * 6 END_SYSEX                  (0xF7)
         * -----------------------------------------------------
         */

        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x6100, 0xFFFF, true, 0, EncoderAttach.Create);

        public byte EncoderNumber { get; set; }
        public byte PinA { get; set; }
        public byte PinB { get; set; }


        public EncoderAttach()
            : base(MessageDescriptor)
        {
        }

        public static IFirmataMessage Create(FirmataMessageDescriptor commandInfo, uint command, byte[] data = null)
        {
            return new EncoderAttach()
            {
                EncoderNumber = data[0],
                PinA = data[1],
                PinB = data[2]
            };
        }

        public override byte[] Data
        {
            get
            {
                byte[] data = new byte[3];
                data[0] = EncoderNumber;
                data[1] = PinA;
                data[2] = PinB;
                return data;
            }

            /*set
            {
                throw new NotImplementedException();
            }*/
        }
    }
}
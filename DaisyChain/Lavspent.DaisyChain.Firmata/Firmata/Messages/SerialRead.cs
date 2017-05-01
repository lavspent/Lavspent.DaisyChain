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

/*
0  START_SYSEX        (0xF0)
1  SERIAL_DATA        (0x60)
2  SERIAL_READ        (0x30) // OR with port (0x31 = SERIAL_READ | HW_SERIAL1)
3  SERIAL_READ_MODE   (0x00) // 0x00 => read continuously, 0x01 => stop reading
4  maxBytesToRead     (lsb) [optional]
5  maxBytesToRead     (msb) [optional]
4|6 END_SYSEX         (0xF7)

*/


using System;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class SerialRead : AbstractFirmataMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x6030, 0xFFF0, true, 0, SerialRead.Create);

        /// <summary>
        /// 
        /// </summary>
        public SerialRead()
            : base(MessageDescriptor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override byte[] Data
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IFirmataMessage Create(FirmataMessageDescriptor commandInfo, uint command, byte[] data = null)
        {
            var serialRead = new SerialRead();
            return serialRead;
        }
    }
}
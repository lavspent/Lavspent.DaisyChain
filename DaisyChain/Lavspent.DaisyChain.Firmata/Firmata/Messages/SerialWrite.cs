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
0  START_SYSEX      (0xF0)
1  SERIAL_DATA      (0x60)
2  SERIAL_WRITE     (0x20) // OR with port (0x21 = SERIAL_WRITE | HW_SERIAL1)
3  data 0           (LSB)
4  data 0           (MSB)
5  data 1           (LSB)
6  data 1           (MSB)
...                 // up to max buffer - 5
n  END_SYSEX        (0xF7)
*/



namespace Lavspent.DaisyChain.Firmata.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class SerialWrite : AbstractFirmataMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x6020, 0xFFF0, true, 0, SerialWrite.Create);

        /// <summary>
        /// 
        /// </summary>
        public SerialPortEnum Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override uint Command
        {
            get
            {
                return base.Command | (byte)Port;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SerialWrite()
            : base(MessageDescriptor)
        {
        }

        private byte[] _data;

        /// <summary>
        /// 
        /// </summary>
        public override byte[] Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value ?? new byte[0];
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
            var serialWrite = new SerialWrite();
            serialWrite.Port = (SerialPortEnum)(commandInfo.Command & 0xf);
            serialWrite.Data = data;
            return serialWrite;
        }
    }
}
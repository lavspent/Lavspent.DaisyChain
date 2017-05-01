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


using System.Diagnostics;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class AnalogMessage : AbstractFirmataMessage
    {
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0xE0, 0xF0, false, 2, AnalogMessage.Create);

        public byte Pin
        {
            get;
            set;
        }

        public ushort Value
        {
            get;
            set;
        }

        public AnalogMessage()
            : base(MessageDescriptor)
        {
        }

        public static IFirmataMessage Create(FirmataMessageDescriptor commandInfo, uint command, byte[] data = null)
        {
            Debug.WriteLine($"{command}");
            return new AnalogMessage()
            {
                Pin = (byte)(command & 0x0f),
                Value = FirmataUtils.MidiDecodeUShort(data)
            };
        }
    }
}

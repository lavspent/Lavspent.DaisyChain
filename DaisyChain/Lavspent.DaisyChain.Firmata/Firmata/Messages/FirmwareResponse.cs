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
    public class FirmwareResponse : AbstractFirmataMessage
    {
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x79, 0xFF, true, 0, FirmwareResponse.Create);


        /// <summary>
        /// 
        /// </summary>
        public byte MajorVersion
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte MinorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Firmware
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public FirmwareResponse()
            : base(MessageDescriptor)
        {
        }

        public override byte[] Data
        {
            get
            {
                byte[] firmwareData = FirmataUtils.Encode7BitString(Firmware);
                byte[] data = new byte[2 + firmwareData.Length];
                data[0] = MajorVersion;
                data[1] = MinorVersion;
                Array.Copy(firmwareData, 0, data, 2, firmwareData.Length);

                return data;
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
            if (data == null || data.Length < 2 || data.Length % 2 != 0)
                throw new ArgumentException("Invalid data format.");

            FirmwareResponse firmwareResponse = new FirmwareResponse()
            {
                MajorVersion = data[0],
                MinorVersion = data[1],
                Firmware = FirmataUtils.Decode7BitString(data, 2, data.Length - 2)
            };

            return firmwareResponse;
        }
    }
}

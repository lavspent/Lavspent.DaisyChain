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
    /// <summary>
    /// 
    /// </summary>
    public class ProtocolVersionResponse : FirmataMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0xF9, 0xFF, false, 2, ProtocolVersionResponse.Create);

        /// <summary>
        /// 
        /// </summary>
        public byte MajorVersion
        {
            get
            {
                return Data[0];
            }
            set
            {
                Data[0] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte MinorVersion
        {
            get
            {
                return Data[1];
            }
            set
            {
                Data[1] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProtocolVersionResponse()
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
            var protocolVersionResponse = new ProtocolVersionResponse()
            {
                MajorVersion = data[0],
                MinorVersion = data[1]
            };

            return protocolVersionResponse;
        }
    }
}

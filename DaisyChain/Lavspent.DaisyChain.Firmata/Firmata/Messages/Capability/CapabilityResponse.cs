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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class CapabilityResponse : AbstractFirmataMessage
    {
        public static readonly FirmataMessageDescriptor MessageDescriptor =
            new FirmataMessageDescriptor(0x6C, 0xFF, true, 0, CapabilityResponse.Create);

        /// <summary>
        /// 
        /// </summary>
        public PinCapabilities PinCapabilities
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public CapabilityResponse()
            : base(MessageDescriptor)
        {
            PinCapabilities = new PinCapabilities();
        }

        /// <summary>
        /// 
        /// </summary>
        public override byte[] Data
        {
            get
            {
                if (PinCapabilities.Count == 0)
                    return new byte[0]; // no capability

                // get highest pin number
                int maxPinNumber = PinCapabilities.Keys.ToList().Max();

                MemoryStream memoryStream = new MemoryStream(1024);


                int pinNumber;
                for (pinNumber = 1; pinNumber <= maxPinNumber; pinNumber++)
                {
                    if (PinCapabilities.ContainsKey(pinNumber))
                    {
                        List<PinCapability> capabilityList = PinCapabilities[pinNumber];
                        foreach (var pinCapability in capabilityList)
                        {
                            memoryStream.WriteByte((byte)pinCapability.Mode);
                            memoryStream.WriteByte((byte)pinCapability.Resolution);
                        }
                    }
                    // todo: move this const somewhere
                    memoryStream.WriteByte(0x7f);
                }

                return memoryStream.ToArray();
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
            var capabilityResponse = new CapabilityResponse();

            List<PinCapability> pinCapabilityList = new List<PinCapability>();

            byte pinNumber = 1;
            int index = 0;

            while (index < data.Length)
            {
                // todo: fix this magic number
                if (data[index] == 0x7f)
                {
                    capabilityResponse.PinCapabilities.Add(pinNumber, pinCapabilityList);
                    pinCapabilityList = new List<PinCapability>();
                    pinNumber++;
                    index++;
                }
                else
                {
                    PinCapability pinCapability = new PinCapability();
                    pinCapability.Pin = pinNumber;
                    pinCapability.Mode = (PinMode)data[index];
                    pinCapability.Resolution = data[index + 1];
                    pinCapabilityList.Add(pinCapability);
                    index += 2;
                }
            }

            return capabilityResponse;
        }
    }
}

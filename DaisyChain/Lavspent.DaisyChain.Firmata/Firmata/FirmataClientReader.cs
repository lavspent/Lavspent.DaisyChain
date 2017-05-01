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

using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.Stream;

namespace Lavspent.DaisyChain.Firmata
{
    /// <summary>
    /// Implementation of the FirmataReader that reads client side response
    /// type messages. 
    /// </summary>
    public class FirmataClientReader : FirmataReader
    {
        public FirmataClientReader(IStream stream)
                : base(stream)
        {
            // Register the commands that the client reader understands
            AddMessageDescriptors(
                DigitalMessage.MessageDescriptor,
                AnalogMessage.MessageDescriptor,
                ProtocolVersionResponse.MessageDescriptor,

                // Sysex commands
                EncoderData.MessageDescriptor,
                AnalogMappingResponse.MessageDescriptor,
                CapabilityResponse.MessageDescriptor,
                PinStateResponse.MessageDescriptor,
                ExtendedAnalog.MessageDescriptor,
                ServoConfig.MessageDescriptor,
                StringData.MessageDescriptor,
                StepperData.MessageDescriptor,
                OneWireSearchReply.MessageDescriptor,
                OneWireSearchReply.AlarmMessageDescriptor,
                OneWireReadReply.MessageDescriptor,

                ShiftData.MessageDescriptor,
                I2cReply.MessageDescriptor,
                FirmwareResponse.MessageDescriptor,
                SchedulerData.MessageDescriptor,
                SysexNonRealtime.MessageDescriptor,
                SysexRealtime.MessageDescriptor);
        }
    }
}

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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    /// <summary>
    /// Writes Firmata commands to a stream.
    /// </summary>
    public class FirmataWriter : IFirmataWriter
    {
        private IStream _stream;

        public FirmataWriter(IStream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private byte[] BuildMessage(IFirmataMessage message)
        {
            using (MemoryStream memoryStream = new MemoryStream(1024))
            {
                // Add StartSysex if necessary
                if (message.IsSysex)
                    memoryStream.WriteByte(FirmataConstants.StartSysex);

                // Add command
                byte[] commandBytes = FirmataUtils.CommendToBytes(message.Command);
                memoryStream.Write(commandBytes, 0, commandBytes.Length);

                // add message data if any
                byte[] messageData = message.Data;
                if (messageData.Length > 0)
                {
                    memoryStream.Write(messageData, 0, messageData.Length);
                }

                // Add EndSysex if necessary
                if (message.IsSysex)
                    memoryStream.WriteByte(FirmataConstants.EndSysex);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task WriteAsync(IFirmataMessage message, CancellationToken cancellationToken)
        {
            byte[] buffer = BuildMessage(message);

            // Write message
            return _stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}

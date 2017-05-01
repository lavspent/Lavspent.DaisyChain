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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    /// <summary>
    ///  Reads Firmata commands from a stream.
    /// </summary>
    public class FirmataReader : IFirmataReader, IDisposable
    {
        private IStream _stream;
        private byte[] _buffer = new byte[1024];
        private MemoryStream _backLog = new MemoryStream(1024);
        private MemoryStream _currentMessageData = new MemoryStream(1024);

        protected Dictionary<uint, FirmataMessageDescriptor> _messageDescriptors = new Dictionary<uint, FirmataMessageDescriptor>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public FirmataReader(IStream stream)
        {
            _stream = stream;
            ReadTimeout = TimeSpan.FromSeconds(10);
        }

        protected void AddMessageDescriptor(FirmataMessageDescriptor messageDescriptor)
        {
            _messageDescriptors.Add(messageDescriptor.Command, messageDescriptor);
        }

        protected void AddMessageDescriptors(params FirmataMessageDescriptor[] messageDescriptors)
        {
            foreach (var messageDescriptor in messageDescriptors)
                AddMessageDescriptor(messageDescriptor);
        }


        public TimeSpan ReadTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IFirmataMessage> ReadAsync(CancellationToken cancellationToken)
        {
            // todo: not thread safe!
            _currentMessageData.SetLength(0);

            int read;
            uint command;

            // read command byte
            read = await ReadInternalAsync(cancellationToken).ConfigureAwait(false);
            if (read == -1)
                return null;  // end of stream

            bool isSysex = read == FirmataConstants.StartSysex;

            // if sysex then read next byte as command byte
            if (isSysex)
            {
                // 
                read = await ReadInternalAsync(cancellationToken).ConfigureAwait(false);
                if (read == -1)
                    return null;  // end of stream
            }

            //
            command = (byte)read;

            Debug.WriteLine("command " + command);

            // get until we got a complete "command" header
            FirmataMessageDescriptor messageDescriptor = null; //
            bool commandTrunked = false;

            while (!GetMessageDescriptor(command, out messageDescriptor, out commandTrunked) && commandTrunked)
            {
                read = await ReadInternalAsync(cancellationToken).ConfigureAwait(false);
                if (read == -1)
                    return null;  // end of stream
                command = command << 8 | (byte)read;
            }

            if (messageDescriptor == null)
                throw new Exception($"Invalid command byte ({command}).");

            // read until all bytes read or sysexend
            while (isSysex || _currentMessageData.Length < messageDescriptor.Length)
            {
                read = await ReadInternalAsync(cancellationToken).ConfigureAwait(false);
                if (read == -1)
                    return null; // end of stream

                if (isSysex && read == FirmataConstants.EndSysex)
                    break;

                _currentMessageData.WriteByte((byte)read);
            }

            byte[] messageData = _currentMessageData.ToArray();

            IFirmataMessage firmataMessage = CreateFirmataMessage(messageDescriptor, (byte)command, messageData);
            return firmataMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="messageDescriptor"></param>
        /// <param name="commandTrunked"></param>
        /// <returns></returns>
        protected bool GetMessageDescriptor(uint command, out FirmataMessageDescriptor messageDescriptor, out bool commandTrunked)
        {
            messageDescriptor = null;
            commandTrunked = false;

            var matchingDescriptors =
                _messageDescriptors.Values.Where(ci => ci.IsPartialMatch(command));

            if (matchingDescriptors.Count() == 1)
                messageDescriptor = matchingDescriptors.Single();
            //else
            //    messageDescriptor = null;

            commandTrunked = matchingDescriptors.Count() > 1;  // need more command bytes
            return messageDescriptor != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageDescriptor"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual IFirmataMessage CreateFirmataMessage(FirmataMessageDescriptor messageDescriptor, byte command, byte[] data)
        {
            if (messageDescriptor?.Factory == null)
                throw new ArgumentNullException("messageDescscriptor.Factory is null.");

            return messageDescriptor.Factory(messageDescriptor, command, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        private async Task<int> ReadInternalAsync(CancellationToken cancellationToken)
        {
            if (_backLog.Position == _backLog.Length)
            {
                int read = await _stream.ReadAsync(_buffer, 0, 1024, cancellationToken).ConfigureAwait(false);

                if (read == 0)
                    return -1;

                _backLog.SetLength(0);
                _backLog.Write(_buffer, 0, read);
                _backLog.Position = 0;
            }

            return _backLog.ReadByte();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _currentMessageData.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

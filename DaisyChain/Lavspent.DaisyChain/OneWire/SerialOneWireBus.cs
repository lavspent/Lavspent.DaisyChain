///*
//    Copyright(c) 2017 Petter Labråten/LAVSPENT.NO. All rights reserved.

//    The MIT License(MIT)

//    Permission is hereby granted, free of charge, to any person obtaining a
//    copy of this software and associated documentation files (the "Software"),
//    to deal in the Software without restriction, including without limitation
//    the rights to use, copy, modify, merge, publish, distribute, sublicense,
//    and/or sell copies of the Software, and to permit persons to whom the
//    Software is furnished to do so, subject to the following conditions:

//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.

//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//    DEALINGS IN THE SOFTWARE.
//*/

//using Lavspent.DaisyChain.Serial;
//using Lavspent.DaisyChain.Stream;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Lavspent.DaisyChain.OneWire
//{
//    /// <summary>
//    /// Provides a software based 1-Wire bus using a serial device such as an UART.
//    /// </summary>
//    public class SerialOneWireBus : IOneWireBus, IDisposable
//    {
//        private string _portName;
//        private ISerialController _serialController;
//        private ISerial _serial;
//        private IStream _stream;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="deviceId"></param>
//        public SerialOneWireBus(ISerialController serialController, string portName)
//        {
//            _serialController = serialController;
//            _portName = portName;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="baudRate"></param>
//        /// <returns></returns>
//        private async Task ResetSerialDevice(int baudRate)
//        {
//            // todo: cancellationtoken

//            if (_serial != null)
//            {
//                //await _serial.CloseAsync().ConfigureAwait(false);
//                _serial.Dispose();
//            }

//            _serial = await _serialController.OpenSerialAsync(
//                _portName,
//                baudRate,
//                Parity.None,
//                8,
//                StopBits.One
//                ).ConfigureAwait(false);

//            /*
//            _serialDevice.Handshake = SerialHandshake.None;
//            */

//            //await _serial.OpenAsync();

//            _stream = _serial.GetStream();
//            _stream.WriteTimeout = TimeSpan.FromMilliseconds(1000);
//            _stream.ReadTimeout = TimeSpan.FromMilliseconds(1000);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public async Task ResetAsync(CancellationToken cancellationToken)
//        {

//            // reset 
//            await ResetSerialDevice(9600).ConfigureAwait(false);
//            await _stream.WriteAsync(0xF0, cancellationToken).ConfigureAwait(false); /* todo: magic number */
//            await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

//            // get response
//            //await _dataReader.LoadAsync(1);
//            //byte[] buf = new byte[1];
//            int responseInt = await _stream.ReadAsync(cancellationToken).ConfigureAwait(false);

//            /*            if (responseInt == 0)
//                        {
//                            // no data, timeout?
//                            throw new Exception("Uart to OneWire not properly connected? Is there as fast switching diode between Tx and Rx?");
//                        }
//                        */

//            byte response = (byte)responseInt;

//            if (response == 0xFF) /* todo: magic number */
//            {
//                // no device connected to uart
//                throw new Exception("Serial device not found.");
//            }
//            else if (response == 0xF0) /* todo: magic number */
//            {
//                // no 1-wire device connected
//                throw new Exception("1-Wire device not found.");
//            }
//            else
//            {
//                // 1-wire device present, switch to data speed
//                await ResetSerialDevice(115200);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void Dispose()
//        {
//            _serial?.Dispose();
//            _serial = null;
//        }

//        public Task<IReadOnlyList<OneWireAddress>> GetDeviceAddressesAsync(CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task MatchRomAsync(byte[] rom, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public async Task<byte> ReadAsync(CancellationToken cancellationToken)
//        {
//            byte[] buffer = new byte[1];
//            await ReadAsync(buffer, 1, cancellationToken);
//            return buffer[0];
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="buffer"></param>
//        /// <param name="length"></param>
//        /// <returns></returns>
//        public async Task ReadAsync(byte[] buffer, int length, CancellationToken cancellationToken)
//        {
//            for (int i = 0; i < length; i++)
//            {
//                byte byteValue = 0;
//                for (int j = 0; j < 8; j++)
//                {
//                    byte bitValue = await ReadBitAsync(cancellationToken);
//                    byteValue |= (byte)(bitValue << j);
//                }

//                buffer[i] = byteValue;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public Task WriteAsync(byte[] data, CancellationToken cancellationToken)
//        {
//            return Task.Run(() =>
//            {
//                // loop through bytes
//                for (int i = 0; i < data.Length; i++)
//                {
//                    // todo: what overhead will this cause?
//                    cancellationToken.ThrowIfCancellationRequested();

//                    byte byteValue = data[i];

//                    // loop through bits
//                    for (int j = 0; j < 8; j++)
//                    {
//                        byte bitValue = (byte)((byteValue >> j) & 0x01);
//                        WriteBit(bitValue);
//                    }
//                }
//            }, cancellationToken);

//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public void WriteBit(byte data)
//        {
//            WriteReadBit(data);
//        }

//        public Task WriteBitAsync(byte data, CancellationToken cancellationToken)
//        {
//            WriteBit(data);
//            return Task.FromResult<object>(null); // Run(() => { }); //
//        }

//        private byte WriteReadBit(byte bit)
//        {
//            bit = bit == 0 ? (byte)0x00 : (byte)0xff;
//            _stream.WriteAsync(bit).Wait();
//            _stream.FlushAsync().Wait();

//            int responseInt = _stream.ReadAsync().Result;
//            if (responseInt == -1)
//                throw new Exception("End of stream encountered.");

//            byte response = (byte)responseInt;

//            return response == 0xFF ? (byte)1 : (byte)0;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public Task<byte> ReadBitAsync(CancellationToken cancellationToken)
//        {
//            return Task.FromResult(WriteReadBit(1));
//        }

//        public Task SkipRomAsync(CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

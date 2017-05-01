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


using Lavspent.DaisyChain.OneWire;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lavspent.DaisyChain.Firmata.Messages
{
    public class OneWireCommand : AbstractFirmataMessage
    {
        /*
            0  START_SYSEX      (0xF0)
            1  OneWire Command  (0x73)
            2  command bits     (0x00-0x2F) bit 0 = reset, bit 1 = skip, bit 2 = select,
                                            bit 3 = read, bit 4 = delay, bit 5 = write
            3  pin              (0-127)
            4  bit 0-6   [optional] data bytes encoded using 8 times 7 bit for 7 bytes of 8 bit
            5  bit 7-13  [optional] data[0] = byte[0]   + byte[1]<<7 & 0x7F
            6  bit 14-20 [optional] data[1] = byte[1]>1 + byte[2]<<6 & 0x7F
            7  ....                 data[2] = byte = byte[2]>2 + byte[3]<<5 & 0x7F ...
            n  ... as many bytes as needed (don't exceed MAX_DATA_BYTES though)
            n+1  END_SYSEX      (0xF7)
            
            // data bytes within OneWire Request Command message
            0  address[0]                    [optional, if bit 2 set]
            1  address[1]                              "
            2  address[2]                              "
            3  address[3]                              "
            4  address[4]                              "
            5  address[5]                              "
            6  address[6]                              "
            7  address[7]                              "
            8  number of bytes to read (LSB) [optional, if bit 3 set]
            9  number of bytes to read (MSB)           "
            10 request correlationid byte 0            "
            11 request correlationid byte 1            "
            10 delay in ms      (bits 0-7)   [optional, if bit 4 set]
            11 delay in ms      (bits 8-15)            "
            12 delay in ms      (bits 16-23)           "
            13 delay in ms      (bits 24-31)           "
            14 data to write    (bits 0-7)   [optional, if bit 5 set]
            15 data to write    (bits 8-15)            "
            16 data to write    (bits 16-23)           "
            n  ... as many bytes as needed (don't exceed MAX_DATA_BYTES though)        
        */

        public static readonly FirmataMessageDescriptor MessageDescriptor =
                  new FirmataMessageDescriptor(0x73, 0xFF, true, 0, OneWireCommand.Create);

        [Flags]
        public enum CommandFlags : byte
        {
            Reset = 1 << 0,
            Skip = 1 << 1,
            Select = 1 << 2,
            Read = 1 << 3,
            Delay = 1 << 4,
            Write = 1 << 5
        };

        private CommandFlags _commands;

        public CommandFlags Commands
        {
            get
            {
                return _commands;
            }

            set
            {
                _commands = value;
            }
        }

        private byte _pin;

        public byte Pin
        {
            get
            {
                return _pin;
            }

            set
            {
                // todo: range check
                _pin = value;
            }
        }


        private OneWireAddress _address;

        public OneWireAddress Address
        {
            get
            {
                return _address;
            }

            set
            {
                // todo: why this immutable thingy? should be a struct?
                _address = new OneWireAddress(value);
            }
        }

        private UInt16 _byteToRead;

        public UInt16 BytesToRead
        {
            get
            {
                return _byteToRead;
            }

            set
            {
                _byteToRead = value;
            }
        }

        private UInt16 _correlationId;

        public UInt16 CorrelationId
        {
            get
            {
                return _correlationId;
            }

            set
            {
                _correlationId = value;
            }
        }

        private UInt32 _delay;

        public UInt32 Delay
        {
            get
            {
                return _delay;
            }

            set
            {
                _delay = value;
            }
        }

        private byte[] _dataToWrite;

        public byte[] DataToWrite
        {
            get
            {
                return _dataToWrite;
            }

            set
            {
                _dataToWrite = value;
            }
        }

        public override byte[] Data
        {


            get
            {
                MemoryStream ms = new MemoryStream();
                MemoryStream data = new MemoryStream();

                // command bits and pin
                ms.Write(new byte[] { (byte)Commands, (byte)Pin }, 0, 2);


                // select 
                if (Commands.HasFlag(CommandFlags.Select))
                {
                    data.Write(this.Address.Raw, 0, this.Address.Raw.Length);
                    data.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0 }, 0, 6);
                }

                // read
                if (Commands.HasFlag(CommandFlags.Read))
                {
                    byte lsb = (byte)(BytesToRead & 0xff);
                    byte msb = (byte)(BytesToRead >> 8 & 0xff);
                    data.Write(new byte[] { lsb, msb, 0, 5 }, 0, 4);
                }

                // correlation id

                // write
                if (Commands.HasFlag(CommandFlags.Write))
                {
                    data.Write(DataToWrite, 0, DataToWrite.Length);
                }


                data.Position = 0; // needed?
                var converted = Convert8BitArrayTo7BitArray(data.ToArray());
                ms.Write(converted, 0, converted.Length);

                //
                ms.Position = 0; // needed?
                var bytes = ms.ToArray();
                return bytes;
            }

            set => base.Data = value;
        }


        // TODO: Clean up! And Move!
        public static byte[] Convert8BitArrayTo7BitArray(byte[] source)
        {
            List<byte> resultBytes = new List<byte>();
            int sourceIndex = 0;

            //int shift = 0;
            byte remain = 0;
            for (sourceIndex = 0; sourceIndex < source.Length; sourceIndex++)
            {
                int shift = sourceIndex % 8;
                byte sourceByte = source[sourceIndex];

                byte low = (byte)((sourceByte << shift & 0x7f));
                byte high = (byte)((sourceByte >> (7 - shift)));

                byte v = (byte)(low | remain);
                resultBytes.Add(v);

                if (sourceIndex == source.Length - 1)
                {
                    resultBytes.Add(high);

                    //resultBytes.Add(0);
                    break;
                }

                remain = high;
            }

            return resultBytes.ToArray();

        }

        /// <summary>
        /// 
        /// </summary>
        public OneWireCommand()
            : base(MessageDescriptor)
        {
            //SearchAlarms = searchAlarms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IFirmataMessage Create(FirmataMessageDescriptor commandInfo, uint command, byte[] data = null)
        {
            //if (data == null || data.Length != 2)
            //    throw new ArgumentException("Must be two bytes long.", nameof(data));

            //var oneWireSearch = new OneWireSearch();
            //oneWireSearch.SearchAlarms = data[0] == 0x44;
            //oneWireSearch.Pin = data[1];

            //return oneWireSearch;
            return null;
        }
    }
}
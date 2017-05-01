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

//using Lavspent.DaisyChain.Gpio;
//using System;
//using System.Threading.Tasks;

//namespace Lavspent.DaisyChain.Spi
//{
//    internal class SoftSpiChannel : ISpiChannel
//    {
//        private SoftSpiController _softSpiController;

//        public SoftSpiChannel(SoftSpiController softSpiController)
//        {
//            this._softSpiController = softSpiController;
//        }

//        public async Task ReadAsync(byte[] buffer)
//        {
//            // set chipselect high
//            await _softSpiController._chipSelect.WriteAsync(GpioValue.High);

//            // todo: 
//        }

//        public Task TransferFullDuplexAsync(byte[] writeBuffer, byte[] readBuffer)
//        {
//            throw new NotImplementedException();
//        }

//        public Task TransferSequentialAsync(byte[] writeBuffer, byte[] readBuffer)
//        {
//            throw new NotImplementedException();
//        }

//        public Task WriteAsync(byte[] buffer)
//        {
//            throw new NotImplementedException();
//        }


//        /*
//         * Simultaneously transmit and receive a byte on the SPI.
//         *
//         * Polarity and phase are assumed to be both 0, i.e.:
//         *   - input data is captured on rising edge of SCLK.
//         *   - output data is propagated on falling edge of SCLK.
//         *
//         * Returns the received byte.
//         */

//        private async byte[] ReadWriteBits(byte[] bitsOut, int offset, int count)
//        {
//            int byteIndex = offset / 8;
//            byte bitMask = (byte) (0x80 >> (offset % 8));

//            byte bits

//            byte byteIn = 0;
//            byte bit;

//            for (bit = 0x80; bit != 0; bit >>= 1)
//            {
//                _softSpiController._mosi.WriteAsync((byteOut & bit) == 1 ? GpioValue.High : GpioValue.Low);

//                /* Delay for at least the peer's setup time */
//                delay(SPI_SCLK_LOW_TIME);

//                /* Pull the clock line high */
//                write_SCLK(HIGH);

//                /* Shift-in a bit from the MISO line */
//                if (read_MISO() == HIGH)
//                    byte_in |= bit;

//                /* Delay for at least the peer's hold time */
//                delay(SPI_SCLK_HIGH_TIME);

//                /* Pull the clock line low */
//                write_SCLK(LOW);
//            }

//            return byte_in;
//        }



//        public void Dispose()
//        {
//            _softSpiController._softSpiChannel = null;
//        }



//    }
//}

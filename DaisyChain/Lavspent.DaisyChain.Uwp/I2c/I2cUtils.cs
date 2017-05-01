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

using Windows.Devices.I2c.Provider;

namespace Lavspent.DaisyChain.I2c
{
    internal class I2cUtils
    {
        public static I2cTransferResult I2cTransferResultFromProviderI2cTransferResult(ProviderI2cTransferResult providerI2cTransferResult)
        {
            I2cTransferResult i2cTransferResult = new I2cTransferResult();
            i2cTransferResult.BytesTransferred = providerI2cTransferResult.BytesTransferred;
            i2cTransferResult.Status = (I2cTransferStatus)providerI2cTransferResult.Status;
            return i2cTransferResult;
        }

        public static ProviderI2cTransferResult ProviderI2cTransferResultFromI2cTransferResult(I2cTransferResult i2cTransferResult)
        {
            ProviderI2cTransferResult provideri2cTransferResult = new ProviderI2cTransferResult();
            provideri2cTransferResult.BytesTransferred = i2cTransferResult.BytesTransferred;
            provideri2cTransferResult.Status = (ProviderI2cTransferStatus)i2cTransferResult.Status;
            return provideri2cTransferResult;
        }
    }
}

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
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Lavspent.DaisyChain.I2c
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class UwpII2cExtensions
    {
        /// <summary>
        /// Convert this DaisyChain I2cControllerProvider to a UWP I2cControllerProvider.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Windows.Devices.I2c.Provider.II2cControllerProvider AsUwpI2cControllerProvider(this DaisyChain.I2c.II2cController _this)
        {
            return new UwpII2cControllerProvider(_this);
        }

        /// <summary>
        /// Convert this UWP I2cDevice to a DaisyChain I2cDevice.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static DaisyChain.I2c.II2cDevice AsDaisyChainI2cDevice(this Windows.Devices.I2c.I2cDevice _this)
        {
            return new UwpI2cDeviceWrapper(_this);
        }

        /// <summary>
        /// Convert this UWP I2cDevice to a DaisyChain I2cDevice.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Task<II2cDevice> AsDaisyChainI2cDeviceAsync(this IAsyncOperation<Windows.Devices.I2c.I2cDevice> _this)
        {
            return _this.AsTask().AsDaisyChainI2cDeviceAsync();
        }

        /// <summary>
        /// Convert this UWP I2cDevice to a DaisyChain I2cDevice.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async static Task<II2cDevice> AsDaisyChainI2cDeviceAsync(this Task<Windows.Devices.I2c.I2cDevice> _this)
        {
            return (await _this.ConfigureAwait(false)).AsDaisyChainI2cDevice();
        }

        /// <summary>
        /// Convert this UWP I2cController to a DaisyChain I2cController.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static DaisyChain.I2c.II2cController AsDaisyChainI2cController(this Windows.Devices.I2c.I2cController _this)
        {
            return new UwpI2cControllerWrapper(_this);
        }


        /// <summary>
        /// Convert this UWP I2cController to a DaisyChain I2cController.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Task<DaisyChain.I2c.II2cController> AsDaisyChainI2cControllerAsync(this IAsyncOperation<Windows.Devices.I2c.I2cController> _this)
        {
            return _this.AsTask().AsDaisyChainI2cControllerAsync();
        }

        /// <summary>
        /// Convert this UWP I2cController to a DaisyChain I2cController.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static async Task<DaisyChain.I2c.II2cController> AsDaisyChainI2cControllerAsync(this Task<Windows.Devices.I2c.I2cController> _this)
        {
            return (await _this.ConfigureAwait(false)).AsDaisyChainI2cController();
        }
    }
}

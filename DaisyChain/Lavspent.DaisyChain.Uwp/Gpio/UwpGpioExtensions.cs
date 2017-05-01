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

using System.ComponentModel;
using Windows.Devices.Gpio.Provider;

namespace Lavspent.DaisyChain.Gpio
{
    // TODO: Give a better name?
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class UwpGpioExtensions
    {
        ///// <summary>
        ///// Wraps a UWP GpioPin as a DaisyChain IGpio
        ///// </summary>
        ///// <param name="_this"></param>
        ///// <returns></returns>
        //public static IGpio AsDaisyChainGpio(this Windows.Devices.Gpio.GpioPin _this)
        //{
        //    return new UwpGpioPinWrapper(_this);
        //}

        ///// <summary>
        ///// Wraps a DaisyChain IGpio as a UWP GpioPin
        ///// </summary>
        ///// <param name="gpioPin"></param>
        ///// <returns></returns>
        //public static async Task<Windows.Devices.Gpio.GpioPin> AsUwpGpioPinAsync(this IGpio gpioPin)
        //{
        //    // Create GpioController that controls this pin
        //    var gpioController = new GpioController(gpioPin);

        //    //
        //    var uwpGpioControllerProvider = new DaisyChainGpioControllerProviderWrapper(gpioController);
        //    var uwpGpioProvider = new UwpGpioProvider(uwpGpioControllerProvider);
        //    var uwpGpioControllers = await Windows.Devices.Gpio.GpioController.GetControllersAsync(uwpGpioProvider);

        //    return uwpGpioControllers[0].OpenPin(0);
        //}

        ///// <summary>
        ///// Returns as UWP IGpioProvider that provides this DaisyChain GpioController.
        ///// </summary>
        ///// <param name="_this"></param>
        ///// <returns></returns>
        //public static IGpioProvider AsUwpGpioProvider(this IGpioController _this)
        //{
        //    return new UwpGpioProvider(_this.AsUwpGpioControllerProvider());
        //}

        /// <summary>
        /// Returns this DaisyChain IGpioController as a UWN IGpioControllerProvider.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static IGpioControllerProvider AsUwpGpioControllerProvider(this IGpioController _this)
        {
            return new UwpGpioControllerProvider(_this);
        }

        /// <summary>
        /// Returns this UWP GpioController as a DaisyChain IGpioController.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static IGpioController AsDaisyChainGpioController(this Windows.Devices.Gpio.GpioController _this)
        {
            return new UwpGpioControllerWrapper(_this);
        }
    }
}

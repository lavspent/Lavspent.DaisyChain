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
using System.Collections.Generic;

namespace Lavspent.DaisyChain.I2c
{
    /// <summary>
    /// 
    /// </summary>
    public class I2cControllerManager
    {
        private static I2cControllerManager _instance = new I2cControllerManager();

        private Dictionary<string, WeakReference<II2cController>> _i2cControllers;

        /// <summary>
        /// 
        /// </summary>
        public I2cControllerManager()
        {
            _i2cControllers = new Dictionary<string, WeakReference<II2cController>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="i2cController"></param>
        public void Register(string deviceId, II2cController i2cController)
        {
            lock (_i2cControllers)
            {
                _i2cControllers.Add(deviceId, new WeakReference<II2cController>(i2cController));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        public void Unregister(string deviceId)
        {
            lock (_i2cControllers)
            {
                _i2cControllers.Remove(deviceId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public II2cController GetI2cController(string deviceId)
        {
            II2cController i2cController = null;

            lock (_i2cControllers)
            {
                if (!_i2cControllers.ContainsKey(deviceId))
                    return null;

                if (!_i2cControllers[deviceId].TryGetTarget(out i2cController))
                {
                    // remove from dictionary
                    _i2cControllers.Remove(deviceId);
                }

                return i2cController;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static I2cControllerManager Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}

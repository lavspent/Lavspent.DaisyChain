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

using Lavspent.DaisyChain.Gpio;

namespace Lavspent.DaisyChain.Devices.Microchip.Mcp23017
{
    /// <summary>
    /// 
    /// </summary>
    public class Mcp23017Port   // One of the who 8-bit port on the Gpio expander. 
                                //This might be reused, share interface with the Mcp23008?
    {
        private Mcp23017 _mcp23017;
        private Mcp23017PortEnum _port;
        private Mcp23017ControlRegisters _registers;
        private IGpioController _gpioController;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="mcp23017"></param>
        public Mcp23017Port(Mcp23017PortEnum port, Mcp23017 mcp23017)
        {
            _mcp23017 = mcp23017;
            _port = port;
            _registers = new Mcp23017ControlRegisters(_port, _mcp23017);
            _gpioController = new Mcp23017GpioController(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public byte IoDirectionARegister
        {
            get
            {
                return _mcp23017.ReadRegister(_registers.IoDir);
            }

            set
            {
                _mcp23017.WriteRegister(_registers.IoDir, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte InputPolarityRegister
        {
            get
            {
                return _mcp23017.ReadRegister(_registers.IPol);
            }

            set
            {
                _mcp23017.WriteRegister(_registers.IPol, value);
            }
        }


        //private byte _oLat;
        /// <summary>
        /// 
        /// </summary>
        public byte OLat
        {
            get
            {
                return _mcp23017.ReadRegister(_registers.OLat);
                //return _oLat;
            }

            set
            {
                //_oLat = value;
                _mcp23017.WriteRegister(_registers.OLat, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte Gpio
        {
            get
            {
                return _mcp23017.ReadRegister(_registers.Gpio);
            }

            set
            {
                _mcp23017.WriteRegister(_registers.Gpio, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IGpioController GetGpioController()
        {
            return _gpioController;
        }
    }
}
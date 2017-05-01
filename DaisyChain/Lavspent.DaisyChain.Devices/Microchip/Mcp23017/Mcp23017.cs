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

using Lavspent.AsyncInline;
using Lavspent.DaisyChain.I2c;
using System.Collections.Generic;
using System.Linq;

namespace Lavspent.DaisyChain.Devices.Microchip.Mcp23017
{
    public class Mcp23017
    {
        private II2cDevice _i2cDevice;
        private byte[] _registers;

        public Mcp23017Port PortA
        {
            get; private set;
        }

        public Mcp23017Port PortB
        {
            get; private set;
        }

        public Mcp23017(II2cDevice i2cDevie)
        {
            _i2cDevice = i2cDevie;

            PortA = new Mcp23017Port(Mcp23017PortEnum.A, this);
            PortB = new Mcp23017Port(Mcp23017PortEnum.B, this);

            InitializeRegisters();
        }

        private void InitializeRegisters()
        {
            // These should be the default Power on/Reset register values of the Mcp23017
            _registers = new byte[]
            {
                0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
        }

        public byte ReadRegister(byte register)
        {
            byte[] readBuffer = new byte[1];
            _i2cDevice.WriteReadAsync(new byte[] { register }, readBuffer).WaitInline();

            _registers[register] = readBuffer[0];
            return _registers[register];
        }

        public void WriteRegister(byte register, byte value)
        {
            _i2cDevice.WriteRegisterAsync(register, value).WaitInline();
            _registers[register] = value;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class Register
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Register Olat = new Register(0x14);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Register Gpio = new Register(0x12);

        private static readonly List<Register> _registers = new List<Register>
        {
            Olat, Gpio
        };

        /// <summary>
        /// 
        /// </summary>
        public byte Address { get; private set; }

        private Register(byte address)
        {
            Address = address;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static Register FromAddress(byte address)
        {
            return _registers.FirstOrDefault(r => r.Address == address);
        }

    }
}

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

namespace Lavspent.DaisyChain.Devices.Microchip.Mcp23017
{
    /// <summary>
    /// 
    /// </summary>
    public enum Mcp23017PortEnum : byte
    {
        /// <summary>
        /// 
        /// </summary>
        A = 0,

        /// <summary>
        /// 
        /// </summary>
        B = 1
    }

    /// <summary>
    /// Control Registers. Their value depends on the port and the value of the
    /// IOCONx.Bank flag. If set then these values are (V &amp; 0x01) &lt;&lt; 4 | (V &amp; 0xFE) &gt;&gt; 1
    /// </summary>
    public class Mcp23017ControlRegisters
    {
        private IIoConBankProvider _ioConBankProvider;
        private Mcp23017PortEnum _port;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="ioConBank"></param>
        public Mcp23017ControlRegisters(Mcp23017PortEnum port, bool ioConBank)
        {
            _port = port;
            _ioConBankProvider = new StaticIoConBankProvider(ioConBank);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="mcp23017"></param>
        public Mcp23017ControlRegisters(Mcp23017PortEnum port, Mcp23017 mcp23017)
        {
            _port = port;
            _ioConBankProvider = new Mcp23017IoConBankProvider(mcp23017);
        }

        private bool IoConBank
        {
            get
            {
                return _ioConBankProvider.GetIoConBank();
            }
        }

        private const byte IODIR = 0x00;      // IO Direction Register
        private const byte IPOL = 0x02;       // Input Polarity Register 
        private const byte GPINTEN = 0x04;    // Interrupt-On-Change Control Register
        private const byte DEFVAL = 0x06;     // Default Compare Register For Interrupt-On-Change
        private const byte INTCON = 0x08;     // Interrupt Control Register
        private const byte IOCON = 0x0A;      // Configuration Register
        private const byte GPPU = 0x0C;       // Pull-up Resistor Configuration Register   
        private const byte INTF = 0x0E;       // Interrupt Flag Register
        private const byte INTCAP = 0x10;     // Interrupt Capture Register
        private const byte GPIO = 0x12;       // Port Register
        private const byte OLAT = 0x14;       // Output Latch Register

        public byte IoDir
        {
            get
            {
                return Convert(IODIR);
            }
        }

        public byte IPol
        {
            get
            {
                return Convert(IPOL);
            }
        }

        public byte GpIntEn
        {
            get
            {
                return Convert(GPINTEN);
            }
        }

        public byte DefVal
        {
            get
            {
                return Convert(DEFVAL);
            }
        }

        public byte IntCon
        {
            get
            {
                return Convert(INTCON);
            }
        }

        public byte GpPU
        {
            get
            {
                return Convert(GPPU);
            }
        }

        public byte IntF
        {
            get
            {
                return Convert(INTF);
            }
        }
        public byte IntCap
        {
            get
            {
                return Convert(INTCAP);
            }
        }
        public byte Gpio
        {
            get
            {
                return Convert(GPIO);
            }
        }
        public byte OLat
        {
            get
            {
                return Convert(OLAT);
            }
        }

        private byte Convert(byte value)
        {
            if (_port == Mcp23017PortEnum.A && IoConBank)
                value = (byte)(value >> 1);
            else if (_port == Mcp23017PortEnum.B && !IoConBank)
                value = (byte)(value + 1);
            else if (_port == Mcp23017PortEnum.B && IoConBank)
                value = (byte)((value >> 1) & 0x10);

            return value;
        }



        interface IIoConBankProvider
        {
            bool GetIoConBank();
        }

        class StaticIoConBankProvider : IIoConBankProvider
        {
            private bool _ioConBank;
            public StaticIoConBankProvider(bool ioConBank)
            {
                _ioConBank = ioConBank;
            }

            public bool GetIoConBank()
            {
                return _ioConBank;
            }
        }

        class Mcp23017IoConBankProvider : IIoConBankProvider
        {
            private Mcp23017 _mcp23017;
            public Mcp23017IoConBankProvider(Mcp23017 mcp23017)
            {
                _mcp23017 = mcp23017;
            }

            public bool GetIoConBank()
            {
                return false;
                //throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum Mcp23017IoConFlags : byte
    {
        /// <summary>
        /// Controls how registers are addressed.
        /// </summary>
        BANK = 1 << 7,

        /// <summary>
        /// INT Pins Mirror Bit
        /// </summary>
        MIRROR = 1 << 6,

        /// <summary>
        /// Sequential Operation Mode Bit
        /// </summary>
        SEQOP = 1 << 5,

        /// <summary>
        /// Slew Rate Control Bit For SDA Output
        /// </summary>
        DISSLW = 1 << 4,

        /// <summary>
        /// Hardware Address Enable Bit
        /// </summary>
        HAEN = 1 << 3,

        /// <summary>
        /// INT Output Open Drain Bit
        /// </summary>
        ODR = 1 << 2,

        /// <summary>
        /// INT Output Polarity Bit
        /// </summary>
        INTPOL = 1 << 1,

        /// <summary>
        /// 
        /// </summary>
        Unimplemented = 1 << 0
    }
}

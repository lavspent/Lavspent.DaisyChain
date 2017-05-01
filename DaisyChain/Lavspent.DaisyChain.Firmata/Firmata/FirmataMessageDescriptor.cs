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

using Lavspent.DaisyChain.Firmata.Messages;
using System;
using System.Linq;

namespace Lavspent.DaisyChain.Firmata
{
    public delegate IFirmataMessage FirmataMessageFactoryDelegate(FirmataMessageDescriptor commandInfo, uint command, byte[] data = null);
    public class FirmataMessageDescriptor
    {
        public uint Command { get; private set; }
        public uint CommandMask { get; private set; }
        public int CommandLength
        {
            get
            {
                return (Command > 0xffffff) ? 4 : (Command > 0xffff) ? 3 : (Command > 0xff) ? 2 : 1;
            }
        }

        public bool IsSysex { get; private set; }
        public byte Length { get; private set; }
        public FirmataMessageFactoryDelegate Factory { get; private set; }


        public FirmataMessageDescriptor(uint command, uint commandMask, bool isSysex, byte length, FirmataMessageFactoryDelegate factory)
        {
            Command = command;
            CommandMask = commandMask;
            IsSysex = isSysex;
            Length = length;
            Factory = factory;
        }

        public override bool Equals(object obj)
        {
            var rhs = obj as FirmataMessageDescriptor;
            if (rhs == null)
                return false;

            return
                Command == rhs.Command &&
                CommandMask == rhs.CommandMask &&
                IsSysex == rhs.IsSysex &&
                Length == rhs.Length &&
                Factory == rhs.Factory;
        }

        public override int GetHashCode()
        {
            return
                Command.GetHashCode() ^
                CommandMask.GetHashCode() ^
                IsSysex.GetHashCode() ^
                Length.GetHashCode() ^
                Factory.GetHashCode();
        }

        public bool IsPartialMatch(uint command)
        {
            // todo: take a look at this

            if (command == 0 || Command == 0)
                throw new Exception("Not supported!");

            int commandLength = 4;

            while (command < 0x00ffffff)
            {
                command <<= 8;
                commandLength--;
            }

            var myCommand = Command;
            var myMask = CommandMask;

            while (myCommand < 0x00ffffff)
            {
                myCommand <<= 8;
                myMask <<= 8;
            }

            var commandBytes = BitConverter.GetBytes(command).Reverse().ToArray();
            var myCommandBytes = BitConverter.GetBytes(myCommand).Reverse().ToArray();
            var maskBytes = BitConverter.GetBytes(myMask).Reverse().ToArray();

            bool match = true;

            for (int i = 0; i < commandLength; i++)
            {
                if ((commandBytes[i] & maskBytes[i]) != myCommandBytes[i])
                {
                    match = false;
                    break;
                }
            }

            return match;

            //var alignedCommand = AlignCommand(command);
            //return ((alignedCommand & CommandMask) == (Command & CommandMask));
        }

        private uint AlignCommand(uint command)
        {
            uint myCommand = Command >> 8;
            while (myCommand > 0)
            {
                command = command << 8;
                myCommand = myCommand >> 8;
            }

            return command;
        }
    }
}

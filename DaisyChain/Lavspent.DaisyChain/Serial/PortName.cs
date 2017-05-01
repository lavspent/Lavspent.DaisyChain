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

namespace Lavspent.DaisyChain.Serial
{
    public class PortName
    {
        public static readonly PortName None = new PortName("");

        private string _name;

        public PortName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _name = name;
        }

        public static implicit operator PortName(string name)
        {
            return new PortName(name);
        }

        public static implicit operator string(PortName portName)
        {
            return portName._name;
        }

        public override bool Equals(object obj)
        {
            // same reference?
            if (Object.ReferenceEquals(this, obj))
                return true;

            // same name?
            PortName rhs = obj as PortName;
            return rhs != null && Object.Equals(this._name, rhs._name);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}

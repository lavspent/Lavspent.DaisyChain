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
using Windows.Devices.SerialCommunication;

namespace Lavspent.DaisyChain.Serial
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HandshakeExtensions
    {
        /// <summary>
        /// Convert this UWP SerialHandshake to a DaisyChain Handshake.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Handshake AsHandshake(this SerialHandshake _this)
        {
            switch (_this)
            {
                case SerialHandshake.None:
                    return Handshake.None;
                case SerialHandshake.RequestToSend:
                    return Handshake.RequestToSend;
                case SerialHandshake.RequestToSendXOnXOff:
                    return Handshake.RequestToSendXOnXOff;
                case SerialHandshake.XOnXOff:
                    return Handshake.XOnXOff;

                default:
                    throw new Exception($"Unknown SerialHandshake: {_this}.");
            }
        }

        /// <summary>
        /// Convert this DaisyChain Handshake to a UWP SerialHandshake.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static SerialHandshake AsNativeSerialHandshake(this Handshake _this)
        {
            switch (_this)
            {
                case Handshake.None:
                    return SerialHandshake.None;
                case Handshake.RequestToSend:
                    return SerialHandshake.RequestToSend;
                case Handshake.RequestToSendXOnXOff:
                    return SerialHandshake.RequestToSendXOnXOff;
                case Handshake.XOnXOff:
                    return SerialHandshake.XOnXOff;

                default:
                    throw new Exception($"Unknown Handshake: {_this}.");
            }
        }
    }
}

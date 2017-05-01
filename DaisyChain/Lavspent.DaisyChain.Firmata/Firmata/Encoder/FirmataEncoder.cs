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

using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.Misc;
using System;
using System.Diagnostics;

namespace Lavspent.DaisyChain.Firmata.Encoder
{
    internal class FirmataEncoder : IEncoder, IDisposable
    {
        private FirmataEncoderController _encoderController;
        private int _pinA;
        private int _pinB;
        private Source<EncoderReading> _source;
        private IDisposable _encoderDataSubscription;
        private int _lastPosition;

        public byte EncoderNumber { get; set; }

        public FirmataEncoder(FirmataEncoderController encoderController, byte encoderNumber, byte pinA, byte pinB)
        {
            _encoderController = encoderController;
            EncoderNumber = encoderNumber;
            _pinA = pinA;
            _pinB = pinB;
            _source = new Source<EncoderReading>();

            RegisterEncoderDataObserver();
        }

        private void RegisterEncoderDataObserver()
        {
            _encoderDataSubscription = _encoderController.RegisterEncoderDataObserver(
                new Observer<EncoderData>(OnEncoderData));
        }

        private void UnregisterEncoderDataObserver()
        {
            _encoderDataSubscription.Dispose();
        }

        private void OnEncoderData(EncoderData encoderData)
        {
            //

            /* -----------------------------------------------------
            * 0 START_SYSEX                (0xF0)
            * 1 ENCODER_DATA               (0x61)
            * 2 Encoder #  &  DIRECTION    [= (direction << 6) | (#)]
            * 3 current position, bits 0-6
            * 4 current position, bits 7-13
            * 5 current position, bits 14-20
            * 6 current position, bits 21-27
            * 7 END_SYSEX                  (0xF7)
            * -----------------------------------------------------
            */

            byte encoder = (byte)((encoderData.Data[0] & 0x003F));
            //byte direction = (byte) ((encoderData.Data[0] & 0x40) >> 6);
            int currentPosition = (int)
                ((encoderData.Data[1] & 0x7f) |
                (encoderData.Data[2] & 0x7f) << 7 |
                (encoderData.Data[3] & 0x7f) << 14 |
                (encoderData.Data[4] & 0x7f) << 21);

            Debug.WriteLine($"{encoder} {currentPosition}");

            int delta =  currentPosition - _lastPosition;
            _lastPosition = currentPosition;

            if (delta == 0)
                return; // no change, ignore

            EncoderReading rotaryEncoderReading = new EncoderReading()
            {
                Direction = (delta > 0) ? EncoderReading.DirectionEnum.Clockwise : EncoderReading.DirectionEnum.CounterClockwise,
                Ticks = (uint)Math.Abs(delta)
            };

            _source.Next(rotaryEncoderReading);
        }

        public IDisposable Subscribe(IObserver<EncoderReading> observer)
        {
            return _source.Subscribe(observer);
        }

        public void Dispose()
        {
            UnregisterEncoderDataObserver();
            _encoderController.Close(this);
        }
    }
}

using Lavspent.DaisyChain.Stream;
using System;
using Windows.Devices.SerialCommunication;

namespace Lavspent.DaisyChain.Serial
{
    internal class UwpSerialDeviceStreamWrapper : AbstractUwpStorageStreamWrapper
    {
        // TODO: Relate to SerialDevice or UwpSerialDeviceWrapper?
        private SerialDevice _serialDevice;

        public UwpSerialDeviceStreamWrapper(SerialDevice serialDevice)
            : base(serialDevice.InputStream, serialDevice.OutputStream)
        {
            _serialDevice = serialDevice;
        }

        public override TimeSpan ReadTimeout
        {
            get
            {
                return _serialDevice.ReadTimeout;
            }

            set
            {
                _serialDevice.ReadTimeout = value;
            }
        }

        public override TimeSpan WriteTimeout
        {
            get
            {
                return _serialDevice.WriteTimeout;
            }

            set
            {
                _serialDevice.WriteTimeout = value;
            }
        }
    }
}

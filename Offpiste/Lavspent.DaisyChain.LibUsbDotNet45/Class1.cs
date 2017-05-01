using Lavspent.DaisyChain.Serial;
using LibUsbDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavspent.DaisyChain.Stream;
using System.Threading;
using LibUsbDotNet.Main;

namespace Lavspent.DaisyChain.LibUsbDotNet45
{
    public class UsbDeviceWrapper : IStream
    {
        private UsbEndpointReader _reader;
        private UsbEndpointWriter _writer;

        public UsbDeviceWrapper(IUsbDevice usbDevice)
            : this(usbDevice.OpenEndpointReader()
        {

        }

        public UsbDeviceWrapper(UsbEndpointReader reader, UsbEndpointWriter writer)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public UsbDeviceWrapper(IUsbDevice usbDevice)
        {
            _usbDevice = usbDevice ?? throw new ArgumentNullException(nameof(usbDevice));
            UsbEndpointList a;
            a.
            _reader = _usbDevice.OpenEndpointReader(ReadEndpointID.)
        }

        public TimeSpan ReadTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan WriteTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<int> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] buffer = new byte[1];
            int transferLength;

            ErrorCode result = _reader.Read(buffer, 0, 1, (int)this.ReadTimeout.TotalMilliseconds, out transferLength);
            if (result !=  ErrorCode.Success)
            {
                throw new Exception("ReadAsync failed, errorcode = " + result.ToString());
            }

            // TODO: End of stream?

            return Task.FromResult(transferLength);
        }

        public Task<int> ReadAsync(byte[] buffer, int index, int length, CancellationToken cancellationToken = default(CancellationToken))
        {
            int transferLength;
            ErrorCode result = _reader.Read(buffer, index, length, (int)this.ReadTimeout.TotalMilliseconds, out transferLength);
            if (result != ErrorCode.Success)
            {
                throw new Exception("ReadAsync failed, errorcode = " + result.ToString());
            }

            // TODO: End of stream?

            return Task.FromResult(transferLength);
        }

        public Task WriteAsync(byte data, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(byte[] data, int index, int length, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }

    public class Class1
    {
        IUsbDevice device;
        public void xxx()
        {
            var reader = device.OpenEndpointReader();
            reader.


            LibUsbDotNet.Main.
            var reader = device.OpenEndpointReader();
            reader.
        }
    }
}





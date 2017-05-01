using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.OneWire
{
    public class OneWireBusProxy : IOneWireBus
    {
        private IOneWireBus _oneWireBus;

        public OneWireBusProxy(IOneWireBus oneWireBus)
        {
            _oneWireBus = oneWireBus;
        }

        public Task<IEnumerable<OneWireAddress>> GetDeviceAddressesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.GetDeviceAddressesAsync(cancellationToken);
        }

        public Task MatchRomAsync(byte[] rom, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.MatchRomAsync(rom, cancellationToken);
        }

        public Task<byte> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.ReadAsync(cancellationToken);
        }

        public Task ReadAsync(byte[] buffer, int length, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.ReadAsync(buffer, length, cancellationToken);
        }

        public Task<byte> ReadBitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.ReadBitAsync(cancellationToken);
        }

        public Task ResetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.ResetAsync(cancellationToken);
        }

        public Task SkipRomAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.SkipRomAsync(cancellationToken);
        }

        public Task WriteAsync(byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.WriteAsync(data, cancellationToken);
        }

        public Task WriteBitAsync(byte data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _oneWireBus.WriteBitAsync(data, cancellationToken);
        }
    }

    //public static class AddressBoundOneWireBusExtensions
    //{
    //    public static Task<T> OpenDeviceAsync<T>(this IAddressBoundOneWireBus _this, IOneWireDeviceFactory<T> factory, CancellationToken cancellationToken = default(CancellationToken))
    //               where T : class, IOneWireDevice
    //    {
    //        return _this.OpenDeviceAsync<T>(_this.Address, factory, cancellationToken);
    //    }
    //}

    //public class test
    //{
    //    void texst()
    //    {

    //    }

        
    //}
}

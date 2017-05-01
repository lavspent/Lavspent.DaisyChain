using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.OneWire
{

    public interface IAddressBoundOneWireBus
    {
        OneWireAddress Address { get; }
    }

    /// <summary>
    /// Noooo, God, noooo!
    /// How to solve this? I want to extend IOneWireBus with a single method, while the rest is left untouched?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AddressBoundOneWireBus<T> : OneWireBusProxy, IAddressBoundOneWireBus where T : IOneWireBus
    {
        public OneWireAddress Address { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AddressBoundOneWireBus(T oneWireBus, OneWireAddress address)
            : base(oneWireBus)
        {
            this.Address = address;
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

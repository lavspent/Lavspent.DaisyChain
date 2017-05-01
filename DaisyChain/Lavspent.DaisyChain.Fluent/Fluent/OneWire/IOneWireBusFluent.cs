using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Fluent
{
    public class FluidThenWait<T>
    {
        public Task<T> Milliseconds
        {
            get
            {
                return Task<T>.Run(async () =>
                {
                    await Task.Delay(_i);
                    return _t;
                });
            }
        }

        private int _i;
        private T _t;

        public FluidThenWait(T t, int i)
        {
            _i = i;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public interface IFluentDaisyChain<T>
    {
        FluidThenWait<T> ThenWait(int ms);
    }

    public static class IFluentDaisyChaintExtensions
    {
        public static async Task<FluidThenWait<T>> ThenWait<T>(this Task<T> _this, int ms) where T : IFluentDaisyChain<T>
        {
            return (await _this).ThenWait(ms);
        }
    }

    public abstract class FluentDaisyChain<T> : IFluentDaisyChain<T>
    {
        protected abstract T _t { get; }

        public FluentDaisyChain()
        {
        }

        FluidThenWait<T> IFluentDaisyChain<T>.ThenWait(int ms)
        {
            return new FluidThenWait<T>(_t, ms);
        }
    }

    public interface IFluentOneWireBus : IFluentDaisyChain<IFluentOneWireBus>
    {
        Task<IFluentOneWireBus> Write();
    }

    public static class IFluentOneWireBusExtensions
    {
        public static async Task<IFluentOneWireBus> Write(this Task<IFluentOneWireBus> _this)
        {
            return await (await _this).Write();
        }
    }

    public class FluentOneWireBus : FluentDaisyChain<IFluentOneWireBus>, IFluentOneWireBus
    {
        protected override IFluentOneWireBus _t
        {
            get
            {
                return this;
            }
        }

        public FluentOneWireBus() : base()
        {
        }

        public Task<IFluentOneWireBus> Write()
        {
            throw new NotImplementedException();
        }
    }

    public class xx
    {
        async void t()
        {
            IFluentOneWireBus f = new FluentOneWireBus();
            var z =
                await f
                .ThenWait(5).Milliseconds
                .Write()
                .Write();

        }
    }
}

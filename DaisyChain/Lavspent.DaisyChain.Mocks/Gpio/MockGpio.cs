using Lavspent.Backport;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.Misc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Mocks.Gpio
{
    public class MockGpio : IGpio
    {
        private int _gpioNumber;
        private MockGpioController _mockGpioController;
        private GpioValue _value;
        private Source<GpioEdge> _source;
        private GpioDriveMode _driveMode;


        public MockGpio(MockGpioController mockGpioController, int gpioNumber)
        {
            _mockGpioController = mockGpioController;
            _gpioNumber = gpioNumber;
            _source = new Source<GpioEdge>();
            _driveMode = GpioDriveMode.Output;
        }


        public void Dispose()
        {
            _mockGpioController._gpios.Remove(_gpioNumber);
        }

        public Task<GpioDriveMode> GetDriveModeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(_driveMode);
        }

        public Task<bool> IsDriveModeSupportedAsync(GpioDriveMode driveMode, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        public Task<GpioValue> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(_value);
        }

        public Task SetDriveModeAsync(GpioDriveMode value, CancellationToken cancellationToken = default(CancellationToken))
        {
            _driveMode = value;
            return TaskEx.CompletedTask;
        }

        public IDisposable Subscribe(IObserver<GpioEdge> observer)
        {
            return _source.Subscribe(observer);
        }

        public Task WriteAsync(GpioValue value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_value != value)
            {
                _value = value;

                Task.Run(() =>
                {
                    _source.Next(value == GpioValue.High ? GpioEdge.RisingEdge : GpioEdge.FallingEdge);
                });
            }
            return TaskEx.CompletedTask;
        }
    }
}

using Lavspent.DaisyChain.Gpio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Mocks.Gpio
{
    public class MockGpioController : IGpioController
    {
        internal Dictionary<int, IGpioController> _gpios;

        public MockGpioController()
        {
            _gpios = new Dictionary<int, IGpioController>();
        }

        public int GpioCount
        {
            get
            {
                return 5;
            }
        }

        public Task<IGpio> OpenGpioAsync(int gpioNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_gpios.ContainsKey(gpioNumber))
                throw new Exception("Already open!");

            return Task.FromResult<IGpio>(new MockGpio(this, gpioNumber));
        }
    }
}

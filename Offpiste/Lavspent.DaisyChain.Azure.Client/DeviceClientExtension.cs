using Lavspent.DaisyChain.Gpio;
using Microsoft.Azure.Devices.Client;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Azure.Client
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class DeviceClientExtension
    {
        public static Task<IGpioController> GetGpioControllerAsync(this DeviceClient deviceClient)
        {
            IGpioController controller = new AzureIotHubGpioController(deviceClient);
            return Task.FromResult(controller);
        }
    }
}

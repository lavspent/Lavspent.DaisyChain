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

using Lavspent.Backport;
using Lavspent.DaisyChain.Firmata.Messages;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    public class AbstractFirmataServer : FirmataServerBase
    {
        public AbstractFirmataServer()
            : base()
        {
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            RegisterHandler<SystemReset>(HandleSystemResetAsync);
            RegisterHandler<FirmwareQuery>(HandleFirmwareQueryInternalAsync);
            RegisterHandler<CapabilityQuery>(HandleCapabilityQueryInternalAsync);
            RegisterHandler<SetDigitalPinValue>(HandleSetDigitalPinValue);
            //RegisterHandler<>();
            //RegisterHandler<>();
            //RegisterHandler<>();
            RegisterHandler<IFirmataMessage>(UnhandledFirmataMessageAsync);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        private Task UnhandledFirmataMessageAsync(IFirmataMessage message, CancellationToken cancellationToken)
        {
            return TaskEx.CompletedTask;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        private async Task HandleCapabilityQueryInternalAsync(CapabilityQuery message, CancellationToken cancellationToken)
        {
            var response = await HandleCapabilityQueryAsync(message, cancellationToken);
            await WriteAsync(response, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        protected virtual Task<CapabilityResponse> HandleCapabilityQueryAsync(CapabilityQuery message, CancellationToken cancellationToken)
        {
            PinCapabilities pinCapabilities = new PinCapabilities();
            List<PinCapability> pinCapabilityList = new List<PinCapability>();
            pinCapabilityList.Add(new PinCapability() { Mode = PinMode.Output, Pin = 1 });
            pinCapabilities.Add(1, pinCapabilityList);

            CapabilityResponse capabilityResponse = new CapabilityResponse()
            {
                PinCapabilities = pinCapabilities
            };

            return System.Threading.Tasks.Task.FromResult(capabilityResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        protected virtual Task HandleSetDigitalPinValue(SetDigitalPinValue message, CancellationToken cancellationToken)
        {
            // default does nothing
            Debug.WriteLine($"Setting pin {message.Pin} to {message.Value}");
            return TaskEx.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        protected virtual Task HandleSystemResetAsync(SystemReset message, CancellationToken cancellationToken)
        {
            // default does nothing
            return TaskEx.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        private async Task HandleFirmwareQueryInternalAsync(FirmwareQuery message, CancellationToken cancellationToken)
        {
            var response = await HandleFirmwareQueryAsync(message, cancellationToken);
            await WriteAsync(response, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        protected virtual Task<FirmwareResponse> HandleFirmwareQueryAsync(FirmwareQuery message, CancellationToken cancellationToken)
        {
            var response = new FirmwareResponse()
            {
                // todo: grab these from somewhere
                Firmware = "DaisyChainFirmata",
                MajorVersion = 1,
                MinorVersion = 0
            };

            return System.Threading.Tasks.Task.FromResult(response);
        }
    }
}

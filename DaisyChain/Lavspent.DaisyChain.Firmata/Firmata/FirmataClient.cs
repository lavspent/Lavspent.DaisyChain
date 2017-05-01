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
using Lavspent.DaisyChain.Adc;
using Lavspent.DaisyChain.Devices;
using Lavspent.DaisyChain.Firmata.Adc;
using Lavspent.DaisyChain.Firmata.Encoder;
using Lavspent.DaisyChain.Firmata.Gpio;
using Lavspent.DaisyChain.Firmata.Messages;
using Lavspent.DaisyChain.Firmata.Serial;
using Lavspent.DaisyChain.Gpio;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.OneWire;
using Lavspent.DaisyChain.Serial;
using Lavspent.DaisyChain.Stream;
using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata
{
    /// <summary>
    /// 
    /// </summary>
    public class FirmataClient : AbstractFirmataClient
    {
        /// <summary>
        /// 
        /// </summary>
        public byte MajorVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public byte MinorVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Firmware { get; set; }
        private bool _connected;

        /// <summary>
        /// 
        /// </summary>
        public PinCapabilities PinCapabilities { get; private set; }


        private AsyncLazy<FirmataAdcController> _lazyAdcController;
        private AsyncLazy<FirmataGpioController> _lazyGpioController;
        private AsyncLazy<FirmataI2cController> _lazyI2cController;
        private AsyncLazy<FirmataSerialController> _lazySerialController;
        private AsyncLazy<FirmataEncoderController> _lazyEncoderController;
        private AsyncLazy<FirmataOneWireBusController> _lazyOneWireBusController;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private FirmataClient(IStream stream)
            : base(stream)
        {
            _connected = false;

            // set up lazy controllers
            _lazyGpioController = new AsyncLazy<FirmataGpioController>(GpioContollerFactory);
            _lazyAdcController = new AsyncLazy<FirmataAdcController>(AdcControllerFactory);
            _lazyI2cController = new AsyncLazy<FirmataI2cController>(I2cControllerFactory);
            _lazySerialController = new AsyncLazy<FirmataSerialController>(SerialControllerFactory);
            _lazyEncoderController = new AsyncLazy<FirmataEncoderController>(EncoderControllerFactory);
            _lazyOneWireBusController = new AsyncLazy<FirmataOneWireBusController>(OneWireBusControllerFactory);
        }


        public static Task<FirmataClient> OpenAsync(ISerial serial, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OpenAsync(serial.GetStream());
        }

        public async static Task<FirmataClient> OpenAsync(Task<ISerial> serialTask, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await OpenAsync(await serialTask, cancellationToken);
        }

        public static async Task<FirmataClient> OpenAsync(IStream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            var firmataClient = new FirmataClient(stream);
            await firmataClient.ConnectAsync(cancellationToken).ConfigureAwait(false);
            return firmataClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        private async Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Do state check

            // start loop
            base.IssueRead();

            // Reset device
            await SystemResetAsync(cancellationToken).ConfigureAwait(false);

            // Query firmware
            var firmwareMessage = await QueryFirmwareAsync(cancellationToken).ConfigureAwait(false);
            MajorVersion = firmwareMessage.MajorVersion;
            MinorVersion = firmwareMessage.MinorVersion;
            Firmware = firmwareMessage.Firmware;

            // Query capabilities
            var capabilitiesMessage = await QueryCapabilitiesAsync(cancellationToken).ConfigureAwait(false);
            PinCapabilities = capabilitiesMessage.PinCapabilities;

            _connected = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task Disconnect()
        {
            // TODO: Stop loop


            _connected = false;
            return TaskEx.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<CapabilityResponse> QueryCapabilitiesAsync(CancellationToken cancellationToken)
        {
            var message = await SendRequestAsync(
                CapabilityQuery.MessageDescriptor,
                CapabilityResponse.MessageDescriptor,
                cancellationToken
                ).ConfigureAwait(false);

            return message as CapabilityResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delayMicroseconds"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        // TODO: Move to FirmataI2cController
        private Task SetI2cConfigAsync(ushort delayMicroseconds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var i2cConfig = new I2cConfig()
            {
                DelayMicroseconds = delayMicroseconds
            };

            return SendMessageAsync(
                i2cConfig,
                cancellationToken
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        // TODO: Move to FirmataI2cController
        public async Task<I2cReply> SendI2cReadAsync(byte address, byte length, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] writeBuffer = new byte[4];
            writeBuffer[0] = address;
            writeBuffer[1] = 0x8; // read
            writeBuffer[2] = (byte)(length & 0x7f);
            writeBuffer[3] = (byte)(length >> 7);

            var message = await SendRequestAsync(
                I2cRequest.MessageDescriptor,
                writeBuffer,
                I2cReply.MessageDescriptor,
                cancellationToken
                );

            return (I2cReply)message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        // TODO: Move to FirmataI2cController
        public Task SendI2cWriteRequestAsync(byte address, byte[] data, CancellationToken cancellationToken)
        {
            byte[] writeBuffer = new byte[data.Length * 2 + 2];
            writeBuffer[0] = address;
            writeBuffer[1] = 0; // write

            for (int i = 0; i < data.Length; i++)
            {
                writeBuffer[i * 2 + 2] = (byte)(data[i] & 0x007f);
                writeBuffer[i * 2 + 3] = (byte)((data[i] >> 7) & 0x007f);
            }

            return SendMessageAsync(I2cRequest.MessageDescriptor, writeBuffer, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<PinStateResponse> QueryPinStateAsync(byte pin, CancellationToken cancellationToken)
        {
            var message = await SendRequestAsync(
                PinStateQuery.MessageDescriptor,
                new byte[] { pin },
                PinStateResponse.MessageDescriptor,
                cancellationToken
                );

            return message as PinStateResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<ProtocolVersionResponse> QueryProtocolVersionAsync(CancellationToken cancellationToken)
        {
            var message = await SendRequestAsync(
                ProtocolVersionQuery.MessageDescriptor,
                ProtocolVersionResponse.MessageDescriptor,
                cancellationToken
                );

            return message as ProtocolVersionResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public async Task<FirmwareResponse> QueryFirmwareAsync(CancellationToken cancellationToken)
        {
            var message = await SendRequestAsync(
                FirmwareQuery.MessageDescriptor,
                FirmwareResponse.MessageDescriptor,
                cancellationToken
                ).ConfigureAwait(false);

            return message as FirmwareResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task SystemResetAsync(CancellationToken cancellationToken)
        {
            return SendMessageAsync(
                SystemReset.MessageDescriptor,
                cancellationToken
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="enable"></param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        /// <returns></returns>
        public Task ReportAnalogPinAsync(byte pin, bool enable, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageAsync(
                new ReportAnalogPin()
                {
                    Pin = pin,
                    Enable = enable
                },
                cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<int> ReadAnalogPinValueAsync()
        {
            // todo: implement
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="address"></param>
        ///// <param name="writeBuffer"></param>
        ///// <param name="readLength"></param>
        ///// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
        ///// <returns></returns>
        //public Task<IFirmataMessage> I2cWriteReadAsync(byte address, byte[] writeBuffer, byte readLength, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    throw new NotImplementedException();
        //}


        private Task<FirmataGpioController> GpioContollerFactory()
        {
            return System.Threading.Tasks.Task.FromResult(new FirmataGpioController(this));
        }

        private Task<FirmataAdcController> AdcControllerFactory()
        {
            return System.Threading.Tasks.Task.FromResult(new FirmataAdcController(this));
        }

        private async Task<FirmataI2cController> I2cControllerFactory()
        {
            // TODO: Move somewhere?
            await SetI2cConfigAsync(0);
            return new FirmataI2cController(this);
        }

        private Task<FirmataSerialController> SerialControllerFactory()
        {
            return System.Threading.Tasks.Task.FromResult(new FirmataSerialController(this));
        }

        private Task<FirmataEncoderController> EncoderControllerFactory()
        {
            return System.Threading.Tasks.Task.FromResult(new FirmataEncoderController(this));
        }

        private Task<FirmataOneWireBusController> OneWireBusControllerFactory()
        {
            return System.Threading.Tasks.Task.FromResult(new FirmataOneWireBusController(this));
        }


        public async Task<IGpioController> GetGpioControllerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Make this cancellable
            return await _lazyGpioController;
        }

        /// <summary>
        /// Get the Adc controller for this Firmata client.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async Task<IAdcController> GetAdcControllerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Make this cancellable
            return await _lazyAdcController;
        }

        /// <summary>
        /// Get the I2c controller for this Firmata client.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        //
        public async Task<II2cController> GetI2cControllerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Make this cancellable
            return await _lazyI2cController.ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the serial controller for this Firmata client.
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async Task<ISerialController> GetSerialControllerAsync()
        {
            // TODO: Make this cancellable
            return await _lazySerialController.ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async Task<IEncoderController> GetEncoderControllerAsync()
        {
            // TODO: Make this cancellable
            return await _lazyEncoderController.ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public async Task<IOneWireBusController> GetOneWireBusControllerAsync()
        {
            // TODO: Make this cancellable
            return await _lazyOneWireBusController.ConfigureAwait(false);
        }
    }
}
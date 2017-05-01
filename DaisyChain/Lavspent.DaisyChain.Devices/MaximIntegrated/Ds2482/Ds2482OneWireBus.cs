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

using Lavspent.DaisyChain.OneWire;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Devices.MaximIntegrated.Ds2482
{
    // TODO: We need to remove this chatty async/await stuff.
    // Move into seperate thread and use blocking Wait's?

    /// <summary>
    /// 
    /// </summary>
    public class Ds2482OneWireBus : IOneWireBus
    {
        private Ds2482 ds2482Device;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds2482Device"></param>
        public Ds2482OneWireBus(Ds2482 ds2482Device)
        {
            this.ds2482Device = ds2482Device;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync(CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            await ds2482Device.ClearStrongPullupAsync().ConfigureAwait(false);
            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            await ds2482Device.WriteRawAsync(Ds2482CommandCommands.OneWireReset).ConfigureAwait(false);

            Ds2482StatusFlags status = await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);

            //TODO: Optimize HasFlag
            if (status.HasFlag(Ds2482StatusFlags.ShortDetected))
                throw new Exception("Short detected.");

            // TODO: Clutter?
            //bool presence = status.HasFlag(Ds2482StatusFlags.PresencePulseDetected) ? true : false;
            //return presence;
        }

        /// <summary>
        /// 1-Wire skip
        /// </summary>
        /// <returns></returns>
        public async Task SkipRomAsync(CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            await ds2482Device.WriteRawAsync(
                Ds2482CommandCommands.OneWireWriteByte, OneWireCommands.SkipRom
                ).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteBitAsync(byte data, CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            await ds2482Device.WriteRawAsync(
                Ds2482CommandCommands.OneWireSingleBit, (byte)(data != 0 ? 0x80 : 0x00)
                ).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<byte> ReadBitAsync(CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            await WriteBitAsync(1, cancellationToken).ConfigureAwait(false);
            var status = await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            return (byte)(status.HasFlag(Ds2482StatusFlags.SingleBitResult) ? 1 : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rom"></param>
        /// <returns></returns>
        public async Task MatchRomAsync(byte[] rom, CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            //TODO: Optimize these, move to predefined array
            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            await ds2482Device.WriteRawAsync(Ds2482CommandCommands.OneWireWriteByte, OneWireCommands.MatchRom).ConfigureAwait(false);

            for (int i = 0; i < 8; i++)
            {
                await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
                await ds2482Device.WriteRawAsync(Ds2482CommandCommands.OneWireWriteByte, rom[i]).ConfigureAwait(false);
            }

            //TODO: Check if successful? Possible?
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OneWireAddress>> GetDeviceAddressesAsync(CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            byte[] lastAddress = new byte[8];
            byte lastDiscrepancy = 0;
            bool lastDeviceFlag = false;

            List<OneWireAddress> addresses = new List<OneWireAddress>();

            while (true)
            {

                byte direction;
                byte lastZero = 0;

                if (lastDeviceFlag)
                    break;
                //yield break;

                await ResetAsync(cancellationToken);

                // Check for 1-wire presence signal
                var status = await ds2482Device.ReadStatusAsync().ConfigureAwait(false);
                if (!status.HasFlag(Ds2482StatusFlags.PresencePulseDetected))
                    break;
                //yield break;

                await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);

                await ds2482Device.WriteRawAsync(
                    Ds2482CommandCommands.OneWireWriteByte, OneWireCommands.SearchRom
                    ).ConfigureAwait(false);

                for (byte i = 0; i < 64; i++)
                {
                    int searchByte = i / 8;
                    int searchBit = 1 << i % 8;

                    if (i < lastDiscrepancy)
                        direction = (byte)(lastAddress[searchByte] & searchBit);
                    else
                        direction = (byte)(i == lastDiscrepancy ? 1 : 0);

                    await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
                    await ds2482Device.WriteRawAsync(
                        Ds2482CommandCommands.OneWireTriplet, direction == 1 ? (byte)0x80 : (byte)0x00
                        ).ConfigureAwait(false);

                    status = await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);

                    byte id = (byte)(status & Ds2482StatusFlags.SingleBitResult);
                    byte comp_id = (byte)(status & Ds2482StatusFlags.TripletSecondBit);
                    direction = (byte)(status & Ds2482StatusFlags.BranchDirectionTaken);

                    if (id != 0 && comp_id != 0)
                    {
                        await ResetAsync(cancellationToken).ConfigureAwait(false);
                        break;
                    }
                    else
                    {
                        if (id == 0 && comp_id == 0 && direction == 0)
                        {
                            lastZero = i;
                        }
                    }

                    if (direction != 0)
                        lastAddress[searchByte] |= (byte)searchBit;
                    else
                        lastAddress[searchByte] &= (byte)~searchBit;
                }

                lastDiscrepancy = lastZero;

                if (lastZero == 0)
                    lastDeviceFlag = true;

                await ResetAsync(cancellationToken).ConfigureAwait(false);

                byte[] newAddress = new byte[8];
                Array.Copy(lastAddress, newAddress, 8);

                addresses.Add(newAddress);
            }

            return addresses;
        }

        private byte[] writeWriteBuffer = new byte[] { Ds2482CommandCommands.OneWireWriteByte, 0x00 };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task WriteAsync(byte[] data, CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            for (int i = 0; i < data.Length; i++)
            {
                await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
                writeWriteBuffer[1] = data[i];
                await ds2482Device.WriteRawAsync(writeWriteBuffer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<byte> ReadAsync(CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            await ds2482Device.WriteRawAsync(Ds2482CommandCommands.OneWireReadByte).ConfigureAwait(false);
            await ds2482Device.WaitOnBusyAsync().ConfigureAwait(false);
            return await ds2482Device.ReadDataAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task ReadAsync(byte[] buffer, int length, CancellationToken cancellationToken)
        {
            // todo: implement cancellation

            for (int i = 0; i < length; i++)
            {
                buffer[i] = await ReadAsync(cancellationToken);
            }
        }
    }
}
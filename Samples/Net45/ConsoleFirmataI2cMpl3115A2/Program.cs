using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.Serial;
using System;
using System.Threading.Tasks;

namespace ConsoleFirmataI2cMpl3115A2
{
    class Program
    {

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task ResetAsync(II2cDevice i2cDevice)
        {
            // reset device
            await i2cDevice.WriteRegisterAsync(0x26, 0x04);

            // wait fir reset to complete (reset bit clears)
            byte registerValue;
            do            
            {
                registerValue = await i2cDevice.ReadRegisterAsync(0x26);
            } while ((registerValue & 0x04) == 0x04);
            return;
        }

        static async Task MainAsync(string[] args)
        {
            // Setup serial comminucation on COM6
            var serialPort = await SerialPortController.OpenSerialAsync("COM6", 57600, Parity.None, 8, StopBits.One);

            // Initialize a Firmata client connected to that stream
            // (we assume that there is a Firmata server in hte other end).
            FirmataClient firmataClient = await FirmataClient.OpenAsync(serialPort);

            // Grab the I2cController provided by the firmata server
            var i2cController = await firmataClient.GetI2cControllerAsync();

            // /*0xc0*/
            var i2cDevice = i2cController.GetDevice(new I2cConnectionSettings(0x60)); // 

            await ResetAsync(i2cDevice);
            

            uint pressure = 0;
            byte[] data = new byte[1];
            byte[] rawPressureData = new byte[5];

            await i2cDevice.WriteReadAsync(new byte[] { 0x26 }, data);

            data[0] &= 0xFE;  // clear SBYB (bit 0) to STANDBY
            data[0] |= 0x02;  // set OST (bit 1) 
            await i2cDevice.WriteRegisterAsync(0x26, data[0] );

            await Task.Delay(100);

            await i2cDevice.WriteReadAsync(new byte[] { 0x01 }, rawPressureData);


            /*
            byte[] x1 = new byte[1];
            byte[] x2 = new byte[1];
            byte[] x3 = new byte[1];

            await i2cDevice.WriteReadAsync(new byte[] { 0x01 }, x1);
            await i2cDevice.WriteReadAsync(new byte[] { 0x02 }, x2);
            await i2cDevice.WriteReadAsync(new byte[] { 0x03 }, x3);
            */

            /*
            The pressure data is stored as a 20 - bit unsigned integer with a fractional part. The OUT_P_MSB (01h), OUT_P_CSB(02h) 
            and bits 7 to 6 of the OUT_P_LSB(03h) registers contain the integer part in Pascals.Bits 5 to 4 of OUT_P_LSB contain the 
            fractional component. This value is representative as a Q18.2 fixed point format where there are 18 integer bits 
            (including the signed bit) and two fractional bits.
            */
            uint integral = (uint) ((rawPressureData[0] << 10) | (rawPressureData[1] << 2) | ((rawPressureData[2] >> 6) & 0x03));
            //76543210
            pressure = (uint)(rawPressureData[0] << 16);
            pressure |= (uint)(rawPressureData[1] << 8);
            pressure |= rawPressureData[2];




            double pressurePascals = (pressure >> 6) + (((pressure >> 4) & 0x03) / 4.0);


            // Calculate using US Standard Atmosphere 1976 (NASA)

            double altitude = 44330.77 * (1 - Math.Pow(pressurePascals / 101326, 0.1902632));

            Console.WriteLine($"{pressure}  {pressurePascals}  {altitude}");

            //await i2cDevice.WriteRegisterAsync(0x26, 0xB8);
            //await i2cDevice.WriteRegisterAsync(0x13, 0x07);
            //await i2cDevice.WriteRegisterAsync(0x26, 0xB9);

            ////byte[] b = new byte[1];
            ////await i2cDevice.WriteAsync(new byte[] { 0 });
            ////await i2cDevice.ReadAsync(b);

            //while (true)
            //{
            //    byte status = await i2cDevice.ReadRegisterAsync(0);

            //    if ((status & 0x08) == 0x08)
            //    {
            //        byte p1 = await i2cDevice.ReadRegisterAsync(1);
            //        byte p2 = await i2cDevice.ReadRegisterAsync(2);
            //        byte p3 = await i2cDevice.ReadRegisterAsync(3);
            //        byte t1 = await i2cDevice.ReadRegisterAsync(4);
            //        byte t2 = await i2cDevice.ReadRegisterAsync(5);
            //    }
            //}


            await Task.Delay(1000);

            firmataClient.Dispose();

            // dispose the SerialPort
            serialPort.Dispose();
        }
    }
}

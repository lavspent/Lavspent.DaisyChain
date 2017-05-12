using Lavspent.DaisyChain.Firmata;
using Lavspent.DaisyChain.I2c;
using Lavspent.DaisyChain.Serial;
using System.Threading.Tasks;

namespace ConsoleFirmataI2cSd1306
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static II2cDevice _i2cDevice;

        static async Task MainAsync(string[] args)
        {
            // open serial (port)
            var serial = await SerialPortController.OpenSerialAsync("COM6", 57600, Parity.None, 8, StopBits.One);

            // Initialize a firmata client. We assume there is a ConfigurableFirmata/or other firmata with support for Encoders on the other side.
            var firmataClient = await FirmataClient.OpenAsync(serial);
            var i2cController = await firmataClient.GetI2cControllerAsync();

            _i2cDevice = await i2cController.OpenDeviceAsync(new I2cConnectionSettings() { BusSpeed = I2cBusSpeed.StandardMode, SlaveAddress = 0x7A });

            /*
            await i2cDevice.WriteAsync(new byte[] { 0x00, 0xA5 });

            await i2cDevice.WriteAsync(new byte[] { 0x00, 0xAF });
            await i2cDevice.WriteAsync(new byte[] { 0x00, 0xA7 });
            */


            // Init sequence
            ssd1306_command(0xae /*SSD1306_DISPLAYOFF*/);                    // 0xAE
            ssd1306_command(0xd5 /*SSD1306_SETDISPLAYCLOCKDIV*/);            // 0xD5
            ssd1306_command(0x80);                                  // the suggested ratio 0x80


            ssd1306_command(0xa8 /*SSD1306_SETMULTIPLEX*/);                  // 0xA8
            ssd1306_command(95 /*SSD1306_LCDHEIGHT - 1*/);

            
            ssd1306_command(0xd3 /*SSD1306_SETDISPLAYOFFSET*/);              // 0xD3
            ssd1306_command(0x0);                                   // no offset
            //ssd1306_command(SSD1306_SETSTARTLINE | 0x0);            // line #0
            ssd1306_command(0x8d /*SSD1306_CHARGEPUMP*/);                    // 0x8D

            ssd1306_command(0x14);

            ssd1306_command(0x20 /*SSD1306_MEMORYMODE*/);                    // 0x20
            ssd1306_command(0x00);                                  // 0x0 act like ks0108

            //ssd1306_command(SSD1306_SEGREMAP | 0x1);
            //ssd1306_command(SSD1306_COMSCANDEC);




            ssd1306_command(0xda /*SSD1306_SETCOMPINS*/);                    // 0xDA
            ssd1306_command(0x12);
            ssd1306_command(0x81 /*SSD1306_SETCONTRAST*/);                   // 0x81
            ssd1306_command(0xCF);




            ssd1306_command(0xd9 /*SSD1306_SETPRECHARGE*/);                  // 0xd9

            ssd1306_command(0xF1);

            ssd1306_command(0xdb /*SSD1306_SETVCOMDETECT*/);                 // 0xDB

            ssd1306_command(0x40);

            ssd1306_command(0xa4 /*SSD1306_DISPLAYALLON_RESUME*/);           // 0xA4

            ssd1306_command(0xa6 /*SSD1306_NORMALDISPLAY*/);                 // 0xA6

//            ssd1306_command(SSD1306_DEACTIVATE_SCROLL);

            ssd1306_command(0xaf /*SSD1306_DISPLAYON*/);//--turn on oled panel










            /*            // I2C

                        uint8_t control = 0x00;   // Co = 0, D/C = 0

                        Wire.beginTransmission(_i2caddr);

                        Wire.write(control);

                        Wire.write(c);

                        Wire.endTransmission();

            */



        }

        public static void ssd1306_command(byte command)
        {
            _i2cDevice.WriteAsync(new byte[] { 0x00, command }).Wait();
        }
    }
}

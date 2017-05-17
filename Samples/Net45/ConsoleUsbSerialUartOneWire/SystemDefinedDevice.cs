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

using System;

namespace ConsoleUsbSerialUartOneWire
{

    public class SystemDefinedDevice
    {
        public static readonly SystemDefinedDevice Battery = new SystemDefinedDevice("72631e54-78a4-11d0-bcf7-00aa00b7b32a");
        public static readonly SystemDefinedDevice Biometric = new SystemDefinedDevice("53D29EF7-377C-4D14-864B-EB3A85769359");
        public static readonly SystemDefinedDevice Bluetooth = new SystemDefinedDevice("e0cbf06c-cd8b-4647-bb8a-263b43f0f974");
        public static readonly SystemDefinedDevice CDROM = new SystemDefinedDevice("4d36e965-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice DiskDrive = new SystemDefinedDevice("4d36e967-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Display = new SystemDefinedDevice("4d36e968-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice FDC = new SystemDefinedDevice("4d36e969-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice FloppyDisk = new SystemDefinedDevice("4d36e980-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice GPS = new SystemDefinedDevice("6bdd1fc3-810f-11d0-bec7-08002be2092f");
        public static readonly SystemDefinedDevice HDC = new SystemDefinedDevice("4d36e96a-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice HIDClass = new SystemDefinedDevice("745a17a0-74d3-11d0-b6fe-00a0c90f57da");
        public static readonly SystemDefinedDevice IEEE_1284_Dot4 = new SystemDefinedDevice("48721b56-6795-11d2-b1a8-0080c72e74a2");
        public static readonly SystemDefinedDevice IEEE_1284_Dot4Print = new SystemDefinedDevice("49ce6ac8-6f86-11d2-b1e5-0080c72e74a2");
        public static readonly SystemDefinedDevice IEEE_1394_61883 = new SystemDefinedDevice("7ebefbc0-3200-11d2-b4c2-00a0C9697d07");
        public static readonly SystemDefinedDevice IEEE_1394_AVC = new SystemDefinedDevice("c06ff265-ae09-48f0-812c-16753d7cba83");
        public static readonly SystemDefinedDevice IEEE_1394_SBP = new SystemDefinedDevice("d48179be-ec20-11d1-b6b8-00c04fa372a7");
        public static readonly SystemDefinedDevice IEEE_1394 = new SystemDefinedDevice("6bdd1fc1-810f-11d0-bec7-08002be2092f");
        public static readonly SystemDefinedDevice Image = new SystemDefinedDevice("6bdd1fc6-810f-11d0-bec7-08002be2092f");
        public static readonly SystemDefinedDevice Infrared = new SystemDefinedDevice("6bdd1fc5-810f-11d0-bec7-08002be2092f");
        public static readonly SystemDefinedDevice Keyboard = new SystemDefinedDevice("4d36e96b-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice MediumChanger = new SystemDefinedDevice("ce5939ae-ebde-11d0-b181-0000f8753ec4");
        public static readonly SystemDefinedDevice MTD = new SystemDefinedDevice("4d36e970-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Modem = new SystemDefinedDevice("4d36e96d-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Monitor = new SystemDefinedDevice("4d36e96e-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Mouse = new SystemDefinedDevice("4d36e96f-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Multifunction = new SystemDefinedDevice("4d36e971-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Multimedia = new SystemDefinedDevice("4d36e96c-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice MultiportSerial = new SystemDefinedDevice("50906cb8-ba12-11d1-bf5d-0000f805f530");
        public static readonly SystemDefinedDevice Net = new SystemDefinedDevice("4d36e972-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice NetClient = new SystemDefinedDevice("4d36e973-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice NetService = new SystemDefinedDevice("4d36e974-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice NetTrans = new SystemDefinedDevice("4d36e975-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice SecurityAccelerator = new SystemDefinedDevice("268c95a1-edfe-11d3-95c3-0010dc4050a5");
        public static readonly SystemDefinedDevice PCMCIA = new SystemDefinedDevice("4d36e977-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Ports = new SystemDefinedDevice("4d36e978-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Printer = new SystemDefinedDevice("4d36e979-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice PNPPrinters = new SystemDefinedDevice("4658ee7e-f050-11d1-b6bd-00c04fa372a7");
        public static readonly SystemDefinedDevice Processor = new SystemDefinedDevice("50127dc3-0f36-415e-a6cc-4cb3be910b65");
        public static readonly SystemDefinedDevice SCSIAdapter = new SystemDefinedDevice("4d36e97b-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice Sensor = new SystemDefinedDevice("5175d334-c371-4806-b3ba-71fd53c9258d");
        public static readonly SystemDefinedDevice SmartCardReader = new SystemDefinedDevice("50dd5230-ba8a-11d1-bf5d-0000f805f530");
        public static readonly SystemDefinedDevice Volume = new SystemDefinedDevice("71a27cdd-812a-11d0-bec7-08002be2092f");
        public static readonly SystemDefinedDevice System = new SystemDefinedDevice("4d36e97d-e325-11ce-bfc1-08002be10318");
        public static readonly SystemDefinedDevice TapeDrive = new SystemDefinedDevice("6d807884-7d21-11cf-801c-08002be10318");
        public static readonly SystemDefinedDevice USBDevice = new SystemDefinedDevice("88BAE032-5A81-49f0-BC3D-A4FF138216D6");
        public static readonly SystemDefinedDevice WCEUSBS = new SystemDefinedDevice("25dbce51-6c8f-4a72-8a6d-b54c2b4fc835");
        public static readonly SystemDefinedDevice WPD = new SystemDefinedDevice("eec5ad98-8080-425f-922a-dabf3de3f69a");
        public static readonly SystemDefinedDevice SideShow = new SystemDefinedDevice("997b5d8d-c442-4f2e-baf3-9c8e671e9e21");

        /// <summary>
        /// Guid of this device class.
        /// </summary>
        public Guid ClassGuid { get; private set; }

        private SystemDefinedDevice(string classGuid)
        {
            this.ClassGuid = Guid.Parse(classGuid);
        }
    }
}

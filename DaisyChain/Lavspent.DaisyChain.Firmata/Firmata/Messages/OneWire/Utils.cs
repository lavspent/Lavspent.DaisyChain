using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Firmata.Messages.OneWire
{
    public class Utils
    {

        public static byte[] Convert7BitArrayTo8BitArray(byte[] source)
        {
            List<byte> resultBytes = new List<byte>();
            int sourceIndex = 0;

            int i = 0;
            while (true)
            {
                sourceIndex = (int)Math.Floor((double)(i * 8) / 7);

                if (sourceIndex >= source.Length - 1)
                    break;

                int bitOffset = i % 7;
                byte low = (byte)(source[sourceIndex] >> bitOffset);
                byte high = (byte)((source[sourceIndex + 1] << (7 - bitOffset)) & 0xf7);


                byte v = (byte)(low | high);
                resultBytes.Add(v);

                i++;
            }

            return resultBytes.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavspent.DaisyChain.Misc
{
    public static class DisposedUtil
    {
        public static void Check(object source, bool disposed)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(source.GetType().FullName);
            }
        }
    }
}

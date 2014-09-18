using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScPlums.Aids
{
    internal static class InternalHelpers
    {
        public static string IfNotNullOrEmpty(this string source)
        {
            return !string.IsNullOrEmpty(source) ? source : null;
        }
    }
}

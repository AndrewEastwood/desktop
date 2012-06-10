using System;
using System.Collections.Generic;
using System.Text;

namespace mdcore.Common
{
    public static class pdLogger
    {
        public static void Log(Exception ex)
        {
            Log(ex, string.Empty);
        }

        public static void Log(Exception ex, string message)
        {
        }
    }
}

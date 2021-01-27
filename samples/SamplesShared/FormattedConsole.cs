using System;
using System.Globalization;

namespace SamplesShared
{
    public static class FormattedConsole
    {
        public static void WriteLine(string line)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture)}] - {line}");
        }
    }
}
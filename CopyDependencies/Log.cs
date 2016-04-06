using System;
using Microsoft.Build.Framework;

namespace CopyDependencies
{
    internal static class Log
    {
        public static void LogMessage(MessageImportance importance, string message)
        {
            Console.WriteLine("{0} {1}", importance, message);
        }
    }
}
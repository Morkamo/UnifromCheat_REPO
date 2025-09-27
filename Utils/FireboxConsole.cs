using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnifromCheat_REPO.Utils
{
    public static class FireboxConsole
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, IntPtr lpReserved);

        private const int STD_OUTPUT_HANDLE = -11;
        private static IntPtr consoleHandle;
        private static bool initialized;

        public static void Init()
        {
            if (initialized) return;

            AllocConsole();
            consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            initialized = true;
        }

        public static void FireLog(string message)
        {
            if (initialized && consoleHandle != IntPtr.Zero)
            {
                WriteConsole(consoleHandle, message + "\n", (uint)message.Length + 1, out _, IntPtr.Zero);
            }

            Debug.Log(message);
        }

        public static void Close()
        {
            if (initialized)
            {
                FreeConsole();
                initialized = false;
                consoleHandle = IntPtr.Zero;
            }
        }
    }
}
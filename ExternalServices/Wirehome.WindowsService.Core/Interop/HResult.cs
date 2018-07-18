﻿namespace Wirehome.WindowsService.Interop
{
    public static class HResult
    {
        public static readonly int OK = 0;
        public static readonly int NotFound = unchecked((int)0x80070490);
        public static readonly int FileNotFound = unchecked((int)0x80070002);
    }
}

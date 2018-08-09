﻿using System.Collections;

namespace OPCClientApp.Connection.Types
{
    /// <summary>
    /// Contains the conversion methods to convert Bit from S7 plc to C#.
    /// </summary>
    public static class Bit
    {
        /// <summary>
        /// Converts a Bit to bool
        /// </summary>
        public static bool FromByte(byte v, byte bitAdr)
        {
            return (((int)v & (1 << bitAdr)) != 0);
        }

        /// <summary>
        /// Converts an array of bytes to a BitArray
        /// </summary>
        public static BitArray ToBitArray(byte[] bytes)
        {
            BitArray bitArr = new BitArray(bytes);
            return bitArr;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AODb.Common
{
    public static class BinaryReaderExtensions
    {

        public static int ReadInt16Rev(this BinaryReader reader) => IPAddress.NetworkToHostOrder(reader.ReadInt16());
        public static int ReadInt32Rev(this BinaryReader reader) => IPAddress.NetworkToHostOrder(reader.ReadInt32());
        public static int Read3F1(this BinaryReader reader) => (reader.ReadInt32() / 0x03F1) - 1;
        public static int Read3F1Rev(this BinaryReader reader) => (reader.ReadInt32Rev() / 0x03F1) - 1;
        public static void Skip(this BinaryReader reader, int count) => reader.BaseStream.Position += count;

        public static int ReadInt32_At(this BinaryReader reader, uint position)
        {
            reader.BaseStream.Position = position;
            return reader.ReadInt32();
        }

        public static uint ReadUInt32_At(this BinaryReader reader, uint position)
        {
            reader.BaseStream.Position = position;
            return reader.ReadUInt32();
        }

        public static string ReadHash(this BinaryReader reader , bool reverse = false)
        {
            byte[] array = reader.ReadBytes(4);
            if (reverse) Array.Reverse(array);
            return Encoding.ASCII.GetString(array);
        }

        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            List<char> bytes = new List<char>();

            while (true)
            {
                var val = reader.ReadChar();
                if (val == 0)
                {
                    break;
                }

                bytes.Add(val);
            }

            return new string(bytes.ToArray());
        }

        public static int[] ReadIdentity(this BinaryReader reader)
        {
            var result = new int[2];
            result[0] = reader.ReadInt32();
            result[1] = reader.ReadInt32();
            return result;
        }

        public static string ReadStringRev(this BinaryReader br)
        {
            int count = IPAddress.NetworkToHostOrder(br.ReadInt32());
            count--;
            StringBuilder stringBuilder = new StringBuilder();
            while (count > 0)
            {
                byte b = br.ReadByte();
                stringBuilder.Append((char)b);
                count--;
            }
            return stringBuilder.ToString();
        }
    }
}

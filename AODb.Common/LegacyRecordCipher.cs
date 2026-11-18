using System;

namespace AODb.Common
{
    // Item and nano records in clients up to 13.91 are XOR-obfuscated with a keystream seeded by the record's AOID.
    // Payload layout: type (4), AOID (4), version (4), then obfuscated bytes to the end. Algorithm from adbd (auno.org).
    public static class LegacyRecordCipher
    {
        public const int ItemType = 1000020;
        public const int NanoProgramType = 1040005;

        private const int PlainHeaderLength = 12;
        private const ulong Multiplier = 0x1012003;
        private const ulong Modulus = 0x4e512dc8f;

        public static bool AppliesTo(int type) => type == ItemType || type == NanoProgramType;

        public static void Decode(byte[] payload)
        {
            if (payload.Length <= PlainHeaderLength)
                return;

            ulong seed = (ulong)(long)BitConverter.ToInt32(payload, 4);
            for (int i = PlainHeaderLength; i < payload.Length; i++)
            {
                seed = seed * Multiplier % Modulus;
                payload[i] ^= (byte)seed;
            }
        }
    }
}

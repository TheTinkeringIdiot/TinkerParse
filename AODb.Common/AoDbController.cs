using AODb.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AODb.Common
{
    internal class AoDbController : IDbController
    {
        public const ulong FILE_LENGTH = 0x40000000;

        private Dictionary<int, Dictionary<int, ulong>> _records;

        public Dictionary<int, BinaryReader> _dataReaders;

        private string _idxPath;

        private uint _blockOffset;
        private uint _dataFileSize;

        public AoDbController(string idxPath)
        {
            _idxPath = idxPath;

            LoadRecordTypes();
            LoadReaders();
        }

        private void LoadReaders()
        {
            _dataReaders = new Dictionary<int, BinaryReader>();
            var files = Directory.GetFiles(Path.GetDirectoryName(_idxPath));
            foreach (var f in files)
            {
                if (f.Contains(".dat"))
                {
                    if (f.EndsWith(".dat"))
                    {
                        if (!_dataReaders.ContainsKey(0))
                        {
                            _dataReaders.Add(0, new BinaryReader(new FileStream(f, FileMode.Open)));
                        }
                    }
                    else
                    {
                        var idxStr = f.Substring(f.Length - 3, 3);
                        var idx = int.Parse(idxStr);
                        if (!_dataReaders.ContainsKey(idx))
                        {
                            _dataReaders.Add(idx, new BinaryReader(new FileStream(f, FileMode.Open)));
                        }
                    }
                }
            }
        }

        private void LoadRecordTypes()
        {
            _records = new Dictionary<int, Dictionary<int, ulong>>();

            using (var reader = new BinaryReader(new FileStream(_idxPath, FileMode.Open)))
            {
                _blockOffset = reader.ReadUInt32_At(12);
                var dataStart = reader.ReadInt32_At(72);
                _dataFileSize = reader.ReadUInt32_At(184);

                reader.BaseStream.Position = dataStart;

                while (true)
                {
                    var nextBlock = reader.ReadInt32();
                    var prevBlock = reader.ReadInt32();

                    var recordCount = reader.ReadInt16();
                    var unk1 = reader.ReadInt32();
                    var unk2 = reader.ReadInt32();

                    var unk3 = reader.ReadInt32();
                    var unk4 = reader.ReadInt32();
                    var unk5 = reader.ReadInt16();

                    for (int i = 0; i < recordCount; i++)
                    {
                        ulong offset = reader.ReadUInt32();
                        ulong offset2 = reader.ReadUInt32();
                        offset = offset << 32;
                        offset |= offset2;

                        int type = reader.ReadInt32Rev();
                        int inst = reader.ReadInt32Rev();

                        if (!_records.ContainsKey(type))
                            _records.Add(type, new Dictionary<int, ulong>());

                        _records[type].Add(inst, offset);
                    }

                    if (nextBlock == 0)
                    {
                        break;
                    }

                    reader.BaseStream.Position = nextBlock;
                }
            }
        }

        public byte[] Get(int type, int record)
        {
            if (!_records.ContainsKey(type) || !_records[type].ContainsKey(record))
                return null;

            ulong offset = _records[type][record];

            int readerIdx = (int)Math.Floor((double)offset / (double)_dataFileSize);

            if (readerIdx > 0)
                offset = (uint)((ulong)offset - (ulong)(_dataFileSize - _blockOffset) * (ulong)readerIdx);

            _dataReaders[readerIdx].BaseStream.Position = (long)offset + 10;

            BinaryReader reader = _dataReaders[readerIdx];

            var typeInDat = reader.ReadInt32();
            var instInDat = reader.ReadInt32();

            if (typeInDat != type || instInDat != record)
            {
                throw new ArgumentException();
            }

            var size = reader.ReadInt32();
            return reader.ReadBytes(size);
        }

        public void Put(uint type, uint record, byte[] data)
        {
            throw new NotSupportedException();
        }

        public Dictionary<int, Dictionary<int, ulong>> GetRecords()
        {
            return _records;
        }

        public void Close()
        {
            foreach (var reader in _dataReaders)
            {
                reader.Value.Close();
                reader.Value.Dispose();
            }

            _dataReaders.Clear();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

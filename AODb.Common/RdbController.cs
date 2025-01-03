using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
// using AODb.Common.DbClasses;
// using AODb.Common.RDBObjects;

namespace AODb.Common
{
    public class RdbController : IDisposable
    {
        private IDbController _dbController;

        public Dictionary<int, Dictionary<int, ulong>> RecordTypeToId => _dbController.GetRecords();

        private bool disposedValue;

        public RdbController(string path, bool isPrk)
        {
            if (isPrk)
            {
                _dbController = new PRKController(Path.Combine(path, "cd_image/rdb.db"));
            }
            else
            {
                _dbController = new AoDbController(Path.Combine(path, "cd_image/data/db/ResourceDatabase.idx"));
            }
        }

        public BinaryReader Get(int type, int instance)
        {
            var result = _dbController.Get(type, instance);
            if (result == null)
                return null;
            return new BinaryReader(new MemoryStream(result));
            // using (var reader = new BinaryReader(new MemoryStream(result)))
            // {
            //     return reader;
            // }
        }

        // public RDBObject Get(int type, int instance)
        // {
        //     var result = _dbController.Get(type, instance);
        //     if (result == null)
        //         return null;
        //     using (var reader = new BinaryReader(new MemoryStream(result)))
        //     {
        //         var recordType = reader.ReadInt32();
        //         var recordInst = reader.ReadInt32();
        //         var version = reader.ReadInt32();

        //         var dbObj = RDBObject.GetRdbObjectForRecordType(type);

        //         dbObj.RecordType = recordType;
        //         dbObj.RecordId = recordInst;
        //         dbObj.RecordVersion = version;

        //         dbObj.Deserialize(reader);

        //         return dbObj;
        //     }
        // }

        // public T Get<T>(int instance) where T : RDBObject, new()
        // {
        //     var type = typeof(T).GetCustomAttribute<RDBRecordAttribute>().RecordTypeID;
        //     return (T)Get(type, instance);
        // }

        // public T Get<T>(ResourceTypeId type, int instance) where T : RDBObject, new()
        // {
        //     var result = _dbController.Get((int)type, instance);
        //     if (result == null)
        //         return null;
        //     using (var reader = new BinaryReader(new MemoryStream(result), Encoding.GetEncoding(1252)))
        //     {
        //         var recordType = reader.ReadInt32();
        //         var recordInst = reader.ReadInt32();
        //         var version = reader.ReadInt32();

        //         var dbObj = new T();

        //         dbObj.RecordType = recordType;
        //         dbObj.RecordId = recordInst;
        //         dbObj.RecordVersion = version;

        //         dbObj.Deserialize(reader);

        //         return dbObj;
        //     }
        // }

        public byte[] GetRaw(int type, int instance)
        {
            var result = _dbController.Get(type, instance);

            if (result == null)
                return null;

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _dbController.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

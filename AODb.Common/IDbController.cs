namespace AODb.Common;
using System;
using System.Collections.Generic;
using System.IO;

public interface IDbController
{
    byte[] Get(int type, int instance);
    void Put(uint type, uint record, byte[] data);
    Dictionary<int, Dictionary<int, ulong>> GetRecords();

    void Dispose();
}
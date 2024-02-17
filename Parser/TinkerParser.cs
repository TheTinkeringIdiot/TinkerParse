using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AODb.Common;
using AODb.Common.Attributes;
using AODb.Data;

namespace Parser
{
    internal class TinkerParser
    {
        static void Main(string[] args)
        {
            string aoPath = args[0];
            Directory.SetCurrentDirectory(aoPath);
            RdbController rdbController = new RdbController(aoPath);

            // FileStream fs = new FileStream(AppContext.BaseDirectory + "/data.bin", FileMode.Create);
            // BinaryWriter bw = new BinaryWriter(fs);
            // byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            // bw.Write(bytes);
            // bw.Close();
            // fs.Close();

            List<Item> items = new List<Item>();

            int count = 0;

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Item])
            {
                if(count % 5000 == 0) { Console.WriteLine($"Parsed items: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
                // BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, 305542);
                Item item = new Item();
                item.PopulateFromStream(reader);
                items.Add(item);
            }

        
            Console.WriteLine(aoPath);
        }

    }
}


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
            List<NanoProgram> nanos = new List<NanoProgram>();
            List<Icon> icons = new List<Icon>();

            int count = 0;

            // Console.WriteLine("Parsing Items...");
            // foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Item])
            // {
            //     if(count % 5000 == 0) { Console.WriteLine($"Parsed items: {count}"); }
            //     count++;
            //     BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
            // BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, 305554);
            // Item item = new Item();
            // item.PopulateFromStream(reader);
            //     items.Add(item);
            // }

            // Console.WriteLine("Parsing Nanos...");
            // count = 0;
            // foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.NanoProgram])
            // {
            //     if(count % 1000 == 0) { Console.WriteLine($"Parsed items: {count}"); }
            //     count++;
            //     BinaryReader reader = rdbController.Get((int)RdbRecordType.NanoProgram, kvp.Key);
            //     NanoProgram nano = new NanoProgram();
            //     nano.PopulateFromStream(reader);
            //     nanos.Add(nano);
            // }

            Console.WriteLine("Parsing Icons...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Icon])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed items: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Icon, kvp.Key);
                Icon icon = new Icon();
                icon.PopulateFromStream(reader);
                icons.Add(icon);
            }


            


        
            Console.WriteLine(aoPath);
        }

    }
}


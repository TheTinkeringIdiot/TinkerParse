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
            List<Texture> textures = new List<Texture>();
            List<DungeonTexture> dungeonTextures = new List<DungeonTexture>();
            List<GroundTexture> groundTextures = new List<GroundTexture>();
            List<SkinTexture> skinTextures = new List<SkinTexture>();
            List<BodyTextureMedium> btmTextures = new List<BodyTextureMedium>();
            List<BodyTextureSmall> btsTextures = new List<BodyTextureSmall>();
            List<GroundTextureMedium> gtmTextures = new List<GroundTextureMedium>();
            List<GroundTextureSmall> gtsTextures = new List<GroundTextureSmall>();
            List<DungeonTextureMedium> dtmTextures = new List<DungeonTextureMedium>();
            List<DungeonTextureSmall> dtsTextures = new List<DungeonTextureSmall>();
            

            int count = 0;

            Console.WriteLine("Parsing Items...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Item])
            {
                if(count % 5000 == 0) { Console.WriteLine($"Parsed items: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
                Item item = new Item();
                item.PopulateFromStream(reader);
                items.Add(item);
            }

            Console.WriteLine("Parsing Nanos...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.NanoProgram])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed nanos: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.NanoProgram, kvp.Key);
                NanoProgram nano = new NanoProgram();
                nano.PopulateFromStream(reader);
                nanos.Add(nano);
            }

            Console.WriteLine("Parsing Icons...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Icon])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed icons: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Icon, kvp.Key);
                Icon icon = new Icon();
                icon.PopulateFromStream(reader);
                icons.Add(icon);
            }

            Console.WriteLine("Parsing Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Texture])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Texture, kvp.Key);
                Texture tex = new Texture();
                tex.PopulateFromStream(reader);
                textures.Add(tex);
            }

            Console.WriteLine("Parsing Ground Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.GroundTexture])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTexture, kvp.Key);
                GroundTexture gt = new GroundTexture();
                gt.PopulateFromStream(reader);
                groundTextures.Add(gt);
            }

            Console.WriteLine("Parsing Dungeon Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTexture])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTexture, kvp.Key);
                DungeonTexture dt = new DungeonTexture();
                dt.PopulateFromStream(reader);
                dungeonTextures.Add(dt);
            }

            Console.WriteLine("Parsing Skin Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.SkinTexture])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.SkinTexture, kvp.Key);
                SkinTexture st = new SkinTexture();
                st.PopulateFromStream(reader);
                skinTextures.Add(st);
            }

            Console.WriteLine("Parsing BodyTextureMedium Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.BodyTextureMedium])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.BodyTextureMedium, kvp.Key);
                BodyTextureMedium btm = new BodyTextureMedium();
                btm.PopulateFromStream(reader);
                btmTextures.Add(btm);
            }

            Console.WriteLine("Parsing BodyTextureSmall Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.BodyTextureSmall])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.BodyTextureSmall, kvp.Key);
                BodyTextureSmall bts = new BodyTextureSmall();
                bts.PopulateFromStream(reader);
                btsTextures.Add(bts);
            }

            Console.WriteLine("Parsing GroundTextureMedium Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.GroundTextureMedium])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTextureMedium, kvp.Key);
                GroundTextureMedium gtm = new GroundTextureMedium();
                gtm.PopulateFromStream(reader);
                gtmTextures.Add(gtm);
            }

            Console.WriteLine("Parsing GroundTextureSmall Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.GroundTextureSmall])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTextureSmall, kvp.Key);
                GroundTextureSmall gts = new GroundTextureSmall();
                gts.PopulateFromStream(reader);
                gtsTextures.Add(gts);
            }

            Console.WriteLine("Parsing DungeonTextureMedium Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTextureMedium])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTextureMedium, kvp.Key);
                DungeonTextureMedium dtm = new DungeonTextureMedium();
                dtm.PopulateFromStream(reader);
                dtmTextures.Add(dtm);
            }

            Console.WriteLine("Parsing DungeonTextureSmall Textures...");
            count = 0;
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTextureSmall])
            {
                if(count % 1000 == 0) { Console.WriteLine($"Parsed textures: {count}"); }
                count++;
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTextureSmall, kvp.Key);
                DungeonTextureSmall dts = new DungeonTextureSmall();
                dts.PopulateFromStream(reader);
                dtsTextures.Add(dts);
            }

            // BinaryReader reader = rdbController.Get((int)RdbRecordType.MinimapTextures, 6553601);
            
            

        
            Console.WriteLine(aoPath);
        }
    }
}


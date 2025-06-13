using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using AODb.Common;
using AODb.Data; // This is AODb.Data, not AODb.Core.Data

namespace AODb.Core
{
    public class DataProcessor
    {
        private readonly RdbController rdbController;
        private readonly string outputPath;

        private List<Item> Items = new List<Item>();
        private readonly object itemsLock = new object();
        private List<NanoProgram> Nanos = new List<NanoProgram>();
        private readonly object nanosLock = new object();
        private List<Icon> Icons = new List<Icon>();
        private readonly object iconsLock = new object();
        private List<Texture> Textures = new List<Texture>(); // For General Textures
        private readonly object texturesLock = new object();
        private List<Texture> DungeonTextures = new List<Texture>();
        private readonly object dungeonTexturesLock = new object();
        private List<Texture> GroundTextures = new List<Texture>();
        private readonly object groundTexturesLock = new object();
        private List<Texture> SkinTextures = new List<Texture>(); // For Body Textures (Full)
        private readonly object skinTexturesLock = new object();
        private List<Texture> BtmTextures = new List<Texture>(); // Body Texture Medium
        private readonly object btmTexturesLock = new object();
        private List<Texture> BtsTextures = new List<Texture>(); // Body Texture Small
        private readonly object btsTexturesLock = new object();
        private List<Texture> GtmTextures = new List<Texture>(); // Ground Texture Medium
        private readonly object gtmTexturesLock = new object();
        private List<Texture> GtsTextures = new List<Texture>(); // Ground Texture Small
        private readonly object gtsTexturesLock = new object();
        private List<Texture> DtmTextures = new List<Texture>(); // Dungeon Texture Medium
        private readonly object dtmTexturesLock = new object();
        private List<Texture> DtsTextures = new List<Texture>(); // Dungeon Texture Small
        private readonly object dtsTexturesLock = new object();

        // ModelData might be needed if any parsing/writing methods use it.
        // private ModelData ModelData { get; set; }

        public DataProcessor(RdbController rdbController, string outputPath)
        {
            this.rdbController = rdbController;
            this.outputPath = outputPath;
            Directory.CreateDirectory(this.outputPath); // Ensure output directory exists
        }

        public void ParseAllItems()
        {
            Console.WriteLine("Parsing Items...");
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.Item], kvp =>
            {
                try
                {
                    BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
                    Item item = new Item();
                    item.PopulateFromStream(reader);
                    lock (itemsLock)
                    {
                        this.Items.Add(item);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing item {kvp.Key}: {e.Message}");
                }
            });
        }

        public async Task WriteJsonItemsAsync()
        {
            string fileName = Path.Join(this.outputPath, "items.json");
            string jsonContent = JsonConvert.SerializeObject(this.Items, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, jsonContent);
        }

        public void ParseAllNanos()
        {
            Console.WriteLine("Parsing Nanos...");
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.NanoProgram], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.NanoProgram, kvp.Key);
                NanoProgram nano = new NanoProgram();
                nano.PopulateFromStream(reader);
                lock (nanosLock)
                {
                    this.Nanos.Add(nano);
                }
            });
        }

        public async Task WriteJsonNanosAsync()
        {
            string fileName = Path.Join(this.outputPath, "nanos.json");
            string jsonContent = JsonConvert.SerializeObject(this.Nanos, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, jsonContent);
        }

        public void ParseAllIcons()
        {
            Console.WriteLine("Parsing Icons...");
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.Icon], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Icon, kvp.Key);
                Icon icon = new Icon();
                icon.PopulateFromStream(reader);
                lock (iconsLock)
                {
                    this.Icons.Add(icon);
                }
            });
        }

        public async Task WriteIconsAsync()
        {
            string iconPath = Path.Join(this.outputPath, "Icons");
            Console.WriteLine($"Exporting icons to {iconPath}");
            DirectoryInfo iconDir = Directory.CreateDirectory(iconPath); // Ensure subdir exists
            foreach(Icon icon in this.Icons)
            {
                string fileName = Path.Join(iconDir.FullName, icon.AOID.ToString() + ".png");
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fs.WriteAsync(icon.PngData, 0, icon.PngData.Length);
                }
            }
        }

        public void ParseAllGeneralTextures()
        {
            Console.WriteLine("Parsing General Textures...");
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.Texture], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Texture, kvp.Key);
                Texture tex = new Texture();
                tex.PopulateFromStream(reader);
                lock (texturesLock)
                {
                    this.Textures.Add(tex);
                }
            });
        }

        public async Task WriteGeneralTexturesAsync()
        {
            await WriteTexturesAsync(this.Textures, "GeneralTextures", ".jpg");
        }

        public void ParseAllDungeonTextures()
        {
            Console.WriteLine("Parsing Dungeon Textures...");
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTexture], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTexture, kvp.Key);
                Texture dt = new Texture();
                dt.PopulateFromStream(reader);
                lock (dungeonTexturesLock)
                {
                    this.DungeonTextures.Add(dt);
                }
            });

            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTextureMedium], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTextureMedium, kvp.Key);
                Texture dtm = new Texture();
                dtm.PopulateFromStream(reader);
                lock (dtmTexturesLock)
                {
                    this.DtmTextures.Add(dtm);
                }
            });

            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTextureSmall], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTextureSmall, kvp.Key);
                Texture dts = new Texture();
                dts.PopulateFromStream(reader);
                lock (dtsTexturesLock)
                {
                    this.DtsTextures.Add(dts);
                }
            });
        }

        public async Task WriteDungeonTexturesAsync()
        {
            await WriteTexturesAsync(this.DungeonTextures, "DungeonTextures/Full", ".jpg");
            await WriteTexturesAsync(this.DtmTextures, "DungeonTextures/Medium", ".jpg");
            await WriteTexturesAsync(this.DtsTextures, "DungeonTextures/Small", ".jpg");
        }

        public void ParseAllGroundTextures()
        {
            Console.WriteLine("Parsing Ground Textures...");
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.GroundTexture], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTexture, kvp.Key);
                Texture gt = new Texture();
                gt.PopulateFromStream(reader);
                lock (groundTexturesLock)
                {
                    this.GroundTextures.Add(gt);
                }
            });

            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.GroundTextureMedium], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTextureMedium, kvp.Key);
                Texture gtm = new Texture();
                gtm.PopulateFromStream(reader);
                lock (gtmTexturesLock)
                {
                    this.GtmTextures.Add(gtm);
                }
            });

            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.GroundTextureSmall], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTextureSmall, kvp.Key);
                Texture gts = new Texture();
                gts.PopulateFromStream(reader);
                lock (gtsTexturesLock)
                {
                    this.GtsTextures.Add(gts);
                }
            });
        }

        public async Task WriteGroundTexturesAsync()
        {
            await WriteTexturesAsync(this.GroundTextures, "GroundTextures/Full", ".jpg");
            await WriteTexturesAsync(this.GtmTextures, "GroundTextures/Medium", ".jpg");
            await WriteTexturesAsync(this.GtsTextures, "GroundTextures/Small", ".jpg");
        }

        public void ParseAllBodyTextures()
        {
            Console.WriteLine("Parsing Skin Textures..."); // This was "Skin Textures" in original, keeping for consistency
            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.SkinTexture], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.SkinTexture, kvp.Key);
                Texture st = new Texture();
                st.PopulateFromStream(reader);
                lock (skinTexturesLock)
                {
                    this.SkinTextures.Add(st);
                }
            });

            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.BodyTextureMedium], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.BodyTextureMedium, kvp.Key);
                Texture btm = new Texture();
                btm.PopulateFromStream(reader);
                lock (btmTexturesLock)
                {
                    this.BtmTextures.Add(btm);
                }
            });

            Parallel.ForEach(rdbController.RecordTypeToId[(int)RdbRecordType.BodyTextureSmall], kvp =>
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.BodyTextureSmall, kvp.Key);
                Texture bts = new Texture();
                bts.PopulateFromStream(reader);
                lock (btsTexturesLock)
                {
                    this.BtsTextures.Add(bts);
                }
            });
        }

        public async Task WriteBodyTexturesAsync()
        {
            await WriteTexturesAsync(this.SkinTextures, "BodyTextures/Full", ".jpg");
            await WriteTexturesAsync(this.BtmTextures, "BodyTextures/Medium", ".jpg");
            await WriteTexturesAsync(this.BtsTextures, "BodyTextures/Small", ".jpg");
        }

        public async Task WriteTexturesAsync(List<Texture> images, string dirPath, string extension)
        {
            string texPath = Path.Join(this.outputPath, dirPath);
            Console.WriteLine($"Exporting textures to {texPath}"); // Changed "icons" to "textures" for clarity
            DirectoryInfo imgDir = Directory.CreateDirectory(texPath); // Ensure subdir exists
            foreach(Texture img in images)
            {
                string fileName;
                if(img.Name != null && img.Name.Length > 0)
                {
                    fileName = Path.Join(imgDir.FullName, img.Name + extension); // Used imgDir.FullName
                }
                else { fileName = Path.Join(imgDir.FullName, img.AOID.ToString() + extension); } // Used imgDir.FullName

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fs.WriteAsync(img.ImgData, 0, img.ImgData.Length);
                }
            }
        }
    }
}

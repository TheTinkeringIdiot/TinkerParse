using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using CommandLine;
using AODb.Common;
using AODb.Common.Attributes;
using AODb.Data;
using System.Text;

namespace TinkerParser
{
    internal class Options
    {
        [Option(Required = true, HelpText = "Path to the Anarchy Online folder (Often C:\\Funcom\\Anarchy Online)")]
        public string AoPath { get; set; }

        [Option('o', "output-dir", Default = ".", HelpText = "Folder to place parsed data files")]
        public string OutputDir { get; set; }

        [Option("output-json", Default = true, HelpText = "Output item/nano data as JSON (Default)")]
        public bool OutputJson { get; set; }

        [Option("items", Default = false, HelpText = "Parse all items")]
        public bool Items { get; set; }

        [Option("nanos", Default = false, HelpText = "Parse all nano programs")]
        public bool Nanos { get; set; }

        [Option("icons", Default = false, HelpText = "Export all game icons")]
        public bool Icons { get; set; }

        [Option("dungeon-textures", Default = false, HelpText = "Export dungeon textures (including medium and small)")]
        public bool DungeonTextures { get; set; }

        [Option("ground-textures", Default = false, HelpText = "Export ground textures (including medium and small)")]
        public bool GroundTextures { get; set; }

        [Option("body-textures", Default = false, HelpText = "Export body textures (including medium and small)")]
        public bool BodyTextures { get; set; }

        [Option("textures", Default = false, HelpText = "Export general game textures")]
        public bool Textures { get; set; }
    }
    internal class TinkerParser
    {
        private RdbController rdbController;

        private string outputPath;

        private List<Item> Items = new List<Item>();
        private List<NanoProgram> Nanos = new List<NanoProgram>();
        private List<Icon> Icons = new List<Icon>();
        private List<Texture> Textures = new List<Texture>();
        private List<Texture> DungeonTextures = new List<Texture>();
        private List<Texture> GroundTextures = new List<Texture>();
        private List<Texture> SkinTextures = new List<Texture>();
        private List<Texture> BtmTextures = new List<Texture>();
        private List<Texture> BtsTextures = new List<Texture>();
        private List<Texture> GtmTextures = new List<Texture>();
        private List<Texture> GtsTextures = new List<Texture>();
        private List<Texture> DtmTextures = new List<Texture>();
        private List<Texture> DtsTextures = new List<Texture>();

        public TinkerParser(string aoPath, string outputPath)
        {
            Directory.SetCurrentDirectory(aoPath);
            this.rdbController = new RdbController(aoPath);
            this.outputPath = outputPath;
        }

        public void ParseAllItems()
        {
            Console.WriteLine("Parsing Items...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Item])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
                Item item = new Item();
                item.PopulateFromStream(reader);
                this.Items.Add(item);
            }
        }

        public void ParseAllNanos()
        {
            Console.WriteLine("Parsing Nanos...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.NanoProgram])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.NanoProgram, kvp.Key);
                NanoProgram nano = new NanoProgram();
                nano.PopulateFromStream(reader);
                this.Nanos.Add(nano);
            }
        }

        public void ParseAllIcons()
        {
            Console.WriteLine("Parsing Icons...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Icon])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Icon, kvp.Key);
                Icon icon = new Icon();
                icon.PopulateFromStream(reader);
                this.Icons.Add(icon);
            }
        }

        public void WriteIcons()
        {
            string iconPath = Path.Join(this.outputPath, "Icons");
            Console.WriteLine($"Exporting icons to {iconPath}");
            DirectoryInfo iconDir = Directory.CreateDirectory(iconPath);
            foreach(Icon icon in this.Icons)
            {
                string fileName = Path.Join(iconDir.FullName, icon.AOID.ToString() + ".png");
                FileStream fs = new FileStream(fileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(icon.PngData);
                bw.Close();
                fs.Close();
            }
        }

        public void ParseAllGeneralTextures()
        {
            Console.WriteLine("Parsing General Textures...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Texture])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.Texture, kvp.Key);
                Texture tex = new Texture();
                tex.PopulateFromStream(reader);
                this.Textures.Add(tex);
            }
        }

        public void WriteGeneralTextures()
        {
            WriteTextures(this.Textures, "GeneralTextures", ".jpg");
        }

        public void ParseAllDungeonTextures()
        {
            Console.WriteLine("Parsing Dungeon Textures...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTexture])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTexture, kvp.Key);
                Texture dt = new Texture();
                dt.PopulateFromStream(reader);
                this.DungeonTextures.Add(dt);
            }

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTextureMedium])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTextureMedium, kvp.Key);
                Texture dtm = new Texture();
                dtm.PopulateFromStream(reader);
                this.DtmTextures.Add(dtm);
            }

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.DungeonTextureSmall])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.DungeonTextureSmall, kvp.Key);
                Texture dts = new Texture();
                dts.PopulateFromStream(reader);
                this.DtsTextures.Add(dts);
            }
        }

        public void WriteDungeonTextures()
        {
            WriteTextures(this.DungeonTextures, "DungeonTextures/Full", ".jpg");
            WriteTextures(this.DtmTextures, "DungeonTextures/Medium", ".jpg");
            WriteTextures(this.DtsTextures, "DungeonTextures/Small", ".jpg");
        }

        public void ParseAllGroundTextures()
        {
            Console.WriteLine("Parsing Ground Textures...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.GroundTexture])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTexture, kvp.Key);
                Texture gt = new Texture();
                gt.PopulateFromStream(reader);
                this.GroundTextures.Add(gt);
            }

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.GroundTextureMedium])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTextureMedium, kvp.Key);
                Texture gtm = new Texture();
                gtm.PopulateFromStream(reader);
                this.GtmTextures.Add(gtm);
            }

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.GroundTextureSmall])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.GroundTextureSmall, kvp.Key);
                Texture gts = new Texture();
                gts.PopulateFromStream(reader);
                this.GtsTextures.Add(gts);
            }
        }

        public void WriteGroundTextures()
        {
            WriteTextures(this.GroundTextures, "GroundTextures/Full", ".jpg");
            WriteTextures(this.GtmTextures, "GroundTextures/Medium", ".jpg");
            WriteTextures(this.GtsTextures, "GroundTextures/Small", ".jpg");
        }

        public void ParseAllBodyTextures()
        {
            Console.WriteLine("Parsing Skin Textures...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.SkinTexture])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.SkinTexture, kvp.Key);
                Texture st = new Texture();
                st.PopulateFromStream(reader);
                this.SkinTextures.Add(st);
            }

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.BodyTextureMedium])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.BodyTextureMedium, kvp.Key);
                Texture btm = new Texture();
                btm.PopulateFromStream(reader);
                this.BtmTextures.Add(btm);
            }

            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.BodyTextureSmall])
            {
                BinaryReader reader = rdbController.Get((int)RdbRecordType.BodyTextureSmall, kvp.Key);
                Texture bts = new Texture();
                bts.PopulateFromStream(reader);
                this.BtsTextures.Add(bts);
            }
        }

        public void WriteBodyTextures()
        {
            WriteTextures(this.SkinTextures, "BodyTextures/Full", ".jpg");
            WriteTextures(this.BtmTextures, "BodyTextures/Medium", ".jpg");
            WriteTextures(this.BtsTextures, "BodyTextures/Small", ".jpg");
        }

        public void WriteTextures(List<Texture> images, string dirPath, string extension)
        {
            string texPath = Path.Join(this.outputPath, dirPath);
            Console.WriteLine($"Exporting icons to {texPath}");
            DirectoryInfo imgDir = Directory.CreateDirectory(texPath);
            foreach(Texture img in images)
            {
                string fileName;
                if(img.Name != null && img.Name.Length > 0)
                {
                    fileName = Path.Join(texPath, img.Name + extension);
                }
                else { fileName = Path.Join(texPath, img.AOID.ToString() + extension); } 

                FileStream fs = new FileStream(fileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(img.ImgData);
                bw.Close();
                fs.Close();
            }
        }




        static void Main(string[] args)
        {

            var parsedOpts = Parser.Default.ParseArguments<Options>(args);
            Options opts = parsedOpts.Value;

            TinkerParser tinkerParser = new TinkerParser(opts.AoPath, opts.OutputDir);

            if(opts.Items) { tinkerParser.ParseAllItems(); }
            if(opts.Nanos) { tinkerParser.ParseAllNanos(); }
            if(opts.Icons) 
            { 
                tinkerParser.ParseAllIcons();
                tinkerParser.WriteIcons(); 
            }
            if(opts.Textures) 
            { 
                tinkerParser.ParseAllGeneralTextures(); 
                tinkerParser.WriteGeneralTextures();
            }
            if(opts.DungeonTextures) 
            { 
                tinkerParser.ParseAllDungeonTextures(); 
                tinkerParser.WriteDungeonTextures();
            }
            if(opts.GroundTextures) 
            { 
                tinkerParser.ParseAllGroundTextures(); 
                tinkerParser.WriteGroundTextures();
            }
            if(opts.BodyTextures) 
            { 
                tinkerParser.ParseAllBodyTextures(); 
                tinkerParser.WriteBodyTextures();
            }

            // BinaryReader reader = rdbController.Get((int)RdbRecordType.MinimapTextures, 6553601);

            // BinaryReader reader = rdbController.Get((int)RdbRecordType.NanoProgram, 76724);
            // NanoProgram item = new NanoProgram();
            // item.PopulateFromStream(reader);

            // string jsontest = JsonConvert.SerializeObject(item, Formatting.Indented);
            
            // Console.WriteLine(jsontest);
            // Console.WriteLine(jsontest);

        }
    }
}


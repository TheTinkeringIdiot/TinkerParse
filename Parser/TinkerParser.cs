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
using System.Text.RegularExpressions;

namespace TinkerParser
{
    internal class Options
    {
        [Option(Required = true, HelpText = "Path to the Anarchy Online folder (Often C:\\Funcom\\Anarchy Online)")]
        public string AoPath { get; set; }

        [Option("prk", Default = false, HelpText = "Use the Project Rubika client database")]
        public bool Prk { get; set; }

        [Option("client-version", Default = null, HelpText = "Client build the database came from (e.g. 14.9.1); selects pre-15.0 record formats. Omit for current clients")]
        public string ClientVersion { get; set; }

        [Option('o', "output-dir", Default = ".", HelpText = "Folder to place parsed data files")]
        public string OutputDir { get; set; }

        [Option("output-json", Default = true, HelpText = "Output item/nano data as JSON (Default)")]
        public bool OutputJson { get; set; }

        [Option("errors-file", Default = null, HelpText = "Write per-record parse errors as JSON to this file")]
        public string ErrorsFile { get; set; }

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
    internal class ParseError
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("aoid")]
        public int Aoid { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    internal class TinkerParser
    {
        private RdbController rdbController;

        private string outputPath;

        private List<Item> Items = new List<Item>();
        private List<NanoProgram> Nanos = new List<NanoProgram>();
        private List<ParseError> ParseErrors = new List<ParseError>();
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

        private ModelData ModelData { get; set; }

        // Item/nano records are XOR-obfuscated up to 13.91 (adbd); shop hash levels are byte-only before 15.0
        private static readonly Version FirstPlainRecords = new Version(14, 0);
        private static readonly Version FirstWideShopLevels = new Version(15, 0);

        private readonly bool shopHashByteLevels;

        public TinkerParser(string aoPath, string outputPath, bool isPrk, Version clientVersion)
        {
            Directory.SetCurrentDirectory(aoPath);
            bool decodeLegacy = clientVersion != null && clientVersion < FirstPlainRecords;
            this.shopHashByteLevels = clientVersion != null && clientVersion < FirstWideShopLevels;
            this.rdbController = new RdbController(aoPath, isPrk, decodeLegacy);
            this.outputPath = outputPath;
        }

        public static Version ParseClientVersion(string value)
        {
            if (value == null)
                return null;

            Match match = Regex.Match(value, @"^\d+(\.\d+){0,3}");
            if (!match.Success)
                throw new ArgumentException($"Unrecognised client version '{value}'");

            string numeric = match.Value.Contains('.') ? match.Value : match.Value + ".0";
            return Version.Parse(numeric);
        }

        public void ParseAllItems()
        {
            Console.WriteLine("Parsing Items...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.Item])
            {
                try
                {
                    BinaryReader reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
                    Item item = new Item();
                    item.PopulateFromStream(reader, this.shopHashByteLevels);
                    this.Items.Add(item);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing item {kvp.Key}: {e.Message}");
                    this.ParseErrors.Add(new ParseError { Kind = "item", Aoid = kvp.Key, Error = $"{e.GetType().Name}: {e.Message}" });
                }
            }
        }

        public void WriteJsonItems()
        {
            string fileName = Path.Join(this.outputPath, "items.json");
            using(StreamWriter file = File.CreateText(fileName))
            {
                file.Write(JsonConvert.SerializeObject(this.Items, Formatting.Indented));
            }
        }

        public void ParseAllNanos()
        {
            Console.WriteLine("Parsing Nanos...");
            foreach(KeyValuePair<int, ulong> kvp in rdbController.RecordTypeToId[(int)RdbRecordType.NanoProgram])
            {
                try
                {
                    BinaryReader reader = rdbController.Get((int)RdbRecordType.NanoProgram, kvp.Key);
                    NanoProgram nano = new NanoProgram();
                    nano.PopulateFromStream(reader, this.shopHashByteLevels);
                    this.Nanos.Add(nano);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing nano {kvp.Key}: {e.Message}");
                    this.ParseErrors.Add(new ParseError { Kind = "nano", Aoid = kvp.Key, Error = $"{e.GetType().Name}: {e.Message}" });
                }

                // foreach (StatValue sv in nano.StatValues)
                // {
                //     if (sv.Stat == Stat.None)
                //     {
                //         bool isHostile = (sv.RawValue & (int)Math.Pow(2, 15)) != 0;
                //         bool isBuff = (sv.RawValue & (int)Math.Pow(2, 16)) != 0;
                //         if (isHostile && !isBuff)
                //         {
                //             Console.WriteLine($"{nano.AOID} - {nano.Name}");
                //         }
                //     }
                // }
            }
        }

        public void WriteJsonNanos()
        {
            string fileName = Path.Join(this.outputPath, "nanos.json");
            using(StreamWriter file = File.CreateText(fileName))
            {
                file.Write(JsonConvert.SerializeObject(this.Nanos, Formatting.Indented));
            }
        }

        public void WriteParseErrors(string fileName)
        {
            using(StreamWriter file = File.CreateText(fileName))
            {
                file.Write(JsonConvert.SerializeObject(this.ParseErrors, Formatting.Indented));
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

            TinkerParser tinkerParser = new TinkerParser(opts.AoPath, opts.OutputDir, opts.Prk, TinkerParser.ParseClientVersion(opts.ClientVersion));

            if(opts.Items) 
            { 
                tinkerParser.ParseAllItems(); 
                if(opts.OutputJson) { tinkerParser.WriteJsonItems(); }
            }
            if(opts.Nanos) 
            { 
                tinkerParser.ParseAllNanos();
                if(opts.OutputJson) { tinkerParser.WriteJsonNanos(); } 
            }
            if(opts.ErrorsFile != null)
            {
                tinkerParser.WriteParseErrors(opts.ErrorsFile);
            }
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
        }
    }
}


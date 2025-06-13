using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using CommandLine;
using AODb.Common;
// using AODb.Common.Attributes; // No longer directly used here
// using AODb.Data; // No longer directly used here
using System.Text;
// using System.Collections.Concurrent; // No longer directly used here
using System.Threading.Tasks;
using AODb.Core; // Added for DataProcessor

namespace TinkerParser
{
    internal class Options
    {
        [Option(Required = true, HelpText = "Path to the Anarchy Online folder (Often C:\\Funcom\\Anarchy Online)")]
        public string AoPath { get; set; }

        [Option("prk", Default = false, HelpText = "Use the Project Rubika client database")]
        public bool Prk { get; set; }

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
        // All parsing methods, data lists, and lock objects have been moved to AODb.Core.DataProcessor

        // The RdbController and outputPath will be passed to DataProcessor via Main
        // private RdbController rdbController;
        // private string outputPath;

        // Constructor is simplified or might be removed if Main handles everything
        public TinkerParser() // string aoPath, string outputPath, bool isPrk
        {
            // Initialization logic will now be in Main, creating RdbController and DataProcessor
            // Directory.SetCurrentDirectory(aoPath);
            // this.rdbController = new RdbController(aoPath, isPrk);
            // this.outputPath = outputPath;
        }

        static async Task Main(string[] args)
        {
            var parsedOpts = Parser.Default.ParseArguments<Options>(args);
            Options opts = parsedOpts.Value;

            // Ensure AO Path is set correctly for RdbController
            Directory.SetCurrentDirectory(opts.AoPath);
            RdbController rdbController = new RdbController(opts.AoPath, opts.Prk);
            DataProcessor dataProcessor = new DataProcessor(rdbController, opts.OutputDir);

            if(opts.Items)
            {
                dataProcessor.ParseAllItems();
                if(opts.OutputJson) { await dataProcessor.WriteJsonItemsAsync(); }
            }
            if(opts.Nanos)
            {
                dataProcessor.ParseAllNanos();
                if(opts.OutputJson) { await dataProcessor.WriteJsonNanosAsync(); }
            }
            if(opts.Icons)
            {
                dataProcessor.ParseAllIcons();
                await dataProcessor.WriteIconsAsync();
            }
            if(opts.Textures)
            {
                dataProcessor.ParseAllGeneralTextures();
                await dataProcessor.WriteGeneralTexturesAsync();
            }
            if(opts.DungeonTextures)
            {
                dataProcessor.ParseAllDungeonTextures();
                await dataProcessor.WriteDungeonTexturesAsync();
            }
            if(opts.GroundTextures)
            {
                dataProcessor.ParseAllGroundTextures();
                await dataProcessor.WriteGroundTexturesAsync();
            }
            if(opts.BodyTextures)
            {
                dataProcessor.ParseAllBodyTextures();
                await dataProcessor.WriteBodyTexturesAsync();
            }
        }
    }
}

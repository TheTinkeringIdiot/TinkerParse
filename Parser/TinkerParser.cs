using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using CommandLine;
using AODb.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AODb.Core;
using Serilog; // Added for Serilog

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

        [Option("max-dop", HelpText = "Maximum degree of parallelism for parsing operations. Defaults to processor count.")]
        public int? MaxDegreeOfParallelism { get; set; } // Nullable to detect if user provided it
    }
    internal class TinkerParser
    {
        public TinkerParser()
        {
        }

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/aodb_parser_.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // Added retainedFileCountLimit
                .CreateLogger();

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                Log.Warning("Cancellation requested via Ctrl+C.");
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
                e.Cancel = true;
            };

            try
            {
                Log.Information("Application starting with arguments: {Args}", string.Join(" ", args));

                var parsedOpts = Parser.Default.ParseArguments<Options>(args);
                Options opts = parsedOpts.Value;

                if (opts == null)
                {
                    Log.Error("Error parsing command line arguments. Use --help for usage information.");
                    // CommandLineParser usually handles help output on parsing failure.
                    return;
                }

                Directory.SetCurrentDirectory(opts.AoPath);
                RdbController rdbController = new RdbController(opts.AoPath, opts.Prk);
                DataProcessor dataProcessor = new DataProcessor(rdbController, opts.OutputDir, opts.MaxDegreeOfParallelism);

                Log.Information("Starting processing... Press Ctrl+C to cancel.");

                if (opts.Items)
                {
                    Log.Information("Processing Items selected.");
                    dataProcessor.ParseAllItems(cts.Token);
                    if (opts.OutputJson) { await dataProcessor.WriteJsonItemsAsync(cts.Token); }
                }
                if (opts.Nanos)
                {
                    Log.Information("Processing Nanos selected.");
                    dataProcessor.ParseAllNanos(cts.Token);
                    if (opts.OutputJson) { await dataProcessor.WriteJsonNanosAsync(cts.Token); }
                }
                if (opts.Icons)
                {
                    Log.Information("Processing Icons selected.");
                    dataProcessor.ParseAllIcons(cts.Token);
                    await dataProcessor.WriteIconsAsync(cts.Token);
                }
                if (opts.Textures)
                {
                    Log.Information("Processing General Textures selected.");
                    dataProcessor.ParseAllGeneralTextures(cts.Token);
                    await dataProcessor.WriteGeneralTexturesAsync(cts.Token);
                }
                if (opts.DungeonTextures)
                {
                    Log.Information("Processing Dungeon Textures selected.");
                    dataProcessor.ParseAllDungeonTextures(cts.Token);
                    await dataProcessor.WriteDungeonTexturesAsync(cts.Token);
                }
                if (opts.GroundTextures)
                {
                    Log.Information("Processing Ground Textures selected.");
                    dataProcessor.ParseAllGroundTextures(cts.Token);
                    await dataProcessor.WriteGroundTexturesAsync(cts.Token);
                }
                if (opts.BodyTextures)
                {
                    Log.Information("Processing Body Textures selected.");
                    dataProcessor.ParseAllBodyTextures(cts.Token);
                    await dataProcessor.WriteBodyTexturesAsync(cts.Token);
                }
                Log.Information("Processing finished successfully.");
            }
            catch (OperationCanceledException)
            {
                Log.Warning("Operation was canceled.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.Information("Application shutting down.");
                Log.CloseAndFlush();
                cts.Dispose();
            }
        }
    }
}

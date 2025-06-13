using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using AODb.Common;
using AODb.Data;
using Serilog;

namespace AODb.Core
{
    public class DataProcessor
    {
        private readonly RdbController rdbController;
        private readonly string outputPath;
        private readonly object rdbGetLock = new object();
        private readonly int? _maxDegreeOfParallelism; // Added field

        private List<Item> Items = new List<Item>();
        private readonly object itemsLock = new object();
        private List<NanoProgram> Nanos = new List<NanoProgram>();
        private readonly object nanosLock = new object();
        private List<Icon> IconsList = new List<Icon>();
        private readonly object iconsListLock = new object();
        private List<Texture> Textures = new List<Texture>();
        private readonly object texturesLock = new object();
        private List<Texture> DungeonTextures = new List<Texture>();
        private readonly object dungeonTexturesLock = new object();
        private List<Texture> GroundTextures = new List<Texture>();
        private readonly object groundTexturesLock = new object();
        private List<Texture> SkinTextures = new List<Texture>();
        private readonly object skinTexturesLock = new object();
        private List<Texture> BtmTextures = new List<Texture>();
        private readonly object btmTexturesLock = new object();
        private List<Texture> BtsTextures = new List<Texture>();
        private readonly object btsTexturesLock = new object();
        private List<Texture> GtmTextures = new List<Texture>();
        private readonly object gtmTexturesLock = new object();
        private List<Texture> GtsTextures = new List<Texture>();
        private readonly object gtsTexturesLock = new object();
        private List<Texture> DtmTextures = new List<Texture>();
        private readonly object dtmTexturesLock = new object();
        private List<Texture> DtsTextures = new List<Texture>();
        private readonly object dtsTexturesLock = new object();

        public DataProcessor(RdbController rdbController, string outputPath, int? maxDegreeOfParallelism) // Constructor updated
        {
            this.rdbController = rdbController;
            this.outputPath = outputPath;
            this._maxDegreeOfParallelism = maxDegreeOfParallelism; // Store the value
            Directory.CreateDirectory(this.outputPath);
        }

        private ParallelOptions GetConfiguredParallelOptions(CancellationToken cancellationToken)
        {
            var parallelOptions = new ParallelOptions { CancellationToken = cancellationToken };
            if (_maxDegreeOfParallelism.HasValue && _maxDegreeOfParallelism.Value > 0)
            {
                parallelOptions.MaxDegreeOfParallelism = _maxDegreeOfParallelism.Value;
                Log.Debug("Using MaxDegreeOfParallelism: {MaxDop}", _maxDegreeOfParallelism.Value);
            }
            else
            {
                Log.Debug("Using default MaxDegreeOfParallelism (ProcessorCount).");
            }
            return parallelOptions;
        }

        public void ParseAllItems(CancellationToken cancellationToken = default)
        {
            var itemMap = rdbController.RecordTypeToId[(int)RdbRecordType.Item];
            int totalCount = itemMap.Count;
            int processedCount = 0;
            Log.Information("Starting to parse {ItemCount} Items...", totalCount);

            var parallelOptions = GetConfiguredParallelOptions(cancellationToken); // Use helper
            Parallel.ForEach(itemMap, parallelOptions, kvp =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                BinaryReader reader;
                lock (rdbGetLock)
                {
                    reader = rdbController.Get((int)RdbRecordType.Item, kvp.Key);
                }
                if (reader == null) return;

                try
                {
                    Item item = new Item();
                    item.PopulateFromStream(reader);
                    lock (itemsLock)
                    {
                        this.Items.Add(item);
                    }
                    int currentProcessed = Interlocked.Increment(ref processedCount);
                    if (currentProcessed % 100 == 0 || currentProcessed == totalCount)
                    {
                        Log.Debug("Parsed {ProcessedCount}/{TotalCount} Items...", currentProcessed, totalCount);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error parsing item {ItemId} of type {ItemType}", kvp.Key, RdbRecordType.Item);
                }
            });
            Log.Information("Finished parsing {ProcessedCount}/{TotalCount} Items.", processedCount, totalCount);
        }

        public async Task WriteJsonItemsAsync(CancellationToken cancellationToken = default)
        {
            Log.Information("Starting to write {ItemCount} items to JSON...", Items.Count);
            string fileName = Path.Join(this.outputPath, "items.json");
            string jsonContent = JsonConvert.SerializeObject(this.Items, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, jsonContent, cancellationToken);
            Log.Information("Finished writing items to JSON at {FileName}.", fileName);
        }

        public void ParseAllNanos(CancellationToken cancellationToken = default)
        {
            var nanoMap = rdbController.RecordTypeToId[(int)RdbRecordType.NanoProgram];
            int totalCount = nanoMap.Count;
            int processedCount = 0;
            Log.Information("Starting to parse {NanoCount} Nanos...", totalCount);

            var parallelOptions = GetConfiguredParallelOptions(cancellationToken); // Use helper
            Parallel.ForEach(nanoMap, parallelOptions, kvp =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                BinaryReader reader;
                lock (rdbGetLock)
                {
                    reader = rdbController.Get((int)RdbRecordType.NanoProgram, kvp.Key);
                }
                if (reader == null) return;

                NanoProgram nano = new NanoProgram();
                nano.PopulateFromStream(reader);
                lock (nanosLock)
                {
                    this.Nanos.Add(nano);
                }
                int currentProcessed = Interlocked.Increment(ref processedCount);
                if (currentProcessed % 100 == 0 || currentProcessed == totalCount)
                {
                    Log.Debug("Parsed {ProcessedCount}/{TotalCount} Nanos...", currentProcessed, totalCount);
                }
            });
            Log.Information("Finished parsing {ProcessedCount}/{TotalCount} Nanos.", processedCount, totalCount);
        }

        public async Task WriteJsonNanosAsync(CancellationToken cancellationToken = default)
        {
            Log.Information("Starting to write {NanoCount} nanos to JSON...", Nanos.Count);
            string fileName = Path.Join(this.outputPath, "nanos.json");
            string jsonContent = JsonConvert.SerializeObject(this.Nanos, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, jsonContent, cancellationToken);
            Log.Information("Finished writing nanos to JSON at {FileName}.", fileName);
        }

        public void ParseAllIcons(CancellationToken cancellationToken = default)
        {
            var iconMap = rdbController.RecordTypeToId[(int)RdbRecordType.Icon];
            int totalCount = iconMap.Count;
            int processedCount = 0;
            Log.Information("Starting to parse {IconCount} Icons...", totalCount);

            var parallelOptions = GetConfiguredParallelOptions(cancellationToken); // Use helper
            Parallel.ForEach(iconMap, parallelOptions, kvp =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                BinaryReader reader;
                lock (rdbGetLock)
                {
                    reader = rdbController.Get((int)RdbRecordType.Icon, kvp.Key);
                }
                if (reader == null) return;

                Icon iconData = new Icon();
                iconData.PopulateFromStream(reader);
                lock (iconsListLock)
                {
                    this.IconsList.Add(iconData);
                }
                int currentProcessed = Interlocked.Increment(ref processedCount);
                if (currentProcessed % 100 == 0 || currentProcessed == totalCount)
                {
                    Log.Debug("Parsed {ProcessedCount}/{TotalCount} Icons...", currentProcessed, totalCount);
                }
            });
            Log.Information("Finished parsing {ProcessedCount}/{TotalCount} Icons.", processedCount, totalCount);
        }

        public async Task WriteIconsAsync(CancellationToken cancellationToken = default)
        {
            string iconPath = Path.Join(this.outputPath, "Icons");
            Log.Information("Starting to export {IconCount} icons to {IconPath}...", IconsList.Count, iconPath);
            DirectoryInfo iconDir = Directory.CreateDirectory(iconPath);
            int writtenCount = 0;
            int totalIcons = IconsList.Count;
            foreach(Icon iconData in this.IconsList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string fileName = Path.Join(iconDir.FullName, iconData.AOID.ToString() + ".png");
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await fs.WriteAsync(iconData.PngData, 0, iconData.PngData.Length, cancellationToken);
                }
                writtenCount++;
                if (writtenCount % 100 == 0 || writtenCount == totalIcons)
                {
                    Log.Debug("Written {WrittenCount}/{TotalIcons} icons...", writtenCount, totalIcons);
                }
            }
            Log.Information("Finished writing {WrittenCount}/{TotalIcons} icons to {IconPath}.", writtenCount, totalIcons, iconPath);
        }

        public void ParseAllGeneralTextures(CancellationToken cancellationToken = default)
        {
            var textureMap = rdbController.RecordTypeToId[(int)RdbRecordType.Texture];
            int totalCount = textureMap.Count;
            int processedCount = 0;
            Log.Information("Starting to parse {TextureCount} General Textures...", totalCount);

            var parallelOptions = GetConfiguredParallelOptions(cancellationToken); // Use helper
            Parallel.ForEach(textureMap, parallelOptions, kvp =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                BinaryReader reader;
                lock (rdbGetLock)
                {
                    reader = rdbController.Get((int)RdbRecordType.Texture, kvp.Key);
                }
                if (reader == null) return;

                Texture tex = new Texture();
                tex.PopulateFromStream(reader);
                lock (texturesLock)
                {
                    this.Textures.Add(tex);
                }
                int currentProcessed = Interlocked.Increment(ref processedCount);
                if (currentProcessed % 50 == 0 || currentProcessed == totalCount)
                {
                    Log.Debug("Parsed {ProcessedCount}/{TotalCount} General Textures...", currentProcessed, totalCount);
                }
            });
            Log.Information("Finished parsing {ProcessedCount}/{TotalCount} General Textures.", processedCount, totalCount);
        }

        public async Task WriteGeneralTexturesAsync(CancellationToken cancellationToken = default)
        {
            await WriteTexturesAsync(this.Textures, "GeneralTextures", ".jpg", cancellationToken);
        }

        private void ProcessTextureType(RdbRecordType recordType, string textureTypeName, List<Texture> textureList, object listLock, CancellationToken cancellationToken)
        {
            var parallelOptions = GetConfiguredParallelOptions(cancellationToken); // Use helper
            var textureMap = rdbController.RecordTypeToId[(int)recordType];
            int totalCount = textureMap.Count;
            int processedCount = 0;
            Log.Information("Starting to parse {TextureCount} {TextureTypeName} Textures...", totalCount, textureTypeName);

            if (totalCount == 0)
            {
                Log.Information("No {TextureTypeName} Textures to parse.", textureTypeName);
                return;
            }

            Parallel.ForEach(textureMap, parallelOptions, kvp => {
                cancellationToken.ThrowIfCancellationRequested();
                BinaryReader reader;
                lock (rdbGetLock) { reader = rdbController.Get((int)recordType, kvp.Key); }
                if (reader == null) return;
                Texture tex = new Texture();
                tex.PopulateFromStream(reader);
                lock (listLock) { textureList.Add(tex); }
                int current = Interlocked.Increment(ref processedCount);
                if (current % 50 == 0 || current == totalCount) Log.Debug("Parsed {ProcessedCount}/{TotalCount} {TextureTypeName} Textures...", current, totalCount, textureTypeName);
            });
            Log.Information("Finished parsing {ProcessedCount}/{TotalCount} {TextureTypeName} Textures.", processedCount, totalCount, textureTypeName);
        }

        public void ParseAllDungeonTextures(CancellationToken cancellationToken = default)
        {
            ProcessTextureType(RdbRecordType.DungeonTexture, "Full Dungeon", DungeonTextures, dungeonTexturesLock, cancellationToken);
            ProcessTextureType(RdbRecordType.DungeonTextureMedium, "Medium Dungeon", DtmTextures, dtmTexturesLock, cancellationToken);
            ProcessTextureType(RdbRecordType.DungeonTextureSmall, "Small Dungeon", DtsTextures, dtsTexturesLock, cancellationToken);
        }

        public async Task WriteDungeonTexturesAsync(CancellationToken cancellationToken = default)
        {
            await WriteTexturesAsync(this.DungeonTextures, "DungeonTextures/Full", ".jpg", cancellationToken);
            await WriteTexturesAsync(this.DtmTextures, "DungeonTextures/Medium", ".jpg", cancellationToken);
            await WriteTexturesAsync(this.DtsTextures, "DungeonTextures/Small", ".jpg", cancellationToken);
        }

        public void ParseAllGroundTextures(CancellationToken cancellationToken = default)
        {
            ProcessTextureType(RdbRecordType.GroundTexture, "Full Ground", GroundTextures, groundTexturesLock, cancellationToken);
            ProcessTextureType(RdbRecordType.GroundTextureMedium, "Medium Ground", GtmTextures, gtmTexturesLock, cancellationToken);
            ProcessTextureType(RdbRecordType.GroundTextureSmall, "Small Ground", GtsTextures, gtsTexturesLock, cancellationToken);
        }

        public async Task WriteGroundTexturesAsync(CancellationToken cancellationToken = default)
        {
            await WriteTexturesAsync(this.GroundTextures, "GroundTextures/Full", ".jpg", cancellationToken);
            await WriteTexturesAsync(this.GtmTextures, "GroundTextures/Medium", ".jpg", cancellationToken);
            await WriteTexturesAsync(this.GtsTextures, "GroundTextures/Small", ".jpg", cancellationToken);
        }

        public void ParseAllBodyTextures(CancellationToken cancellationToken = default)
        {
            ProcessTextureType(RdbRecordType.SkinTexture, "Full Body", SkinTextures, skinTexturesLock, cancellationToken);
            ProcessTextureType(RdbRecordType.BodyTextureMedium, "Medium Body", BtmTextures, btmTexturesLock, cancellationToken);
            ProcessTextureType(RdbRecordType.BodyTextureSmall, "Small Body", BtsTextures, btsTexturesLock, cancellationToken);
        }

        public async Task WriteBodyTexturesAsync(CancellationToken cancellationToken = default)
        {
            await WriteTexturesAsync(this.SkinTextures, "BodyTextures/Full", ".jpg", cancellationToken);
            await WriteTexturesAsync(this.BtmTextures, "BodyTextures/Medium", ".jpg", cancellationToken);
            await WriteTexturesAsync(this.BtsTextures, "BodyTextures/Small", ".jpg", cancellationToken);
        }

        public async Task WriteTexturesAsync(List<Texture> images, string dirPath, string extension, CancellationToken cancellationToken = default)
        {
            string texPath = Path.Join(this.outputPath, dirPath);
            DirectoryInfo imgDir = Directory.CreateDirectory(texPath);
            int writtenCount = 0;
            int totalImages = images.Count;

            if (totalImages == 0)
            {
                Log.Information("No textures to write for {DirectoryPath}.", dirPath);
                return;
            }
            Log.Information("Starting to write {ImageCount} textures to {DirectoryPath}...", totalImages, dirPath);

            foreach(Texture img in images)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string fileName;
                if(img.Name != null && img.Name.Length > 0)
                {
                    fileName = Path.Join(imgDir.FullName, img.Name + extension);
                }
                else { fileName = Path.Join(imgDir.FullName, img.AOID.ToString() + extension); }

                try
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        await fs.WriteAsync(img.ImgData, 0, img.ImgData.Length, cancellationToken);
                    }
                    writtenCount++;
                    if (writtenCount % 50 == 0 || writtenCount == totalImages)
                    {
                        Log.Debug("Written {WrittenCount}/{TotalImages} textures to {DirectoryPath}...", writtenCount, totalImages, dirPath);
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "Failed to write texture {TextureId} to {FileName}", img.AOID, fileName);
                }
            }
            Log.Information("Finished writing {WrittenCount}/{TotalImages} textures to {DirectoryPath}.", writtenCount, totalImages, dirPath);
        }
    }
}

/*
 *Description: SevenZipSharpHelper
 *Author: Chance.zheng
 *Creat Time: 2023/11/26 13:40:49
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using DryIoc.ImTools;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ZstdSharp;

namespace CompressTool
{
    public class SevenZipSharpHelper : HelperBase
    {
        public event UpdateSingleFile UpdateSingleFileHandler;
        public event UpdateTotalPercent UpdateTotalPercentHandler;
        public event UpdateResult UpdateResultHandler;

        public override string CompressDllName => "SevenZipSharp";


        protected override string GetCompressFormat(int index)
        {
            string archiveFileFormat = "";
            switch (Enum.GetValues<OutArchiveFormat>()[index])
            {
                case OutArchiveFormat.SevenZip:
                    archiveFileFormat = "7z";
                    break;
                case OutArchiveFormat.Zip:
                    archiveFileFormat = "zip";
                    break;
                case OutArchiveFormat.GZip:
                    archiveFileFormat = "gz";
                    break;
                case OutArchiveFormat.BZip2:
                    archiveFileFormat = "bz2";
                    break;
                case OutArchiveFormat.Tar:
                    archiveFileFormat = "tar";
                    break;
                case OutArchiveFormat.XZ:
                    archiveFileFormat = "xz";
                    break;
                default:
                    break;
            }

            return archiveFileFormat;
        }

        private async Task CompressAny(MainWindow window, string source, TestType testType, bool isDirectory)
        {
            SevenZipBase.SetLibraryPath(Path.Combine(AppContext.BaseDirectory, @"x64\7z.dll"));

            switch (testType)
            {
                case TestType.Method:
                    foreach (var method in Enum.GetValues<CompressionMethod>())
                    {
                        await Start(method, Enum.GetValues<CompressionLevel>()[window.LevelSelectedIndex], isDirectory);
                    }
                    break;
                case TestType.Level:
                    //var barrier = new Barrier(methods.Length);
                    var levels = Enum.GetValues<CompressionLevel>();
                    var options = new ParallelOptions()
                    {
                        MaxDegreeOfParallelism = 1,
                        TaskScheduler = new ThreadPerTask()
                    };
                    await Parallel.ForEachAsync(levels, options, async (level, token) =>
                    {
                        await Start(Enum.GetValues<CompressionMethod>()[window.MethodSelectedIndex], level, isDirectory);
                    });
                    break;
                case TestType.All:
                    var results = new List<string>(Enum.GetNames<CompressionMethod>()
                                                       .SelectMany(m => Enum.GetNames<CompressionLevel>().Select(l => $"_{m}_{l}")));
                    foreach (var s in results)
                    {
                        var ss = s.Split('_').RemoveAt(0);
                        var method = Enum.Parse<CompressionMethod>(ss[0]);
                        var level = Enum.Parse<CompressionLevel>(ss[1]);
                        await Start(method, level, isDirectory);
                    }
                    break;
                default:
                    break;
            }

            async Task Start(CompressionMethod method, CompressionLevel level, bool isDirectory)
            {
                Suffix = $"_{method}_{level}";

                string filePath = "";
                if (isDirectory)
                    filePath = GetArchiveFilePathOfDirectory(window.FormatSelectedIndex);
                else
                    filePath = GetArchiveFilePathOfFile(window.FormatSelectedIndex);

                try
                {
                    var compressor = new SevenZipCompressor()
                    {
                        CompressionLevel = level,
                        CompressionMethod = method,
                        CompressionMode = CompressionMode.Create,
                        ArchiveFormat = Enum.GetValues<OutArchiveFormat>()[window.FormatSelectedIndex],
                        FastCompression = true, //不触发事件
                        VolumeSize = 0,
                    };
                    SevenZipCompressor.LzmaDictionarySize = 20 * 1024 * 1024;

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    compressor.FileCompressionStarted += (s, e) =>
                    {
                        UpdateSingleFileHandler(e.FileName, e.PercentDone);
                    };

                    byte value = 0;
                    compressor.Compressing += (s, e) =>
                    {
                        value += e.PercentDelta;
                        InteralDW($"{CompressDllName}压缩-{method}：{value}%");
                        UpdateTotalPercentHandler(value);
                    };

                    compressor.CompressionFinished += (s, e) =>
                    {

                    };

                    if (isDirectory)
                        await compressor.CompressDirectoryAsync(source, filePath);
                    else
                        await compressor.CompressFilesAsync(filePath, source);

                    stopwatch.Stop();
                    var resultInfo = new ResultInfo
                    {
                        FileName = Path.GetFileName(filePath),
                        ResultBrush = System.Windows.Media.Brushes.Green,
                        CompressedSize = new FileInfo(filePath).Length,
                        CompressedTime = stopwatch.Elapsed.Minutes + "m" + stopwatch.Elapsed.Seconds + "s",
                        Data = System.Windows.Application.Current.Resources["SuccessGeometry"] as Geometry,
                        IsShow = true
                    };
                    UpdateResultHandler(resultInfo);

                    File.Move(filePath, filePath.Insert(filePath.Length - 1 - GetCompressFormat(window.FormatSelectedIndex).Length, "_" + resultInfo.CompressedTime), true);
                }
                catch (Exception)
                {
                    var resultInfo = new ResultInfo
                    {
                        FileName = Path.GetFileName(filePath),
                        ResultBrush = System.Windows.Media.Brushes.Red,
                        Data = System.Windows.Application.Current.Resources["FailedGeometry"] as Geometry,
                        IsShow = false,
                    };
                    UpdateResultHandler(resultInfo);

                    File.Move(filePath, filePath.Insert(filePath.Length - 1 - GetCompressFormat(window.FormatSelectedIndex).Length, "_Failed"), true);
                }
            }
        }

        protected override async Task CompressFolder(MainWindow window, string sourceFolderPath, TestType testType = TestType.Method)
        {
            await CompressAny(window, sourceFolderPath, testType, true);
        }

        protected override async Task CompressFile(MainWindow window, string sourceFilePath, TestType testType = TestType.Method)
        {
            await CompressAny(window, sourceFilePath, testType, false);
        }

        private async Task DecompressAny(bool isDirectory)
        {
            SevenZipBase.SetLibraryPath(Path.Combine(AppContext.BaseDirectory, @"x64\7z.dll"));

            IEnumerable<string> files;
            if (isDirectory)
                files = Directory.GetFiles(FolderPath).Where(f => !f.Contains("Failed") && !Path.HasExtension(f.Split('_').First()));
            else
                files = Directory.GetFiles(FolderPath).Where(f => !f.Contains("Failed") && Path.HasExtension(f.Split('_').First()));

            foreach (var file in files)
            {
                using (var extractor = new SevenZipExtractor(file))
                {
                    if (isDirectory)
                        FolderName = Path.GetFileNameWithoutExtension(file);
                    else
                        FileName = Path.GetFileNameWithoutExtension(file);

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (isDirectory)
                        await extractor.ExtractArchiveAsync(GetExtractPathOfDirectory());
                    else
                        await extractor.ExtractArchiveAsync(GetExtractPathOfFile());
                    stopwatch.Stop();

                    var fileName = Path.GetFileName(file);
                    var time = fileName.Split('_', '.')[^2];
                    var resultInfo = new ResultInfo
                    {
                        FileName = fileName.Replace($"_{time}", ""),
                        ResultBrush = System.Windows.Media.Brushes.Green,
                        CompressedTime = stopwatch.Elapsed.Minutes + "m" + stopwatch.Elapsed.Seconds + "s",
                        Data = System.Windows.Application.Current.Resources["SuccessGeometry"] as Geometry,
                        IsShow = true
                    };
                    UpdateResultHandler(resultInfo);
                }
            }
        }

        protected override async Task DecompressFolder()
        {
            await DecompressAny(true);
        }

        protected override async Task DecompressFile()
        {
            await DecompressAny(false);
        }
    }
}

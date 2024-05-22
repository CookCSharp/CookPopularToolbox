/*
 *Description: SharpCompressHelper
 *Author: Chance.zheng
 *Creat Time: 2023/11/26 13:49:17
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using DryIoc.ImTools;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Factories;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static ICSharpCode.SharpZipLib.Zip.FastZip;

namespace CompressTool
{
    /// <summary>
    /// decompress 7zip, decompress xz, zip/unzip, tar/untar, lzip/unlzip, bzip2/unbzip2, gzip/ungzip
    /// </summary>
    /// <remarks>
    /// <see cref="CompressionType"/>/>
    /// </remarks>
    public class SharpCompressHelper : HelperBase
    {
        public event UpdateSingleFile UpdateSingleFileHandler;
        public event UpdateTotalPercent UpdateTotalPercentHandler;
        public event UpdateResult UpdateResultHandler;

        public override string CompressDllName => "SharpCompress";

        protected override string GetCompressFormat(int index)
        {
            //string archiveFileFormat = "";
            //switch (Enum.GetValues<ArchiveType>()[index])
            //{
            //    case ArchiveType.SevenZip:
            //        archiveFileFormat = "7z";
            //        break;
            //    case ArchiveType.Zip:
            //        archiveFileFormat = "zip";
            //        break;
            //    case ArchiveType.GZip:
            //        archiveFileFormat = "gz";
            //        break;
            //    case ArchiveType.Rar:
            //        archiveFileFormat = "rar";
            //        break;
            //    case ArchiveType.Tar:
            //        archiveFileFormat = "tar";
            //        break;
            //    default:
            //        break;
            //}

            //return archiveFileFormat;

            string[] formats = new string[] { "zip", "lz", "bz2", "gz(File)", "tar" };
            return formats[index].Replace("(File)", "");
        }

        private async Task CompressAny(MainWindow window, string source, TestType testType, bool isDirectory)
        {
            switch (testType)
            {
                case TestType.Method:
                    foreach (var method in Enum.GetValues<CompressionType>())
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
                        await Start(Enum.GetValues<CompressionType>()[window.MethodSelectedIndex], level, isDirectory);
                    });
                    break;
                case TestType.All:
                    var results = new List<string>(Enum.GetNames<CompressionType>()
                                                       .SelectMany(m => Enum.GetNames<CompressionLevel>().Select(l => $"_{m}_{l}")));
                    foreach (var s in results)
                    {
                        var ss = s.Split('_').RemoveAt(0);
                        var method = Enum.Parse<CompressionType>(ss[0]);
                        var level = Enum.Parse<CompressionLevel>(ss[1]);
                        await Start(method, level, isDirectory);
                    }
                    break;
                default:
                    break;
            }

            async Task Start(CompressionType method, CompressionLevel level, bool isDirectory)
            {
                await Task.Run(() =>
                {
                    string[] formats = new string[] { "zip", "lz", "bz2", "gz(File)", "tar" };
                    ArchiveType archiveType;
                    if (window.FormatSelectedIndex == 0 || window.FormatSelectedIndex == 1 || window.FormatSelectedIndex == 2)
                        archiveType = ArchiveType.Zip;
                    else if (window.FormatSelectedIndex == 3)
                        archiveType = ArchiveType.GZip;
                    else if (window.FormatSelectedIndex == 4)
                        archiveType = ArchiveType.Tar;
                    else
                        archiveType = ArchiveType.Zip;

                    //archiveType = Enum.GetValues<ArchiveType>()[window.FormatSelectedIndex];

                    if (window.FormatSelectedIndex == 0)
                        Suffix = $"_{method}_{level}";
                    else
                        Suffix = $"_{method}";

                    string filePath = "";
                    if (isDirectory)
                        filePath = GetArchiveFilePathOfDirectory(window.FormatSelectedIndex);
                    else
                        filePath = GetArchiveFilePathOfFile(window.FormatSelectedIndex);

                    try
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        using IWritableArchive archive = ArchiveFactory.Create(archiveType);
                        switch (archiveType)
                        {
                            case ArchiveType.Rar:
                                break;
                            case ArchiveType.Zip:
                                ((ZipArchive)archive).DeflateCompressionLevel = level;
                                break;
                            case ArchiveType.Tar:
                                break;
                            case ArchiveType.SevenZip:
                                break;
                            case ArchiveType.GZip:
                                break;
                            default:
                                break;
                        }

                        if (isDirectory)
                            archive.AddAllFromDirectory(source);
                        else
                            archive.AddEntry(FileName, new FileInfo(source));
                        archive.SaveTo(filePath, method);

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

                        try
                        {
                            File.Move(filePath, filePath.Insert(filePath.Length - 1 - GetCompressFormat(window.FormatSelectedIndex).Length, "_Failed"), true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
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
            await Task.Run(() =>
            {
                IEnumerable<string> files;
                if (isDirectory)
                    files = Directory.GetFiles(FolderPath).Where(f => !f.Contains("Failed") && !Path.HasExtension(f.Split('_').First()));
                else
                    files = Directory.GetFiles(FolderPath).Where(f => !f.Contains("Failed") && Path.HasExtension(f.Split('_').First()));

                foreach (var file in files)
                {
                    //ReaderOptions options = new ReaderOptions();
                    //options.ArchiveEncoding.Default = Encoding.UTF8;
                    
                    //using (var archive = ArchiveFactory.Open(file, options))
                    //{
                    //}

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    //if (isDirectory)
                    //{
                    //    FolderName = Path.GetFileNameWithoutExtension(file);
                    //    archive.ExtractToDirectory(GetExtractPathOfDirectory());
                    //}
                    //else
                    //{
                    //    var entries = archive.ExtractAllEntries();
                    //    FileName = Path.GetFileNameWithoutExtension(file);
                    //    entries.WriteEntryToFile(FileName, new ExtractionOptions()
                    //    {
                    //        ExtractFullPath = true,
                    //        Overwrite = true,
                    //    });
                    //}

                    string extractPath;
                    if (isDirectory)
                    {
                        FolderName = Path.GetFileNameWithoutExtension(file);
                        extractPath = GetExtractPathOfDirectory();
                    }
                    else
                    {
                        FileName = Path.GetFileNameWithoutExtension(file);
                        extractPath = GetExtractPathOfFile();
                    }
                    if (!Directory.Exists(extractPath))
                        Directory.CreateDirectory(extractPath);

                    using (Stream stream = File.OpenRead(file))
                    {
                        var reader = ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                Console.WriteLine(reader.Entry.Key);

                                reader.WriteEntryToDirectory(extractPath, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }
                    }

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
            });
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

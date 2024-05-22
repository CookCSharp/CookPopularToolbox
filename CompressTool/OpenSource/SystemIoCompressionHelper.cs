/*
 *Description: SystemIOCompressionHelper
 *Author: Chance.zheng
 *Creat Time: 2023/11/25 15:58:23
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CompressTool
{
    public class SystemIoCompressionHelper : HelperBase
    {

        protected override async Task CompressFolder(MainWindow window, string sourceFolderPath, TestType testType = TestType.Method)
        {
            //if (File.Exists(destinationArchiveFilePath))
            //    File.Delete(destinationArchiveFilePath);

            //if (!Directory.Exists(Path.GetDirectoryName(destinationArchiveFilePath)))
            //    Directory.CreateDirectory(Path.GetDirectoryName(destinationArchiveFilePath));

            ////526M 0m33s
            //ZipFile.CreateFromDirectory(sourceFolderPath, destinationArchiveFilePath, CompressionLevel.SmallestSize, false);

            await Task.FromResult(true);
        }

        protected override async Task CompressFile(MainWindow window, string sourceFilePath, TestType testType = TestType.Method)
        {
            //if (!Directory.Exists(Path.GetDirectoryName(destinationArchiveFilePath)))
            //    Directory.CreateDirectory(Path.GetDirectoryName(destinationArchiveFilePath));

            ////3542KB 0m0s
            //using FileStream fs = new FileStream(destinationArchiveFilePath, FileMode.Create);
            //using ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Create);
            //zipArchive.CreateEntryFromFile(sourceFilePath, Path.GetFileName(sourceFilePath));

            //if (File.Exists(destinationArchiveFilePath))
            //    File.Delete(destinationArchiveFilePath);
            //using var zipArchive = ZipFile.Open(destinationArchiveFilePath, ZipArchiveMode.Create);
            //zipArchive.CreateEntryFromFile(sourceFilePath, Path.GetFileName(sourceFilePath));

            //ZLibStream、GZipStream、DeflateStream、BrotliStream
            //using var stream = new ZLibStream(fs, CompressionLevel.SmallestSize);

            await Task.FromResult(true);
        }

        protected override async Task DecompressFolder()
        {
            //0m3s
            //ZipFile.ExtractToDirectory(archiveFilePath, extractPath, true);

            await Task.FromResult(true);
        }

        protected override async Task DecompressFile()
        {
            //0m3s
            //ZipFile.ExtractToDirectory(archiveFilePath, extractPath, true);

            //if (Directory.Exists(extractPath))
            //    Directory.Delete(extractPath, true);
            //Directory.CreateDirectory(extractPath);

            //using ZipArchive zipArchive = ZipFile.OpenRead(archiveFilePath);
            //string fileName = Path.GetFileName(TestConst.InputFilePath);
            //zipArchive.GetEntry(fileName).ExtractToFile(Path.Combine(extractPath, fileName));

            await Task.FromResult(true);
        }
    }
}

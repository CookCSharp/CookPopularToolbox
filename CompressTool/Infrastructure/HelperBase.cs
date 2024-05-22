/*
 *Description: HelperBase
 *Author: Chance.zheng
 *Creat Time: 2023/11/25 15:59:36
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using DryIoc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressTool
{
    public delegate void UpdateSingleFile(string fileName, byte percent);
    public delegate void UpdateTotalPercent(byte percent);
    public delegate void UpdateResult(ResultInfo resultInfo);


    public abstract class HelperBase
    {
        public virtual string CompressDllName => "SystemIoCompress";

        protected virtual string FolderName { get; set; } = new DirectoryInfo(TestConst.InputDirectory).Name;

        protected virtual string Suffix { get; set; }

        protected virtual string GetCompressFormat(int index) => "zip";

        protected string FolderPath => string.Format(Path.GetDirectoryName(TestConst.ArchiveFilePath), CompressDllName);

        protected virtual string FileName { get; set; } = Path.GetFileName(TestConst.InputFilePath);

        protected string GetArchiveFilePathOfDirectory(int index) =>
            string.Format(TestConst.ArchiveFilePath, CompressDllName, FolderName, Suffix, GetCompressFormat(index));

        protected string GetArchiveFilePathOfFile(int index) =>
            string.Format(TestConst.ArchiveFilePath, CompressDllName, FileName, Suffix, GetCompressFormat(index));

        protected string GetExtractPathOfDirectory() =>
            string.Format(TestConst.ExtractPath, CompressDllName, FolderName);

        protected string GetExtractPathOfFile() =>
            string.Format(TestConst.ExtractPath, CompressDllName, FileName);


        public virtual async Task Compress(MainWindow window, TestType testType = TestType.Method, bool testDirectory = true)
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            GetCompressFormat(window.FormatSelectedIndex);

            if (testDirectory)
                await CompressFolder(window, TestConst.InputDirectory, testType);
            else
                await CompressFile(window, TestConst.InputFilePath, testType);

            //stopwatch.Stop();
            //Console.WriteLine("文件夹压缩：" + Math.Round(new FileInfo(ArchiveFilePath_Directory).Length / 1024D / 1024D, 2) + "M");
            //Console.WriteLine(stopwatch.Elapsed.Minutes + "分" + stopwatch.Elapsed.Seconds + "秒");
            //stopwatch.Restart();

            //CompressFile(TestConst.InputFilePath, ArchiveFilePath_File);

            //stopwatch.Stop();
            //Console.WriteLine("文件压缩：" + Math.Round(new FileInfo(ArchiveFilePath_File).Length / 1024D, 2) + "KB");
            //Console.WriteLine(stopwatch.Elapsed.Minutes + "分" + stopwatch.Elapsed.Seconds + "秒");
        }

        public virtual async Task Decompress(MainWindow window, TestType testType = TestType.Method, bool testDirectory = true)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            if (testDirectory)
                await DecompressFolder();
            else
                await DecompressFile();

            //stopwatch.Stop();
            //long size_directory = new DirectoryInfo(ExtractPath_Directory).GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
            //Console.WriteLine("文件夹解压：" + Math.Round(size_directory / 1024D / 1024D, 2) + "M");
            //Console.WriteLine(stopwatch.Elapsed.Minutes + "分" + stopwatch.Elapsed.Seconds + "秒");
            //stopwatch.Restart();

            //stopwatch.Stop();
            //long size_file = new FileInfo(Path.Combine(ExtractPath_File, Path.GetFileName(TestConst.InputFilePath))).Length;
            //Console.WriteLine("文件解压：" + Math.Round(size_file / 1024D, 2) + "KB");
            //Console.WriteLine(stopwatch.Elapsed.Minutes + "分" + stopwatch.Elapsed.Seconds + "秒");
        }

        protected abstract Task CompressFolder(MainWindow window, string sourceFolderPath, TestType testType = TestType.Method);

        protected abstract Task CompressFile(MainWindow window, string sourceFilePath, TestType testType = TestType.Method);

        protected abstract Task DecompressFolder();

        protected abstract Task DecompressFile();

        protected void InteralDW(object value)
        {
            Debug.WriteLine(value);
            Thread.Sleep(1000);
        }


        //public abstract void CompressInMultiFormat();

        //public abstract void DecompressInMultiFormat();

        //{
        //    var enableList = new List<string>();
        //    var unableList = new List<string>();
        //    Array.ForEach(TestConst.Formats, f =>
        //    {
        //        try
        //        {
        //            CompressFormat = f;
        //            CompressFile(TestConst.InputFilePath, ArchiveFilePath_File);

        //            enableList.Add(f);
        //        }
        //        catch (Exception)
        //        {
        //            unableList.Add(f);
        //        }
        //    });

        //    if (enableList.Count > 0)
        //        Console.WriteLine(string.Join(',', enableList) + "这几种格式可以压缩");

        //    if (unableList.Count > 0)
        //        Console.WriteLine(string.Join(',', unableList) + "这几种格式不可以压缩");
        //}
    }
}

/*
 *Description: TestConst
 *Author: Chance.zheng
 *Creat Time: 2023/11/25 12:27:27
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompressTool
{
    public class TestConst
    {
        internal static string ProjectPath
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                var parent = Directory.GetParent(path).Parent.Parent.Parent.Parent.FullName;
                var projectPath = Path.Combine(parent, "CompressTool");
                return projectPath;
            }
        }

        internal static string InputDirectory { get; private set; } = ConfigurationManager.AppSettings.Get("CompressDirectory");
        internal static string InputFilePath { get; private set; } = ConfigurationManager.AppSettings.Get("CompressFile");

        //internal static readonly string InputFolderPath = ProjectPath + "\\Assets\\Directory"; //71.6M
        //internal static readonly string InputFilePath = ProjectPath + "\\Assets\\File.txt"; //23.9M
        internal static readonly string ArchiveFilePath = AppDomain.CurrentDomain.BaseDirectory + "Assets\\{0}\\Compressed\\{1}{2}.{3}";
        internal static readonly string ExtractPath = AppDomain.CurrentDomain.BaseDirectory + "Assets\\{0}\\Decompressed\\{1}";
        internal static readonly string[] Formats = ["zip", "7z", "lz", "bz2", "xz", "gz", "tgz", "tar", "rar"];

        internal static void SetInputDirectory(string path) => InputDirectory = path;
        internal static void SetInputFilePath(string path) => InputFilePath = path;
    }
}

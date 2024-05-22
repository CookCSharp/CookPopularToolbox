///*
// *Description: DotNetZipHelper
// *Author: Chance.zheng
// *Creat Time: 2023/11/26 13:39:35
// *.Net Version: 8.0
// *CLR Version: 4.0.30319.42000
// *Copyright © CookCSharp 2023 All Rights Reserved.
// */


//using Ionic.Zip;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CompressTool
//{
//    public class DotNetZipHelper : HelperBase
//    {
//        protected override string CompressDllName => "DotNetZip";

//        public override void CompressInMultiFormat()
//        {
//            throw new NotImplementedException();
//        }

//        public override void DecompressInMultiFormat()
//        {
//            throw new NotImplementedException();
//        }

//        protected override void CompressFolder(string sourceFolderPath, string destinationArchiveFilePath)
//        {
//            //532.81M 0m10s
//            using (ZipFile zip = new ZipFile())
//            {
//                zip.AddDirectory(sourceFolderPath);
//                zip.Save(destinationArchiveFilePath);
//            }
//        }

//        protected override void CompressFile(string sourceFilePath, string destinationArchiveFilePath)
//        {
//            //3720KB 0m0s
//            using (ZipFile zip = new ZipFile())
//            {
//                zip.AddFile(sourceFilePath, "");
//                zip.Save(destinationArchiveFilePath);
//            }
//        }

//        protected override void DecompressFolder(string archiveFilePath, string extractPath)
//        {
//            //0m10s
//            using (ZipFile zip = ZipFile.Read(archiveFilePath))
//            {
//                //zip.ExtractProgress += (s, e) =>
//                //{
//                //    Console.WriteLine(e.EntriesExtracted);
//                //};
//                foreach (var entry in zip)
//                {
//                    entry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
//                }
//            }
//        }

//        protected override void DecompressFile(string archiveFilePath, string extractPath)
//        {
//            //0m0s
//            using (ZipFile zip = ZipFile.Read(archiveFilePath))
//            {
//                foreach (var entry in zip)
//                {
//                    entry.Extract(extractPath, ExtractExistingFileAction.OverwriteSilently);
//                    //using (var ms = new MemoryStream())
//                    //{
//                    //    entry.Extract(ms);
//                    //}
//                }
//            }
//        }
//    }
//}

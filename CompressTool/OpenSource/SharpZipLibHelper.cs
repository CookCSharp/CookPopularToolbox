///*
// *Description: SharpZipLibHelper
// *Author: Chance.zheng
// *Creat Time: 2023/11/26 13:34:26
// *.Net Version: 8.0
// *CLR Version: 4.0.30319.42000
// *Copyright © CookCSharp 2023 All Rights Reserved.
// */


//using ICSharpCode.SharpZipLib.Core;
//using ICSharpCode.SharpZipLib.Zip;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static ICSharpCode.SharpZipLib.Zip.FastZip;

//namespace CompressTool
//{
//    public class SharpZipLibHelper : HelperBase
//    {
//        protected override string CompressDllName => "SharpZipLib";

//        public override void CompressInMultiFormat()
//        {

//        }

//        public override void DecompressInMultiFormat()
//        {

//        }

//        protected override void CompressFolder(string sourceFolderPath, string destinationArchiveFilePath)
//        {
//            //526.64M 0m48s
//            using (FileStream fsOut = File.Create(destinationArchiveFilePath))
//            {
//                ZipOutputStream zipStream = new ZipOutputStream(fsOut);
//                zipStream.SetLevel(9); // 设置压缩级别（0-9，9表示最高压缩级别）  

//                // 获取文件夹中的文件列表  
//                var files = Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories);
//                foreach (var file in files)
//                {
//                    // 创建ZipEntry对象，并设置相关属性  
//                    ZipEntry newEntry = new ZipEntry(Path.GetFileName(file));
//                    newEntry.DateTime = DateTime.Now; // 设置修改时间（可选）  
//                    zipStream.PutNextEntry(newEntry); // 将ZipEntry添加到ZipOutputStream中  

//                    // 将文件内容复制到ZipOutputStream中  
//                    using (FileStream streamReader = File.OpenRead(file))
//                    {
//                        StreamUtils.Copy(streamReader, zipStream, new byte[4096]); // 使用缓冲区复制文件内容  
//                    }
//                    zipStream.CloseEntry(); // 关闭当前ZipEntry并清理资源  
//                }
//                zipStream.IsStreamOwner = true; // 使关闭ZipOutputStream也会关闭输出流（可选）  
//                zipStream.Close(); // 完成压缩操作并关闭ZipOutputStream和输出流（可选）  
//            }
//        }

//        protected override void CompressFile(string sourceFilePath, string destinationArchiveFilePath)
//        {
//            //3513KB 0m1s
//            FileStream fsOut = File.Create(destinationArchiveFilePath);
//            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
//            zipStream.SetLevel(9); // 0 - store only to 9 - means best compression  

//            ZipEntry newEntry = new ZipEntry(sourceFilePath);
//            newEntry.DateTime = DateTime.Now;
//            zipStream.PutNextEntry(newEntry);

//            byte[] buffer = new byte[4096];
//            using (FileStream streamReader = File.OpenRead(sourceFilePath))
//            {
//                StreamUtils.Copy(streamReader, zipStream, buffer);
//            }
//            zipStream.CloseEntry();
//            zipStream.IsStreamOwner = true;
//            zipStream.Close();
//        }

//        protected override void DecompressFolder(string archiveFilePath, string extractPath)
//        {

//        }

//        protected override void DecompressFile(string archiveFilePath, string extractPath)
//        {

//        }
//    }
//}

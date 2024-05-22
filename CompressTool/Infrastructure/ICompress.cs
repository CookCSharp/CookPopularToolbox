/*
 *Description: ICompress
 *Author: Chance.zheng
 *Creat Time: 2023/11/25 11:55:51
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressTool
{
    public interface ICompress
    {
        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="sourceFolderPath"></param>
        /// <param name="destinationZipFilePath"></param>
        void CompressFolder(string sourceFolderPath, string destinationArchiveFilePath);

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationZipFilePath"></param>
        void CompressFile(string sourceFilePath, string destinationArchiveFilePath);
    }
}

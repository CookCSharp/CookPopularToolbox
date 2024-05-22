/*
 *Description: IDecompress
 *Author: Chance.zheng
 *Creat Time: 2023/11/25 11:57:30
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
    public interface IDecompress
    {
        /// <summary>
        /// 解压文件夹
        /// </summary>
        /// <param name="archiveFilePath"></param>
        /// <param name="extractPath"></param>
        void UncompressFolder(string archiveFilePath, string extractPath);

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="archiveFilePath"></param>
        /// <param name="extractPath"></param>
        void UncompressFile(string archiveFilePath, string extractPath);
    }
}

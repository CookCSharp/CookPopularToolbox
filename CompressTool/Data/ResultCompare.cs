/*
 *Description: ResultCompare
 *Author: Chance.zheng
 *Creat Time: 2023/11/29 17:00:21
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
    public class ResultCompressedSizeCompare : IComparer<ResultInfo>
    {
        public int Compare(ResultInfo info1, ResultInfo info2)
        {
            if (!info1.IsShow)
                return 1;
            if (!info2.IsShow)
                return -1;

            if (info1.CompressedSize == info2.CompressedSize)
            {
                var timeStr1 = "0:" + info1.CompressedTime.Replace("m", ":").Replace("s", "");
                var time1 = TimeSpan.Parse(timeStr1);

                var timeStr2 = "0:" + info2.CompressedTime.Replace("m", ":").Replace("s", "");
                var time2 = TimeSpan.Parse(timeStr2);

                return StringComparer.OrdinalIgnoreCase.Compare(time1, time2);
            }
            else
                return StringComparer.OrdinalIgnoreCase.Compare(info1.CompressedSize, info2.CompressedSize);
        }
    }

    public class ResultCompressedTimeCompare : IComparer<ResultInfo>
    {
        public int Compare(ResultInfo info1, ResultInfo info2)
        {
            if (!info1.IsShow)
                return 1;
            if (!info2.IsShow)
                return -1;

            if (info1.CompressedTime == info2.CompressedTime)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(info1.CompressedSize, info2.CompressedSize);
            }
            else
            {
                var timeStr1 = "0:" + info1.CompressedTime.Replace("m", ":").Replace("s", "");
                var time1 = TimeSpan.Parse(timeStr1);

                var timeStr2 = "0:" + info2.CompressedTime.Replace("m", ":").Replace("s", "");
                var time2 = TimeSpan.Parse(timeStr2);

                return StringComparer.OrdinalIgnoreCase.Compare(time1, time2);
            }
        }
    }
}

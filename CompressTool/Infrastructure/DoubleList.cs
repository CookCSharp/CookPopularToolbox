/*
 *Description: DoubleList
 *Author: Chance.zheng
 *Creat Time: 2023/11/29 10:16:42
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp;

namespace CompressTool
{
    public class DoubleList<T1, T2> : IEnumerable<T1>, IEnumerator<T1>
    {
        private List<T1> _ts1 = new List<T1>();
        private List<T2> _ts2 = new List<T2>();

        public DoubleList(List<T1> ts1, List<T2> ts2)
        {
            _ts1 = ts1;
            _ts2 = ts2;
        }

        public IEnumerator<T1> GetEnumerator()
        {
            return new DoubleList<T1, T2>(_ts1, _ts2);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T1>)this).GetEnumerator();
        }

        public T1 Current { get; }

        public bool MoveNext()
        {
            return true;
        }

        public void Reset()
        {

        }

        object IEnumerator.Current { get; }

        public void Dispose()
        {

        }
    }
}

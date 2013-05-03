using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW
{
    internal static class Extensions
    {
		static T Last<T>(this List<T> list){
            return list[list.Count - 1];
        }
    }

    class Tuple<T1,T2>
    {
        internal readonly T1 Item1;
        internal readonly T2 Item2;
        public Tuple(T1 o1, T2 o2)
        {
            Item1 = o1;
            Item2 = o2;
        }
    }
}

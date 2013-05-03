using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW
{
    /// <summary>
    /// Extensions for compatibility with .NET 4.0
    /// </summary>
    internal static class Extensions
    {
        static T Last<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }
    }

    /// <summary>
    /// Class for compatibility with .NET 4.0
    /// Represents tuple of items
    /// </summary>
    /// <typeparam name="T1">Type of item1</typeparam>
    /// <typeparam name="T2">Type of item2</typeparam>
    class Tuple<T1, T2>
    {
        /// <summary>
        /// First item
        /// </summary>
        internal readonly T1 Item1;
        /// <summary>
        /// Second item
        /// </summary>
        internal readonly T2 Item2;

        /// <summary>
        /// Creates tuple for given items
        /// </summary>
        /// <param name="item1">First item</param>
        /// <param name="item2">Second item</param>
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }


        #region Standard method overrides
        public override bool Equals(object obj)
        {
            var oTuple = obj as Tuple<T1, T2>;
            if (oTuple == null)
            {
                return false;
            }

            return object.Equals(this.Item1, oTuple.Item1) && object.Equals(this.Item2, oTuple.Item2);
        }

        public override int GetHashCode()
        {
            return ((this.Item1 != null) ? this.Item1.GetHashCode() : 0) ^ ((this.Item2 != null) ? this.Item2.GetHashCode() : 0);
        }

        public static bool operator ==(Tuple<T1, T2> t1, Tuple<T1, T2> t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(Tuple<T1, T2> t1, Tuple<T1, T2> t2)
        {
            return !t1.Equals(t2);
        }
        #endregion
    }
}

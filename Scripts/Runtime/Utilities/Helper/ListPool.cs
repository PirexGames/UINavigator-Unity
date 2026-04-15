using System.Collections.Generic;

namespace PirexGames.Core.UINavigator
{
    /// <summary>
    /// Simple high-performance pool for lists to avoid GC and re-entrancy issues.
    /// </summary>
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> _pool = new Stack<List<T>>();

        public static List<T> Get()
        {
            if (_pool.Count > 0)
            {
                var list = _pool.Pop();
                list.Clear();
                return list;
            }
            return new List<T>(8);
        }

        public static void Release(List<T> list)
        {
            if (list == null) return;
            list.Clear();
            _pool.Push(list);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AFDApp
{
    /// <summary>
    /// This class will be used to
    /// store  ThreadResult in a
    /// HashTable which has been
    /// declared as static.
    /// </summary>
    public class ThreadResult
    {
        private static Hashtable ThreadsList = new Hashtable();

        public static void Add(string key, object value)
        {
            ThreadsList.Add(key, value);
        }

        public static object Get(string key)
        {
            return ThreadsList[key];
        }

        public static void Remove(string key)
        {
            ThreadsList.Remove(key);
        }

        public static bool Contains(string key)
        {
            return ThreadsList.ContainsKey(key);
        }

    }
}
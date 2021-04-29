using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods {
    static class ExtensionMethods {
        public static Stack<T> Reverse<T>(this Stack<T> stack) {
            Stack<T> temp = new();
            while (stack.Count > 0) {
                temp.Push(stack.Pop());
            }
            return temp;
        }

        public static List<int> AllIndexesOf(this string str, string value) {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length) {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}

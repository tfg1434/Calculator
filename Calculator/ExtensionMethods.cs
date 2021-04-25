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
    }
}

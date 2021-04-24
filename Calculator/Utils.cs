using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator {
    static class Utils {
        public static Stack<T> Reverse<T>(Stack<T> stack) {
            Stack<T> temp = new();
            while (stack.Count > 0) {
                temp.Push(stack.Pop());
            }
            return temp;
        }
    }
}

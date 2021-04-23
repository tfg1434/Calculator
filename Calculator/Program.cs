using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator {
    class Program {
        static void Main(string[] args) {
            string equation = args[0];
            Stack<string> equation_stack = new();
            //convert (12+3) to [(, 12, +, 3, )]
            string to_add = "";
            foreach (char c in equation) {
                if (char.IsDigit(c)) {
                    to_add += c;
                } else {
                    equation_stack.Push(to_add);
                    to_add = "";
                    equation_stack.Push(c.ToString());
                }
            }

            Stack<string> operator_stack = new();
            Queue<string> rpn = new();

            string[] operators = { "+", "-", "*", "/" };
            while (equation_stack.Count > 0) {
                string token = equation_stack.Pop();
                if (int.TryParse(token, out int num)) {
                    rpn.Enqueue(num.ToString());

                } else if (token == "("){
                    operator_stack.Push(token);

                } else if () {
                    

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator {
    class Program {
        static void Main(string[] args) {
            string equation = args[0];
            Queue<string> rpn = shunting_yard(equation);
            int a = 5;
        }

        static Queue<string> shunting_yard(string equation) {
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
            equation_stack.Push(to_add);
            equation_stack = Utils.Reverse(equation_stack);

            Stack<string> operator_stack = new();
            Queue<string> rpn = new();

            string[] operators = { "+", "-", "*", "/", "^" };
            Dictionary<string, int> precedence = new() {
                ["^"] = 4,
                ["*"] = 3,
                ["/"] = 3,
                ["+"] = 2,
                ["-"] = 2,
            };
            const int right_associative = 0;
            const int left_associative = 1;
            Dictionary<string, int> associativity = new() {
                ["^"] = right_associative,
                ["*"] = left_associative,
                ["/"] = left_associative,
                ["+"] = left_associative,
                ["-"] = left_associative,
            };

            while (equation_stack.Count > 0) {
                string token = equation_stack.Pop();
                if (int.TryParse(token, out int num)) {
                    rpn.Enqueue(num.ToString());

                } else if (operators.Contains(token)) {
                    while (operator_stack.Count > 0 && operators.Contains(operator_stack.Peek()) &&
                        (precedence[operator_stack.Peek()] > precedence[token] || (precedence[operator_stack.Peek()] == precedence[token] && associativity[token] == left_associative)) &&
                        operator_stack.Peek() != "(") {
                        rpn.Enqueue(operator_stack.Pop());
                    }

                    operator_stack.Push(token);

                } else if (token == "(") {
                    operator_stack.Push(token);

                } else if (token == ")") {
                    while (operator_stack.Count > 0 && operator_stack.Peek() != "(") {
                        rpn.Enqueue(operator_stack.Pop());
                    }
                    // If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                    if (operator_stack.Count > 0 && operator_stack.Peek() == "(") {
                        operator_stack.Pop();
                    }
                }
            }

            if (equation_stack.Count == 0) {
                while (operator_stack.Count > 0) {
                    /* If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses. */
                    rpn.Enqueue(operator_stack.Pop());
                }
            }

            return rpn;
        }
    }
}

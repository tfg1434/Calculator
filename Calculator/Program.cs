﻿using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;

namespace Calculator {
    class Program {
        static void Main(string[] args) {
            string equation = args[0];
            List<string> rpn = ShuntingYard(equation);
            Console.WriteLine(Solve(rpn));
        }

        static int Solve(List<string> rpn) {
            Stack<string> stack = new();

            foreach (string token in rpn) {
                if (int.TryParse(token, out _)) {
                    stack.Push(token);
                    continue;

                } else {
                    //it's an operator
                    int n1 = int.Parse(stack.Pop());
                    int n2 = int.Parse(stack.Pop());

                    stack.Push(token switch {
                        "^" => ((int)Math.Pow(n2, n1)).ToString(),
                        "*" => (n1 * n2).ToString(),
                        "/" => (n1 / n2).ToString(),
                        "+" => (n1 + n2).ToString(),
                        "-" => (n1 - n2).ToString(),
                        _ => throw new Exception("wtf")
                    });
                }
            }

            return int.Parse(stack.Pop());
        }

        static List<string> ShuntingYard(string equation) {
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
            equation_stack = equation_stack.Reverse();

            Stack<string> operator_stack = new();
            List<string> rpn = new();

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
                    rpn.Add(num.ToString());

                } else if (operators.Contains(token)) {
                    while (operator_stack.Count > 0 && operators.Contains(operator_stack.Peek()) &&
                        (precedence[operator_stack.Peek()] > precedence[token] || (precedence[operator_stack.Peek()] == precedence[token] && associativity[token] == left_associative)) &&
                        operator_stack.Peek() != "(") {
                        rpn.Add(operator_stack.Pop());
                    }

                    operator_stack.Push(token);

                } else if (token == "(") {
                    operator_stack.Push(token);

                } else if (token == ")") {
                    while (operator_stack.Count > 0 && operator_stack.Peek() != "(") {
                        rpn.Add(operator_stack.Pop());
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
                    rpn.Add(operator_stack.Pop());
                }
            }

            return rpn;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using DecimalMath;

namespace Calculator {
    class Program {
        static void Main(string[] args) {
            string equation = args[0];
            List<string> rpn = ShuntingYard(equation);
            decimal a = Solve(rpn);
            int b = 5;
        }

        static readonly string[] operators = { "+", "-", "*", "/", "^" };

        static decimal Solve(List<string> rpn) {
            Stack<string> stack = new();

            foreach (string token in rpn) {
                if (decimal.TryParse(token, out _)) {
                    stack.Push(token);
                    continue;

                } else if (operators.Contains(token)){
                    //it's an operator
                    decimal n1 = decimal.Parse(stack.Pop());
                    decimal n2 = decimal.Parse(stack.Pop());

                    stack.Push(token switch {
                        "^" => DecimalEx.Pow(n2, n1).ToString(),
                        "*" => (n2 * n1).ToString(),
                        "/" => (n2 / n1).ToString(),
                        "+" => (n2 + n1).ToString(),
                        "-" => (n2 - n1).ToString(),
                        _ => throw new Exception("wtf")
                    });

                } else {
                    //function
                    switch (token) {
                        case "e":
                            stack.Push(Math.E.ToString());
                            break;
                        case "pi":
                            stack.Push(Math.PI.ToString());
                            break;
                        case "abs":
                            stack.Push(Math.Abs(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "arccos":
                            stack.Push(DecimalEx.ACos(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darccos":
                            stack.Push((DecimalEx.ACos(decimal.Parse(stack.Pop())) * 180 / DecimalEx.Pi).ToString());
                            break;
                        case "arcsin":
                            stack.Push(DecimalEx.ASin(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darcsin":
                            stack.Push((DecimalEx.ASin(decimal.Parse(stack.Pop())) * 180 / DecimalEx.Pi).ToString());
                            break;
                        case "arctan":
                            stack.Push(DecimalEx.ATan(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darctan":
                            stack.Push((DecimalEx.ATan(decimal.Parse(stack.Pop())) * 180 / DecimalEx.Pi).ToString());
                            break;
                        case "arctan2":
                            stack.Push(DecimalEx.ATan2(decimal.Parse(stack.Pop()), decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darctan2":
                            stack.Push((DecimalEx.ATan2(decimal.Parse(stack.Pop()), decimal.Parse(stack.Pop())) * 180 / DecimalEx.Pi).ToString());
                            break;
                        case "ceil":
                            stack.Push(DecimalEx.Ceiling(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "clamp":
                            //args backward because it's popped off backwards
                            decimal max = decimal.Parse(stack.Pop());
                            decimal min = decimal.Parse(stack.Pop());
                            decimal val = decimal.Parse(stack.Pop());

                            //Console.WriteLine($"val: {val}  min: {min}   max: {max}");
                            stack.Push(Math.Clamp(val, min, max).ToString());
                            break;
                        case "cos":
                            stack.Push(DecimalEx.Cos(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "dcos":
                            stack.Push((DecimalEx.Cos(decimal.Parse(stack.Pop())) * 180 / DecimalEx.Pi).ToString());
                            break;
                        case "mod":
                            decimal divisor = decimal.Parse(stack.Pop());
                            val = decimal.Parse(stack.Pop());
                            stack.Push((val % divisor).ToString());
                            break;
                        case "floor":
                            stack.Push(Math.Floor(decimal.Parse(stack.Pop())).ToString());
                            break;
                    }
                }
            }

            return decimal.Parse(stack.Pop());
        }

        static List<string> ShuntingYard(string equation) {
            Stack<string> equation_stack = new();
            Lexer lexer = new(equation);
            while (!lexer.empty) {
                equation_stack.Push(lexer.Next());
            }
            equation_stack = equation_stack.Reverse();

            Stack<string> operator_stack = new();
            List<string> rpn = new();

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
                if (decimal.TryParse(token, out _)) {
                    rpn.Add(token);

                } else if (token.All(char.IsLetter)){
                    //operator
                    operator_stack.Push(token);

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
                    //If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                    if (operator_stack.Count == 0) {
                        throw new Exception("Mismatched parenthesis");
                    }

                    if (operator_stack.Count > 0 && operator_stack.Peek() == "(") {
                        operator_stack.Pop();
                    }
                    if (operator_stack.Count > 0 && operator_stack.Peek().All(char.IsLetter)) {
                        rpn.Add(operator_stack.Pop());
                    }
                }
            }

            if (equation_stack.Count == 0) {
                while (operator_stack.Count > 0) {
                    //If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
                    if (operator_stack.Peek() == "(" || operator_stack.Peek() == ")") {
                        throw new Exception("Mismatched Parentheses");
                    }

                    rpn.Add(operator_stack.Pop());
                }
            }

            return rpn;
        }
    }
}

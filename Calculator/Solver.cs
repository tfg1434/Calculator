﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecimalMath;

namespace Calculator {
    static class Solver {
        private static decimal eval(List<string> rpn) {
            string[] operators = { "+", "-", "*", "/", "^" };
            Stack<string> stack = new();

            foreach (string token in rpn) {
                if (decimal.TryParse(token, out _)) {
                    stack.Push(token);
                    continue;

                } else if (operators.Contains(token)) {
                    //it's an operator
                    decimal n1 = decimal.Parse(stack.Pop());
                    decimal n2 = decimal.Parse(stack.Pop());

                    stack.Push(token switch {
                        "^" => DecimalEx.Pow(n2, n1).ToString(),
                        "*" => (n2 * n1).ToString(),
                        "/" => (n2 / n1).ToString(),
                        "+" => (n2 + n1).ToString(),
                        "-" => (n2 - n1).ToString(),
                        _ => throw new Exception("wtf"),
                    });

                } else {
                    //function
                    switch (token) {
                        case "~":
                            //negate
                            //precedence doesn't matter here since rpn removes it
                            stack.Push("-" + stack.Pop());
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
                        case "clamp": {
                                //args backward because it's popped off backwards
                                decimal max = decimal.Parse(stack.Pop());
                                decimal min = decimal.Parse(stack.Pop());
                                decimal val = decimal.Parse(stack.Pop());

                                //Console.WriteLine($"val: {val}  min: {min}   max: {max}");
                                stack.Push(Math.Clamp(val, min, max).ToString());
                                break;
                            }
                        case "cos":
                            stack.Push(DecimalEx.Cos(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "dcos":
                            stack.Push((DecimalEx.Cos(decimal.Parse(stack.Pop())) * 180 / DecimalEx.Pi).ToString());
                            break;
                        case "mod": {
                                decimal divisor = decimal.Parse(stack.Pop());
                                decimal val = decimal.Parse(stack.Pop());
                                stack.Push((val % divisor).ToString());
                                break;
                            }
                        case "floor":
                            stack.Push(Math.Floor(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "ln":
                            stack.Push(DecimalEx.Log(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "log": {
                                decimal my_base = decimal.Parse(stack.Pop());
                                decimal val = decimal.Parse(stack.Pop());
                                stack.Push(DecimalEx.Log(val, my_base).ToString());
                                break;
                            }
                        case "log10":
                            stack.Push(DecimalEx.Log10(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "log2":
                            stack.Push(DecimalEx.Log2(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "max": {
                                decimal val2 = decimal.Parse(stack.Pop());
                                decimal val1 = decimal.Parse(stack.Pop());
                                stack.Push(Math.Max(val1, val2).ToString());
                                break;
                            }
                        case "min": {
                                decimal val2 = decimal.Parse(stack.Pop());
                                decimal val1 = decimal.Parse(stack.Pop());
                                stack.Push(Math.Min(val1, val2).ToString());
                                break;
                            }
                        case "round":
                            stack.Push(Math.Round(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "sign":
                            stack.Push(Math.Sign(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "sin":
                            stack.Push(DecimalEx.Sin(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "sqrt":
                            stack.Push(DecimalEx.Sqrt(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "tan":
                            stack.Push(DecimalEx.Tan(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "trunc":
                            stack.Push(Math.Truncate(decimal.Parse(stack.Pop())).ToString());
                            break;
                        default:
                            throw new Exception($"Unknown function token {token}");
                    }
                }
            }

            return decimal.Parse(stack.Pop());
        }

        public static decimal Solve(string equation, Dictionary<string, string> variables) {
            Parser parser = new(equation, variables);
            return eval(parser.Parse());
        }
    }
}

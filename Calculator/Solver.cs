using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecimalMath;

namespace Calculator {
    static class Solver {
        public static decimal Solve(string equation, Dictionary<string, string> variables) => eval(Parser.Parse(equation, variables));

        public static uint NCr(uint n, uint r) {
            if (n < r)
                throw new Exception("nCr: must be n >= r >= 0");

            return fact(n) / (fact(r) * fact(n - r));
        }

        public static int IntPow(int x, uint pow) {
            int ret = 1;
            while (pow != 0) {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        private static uint fact(uint num) {
            uint ans = 1;
            for (uint i = 1; i <= num; i++)
                ans *= i;
            return ans;
        }

        private static decimal eval(List<string> rpn) {
            string[] operators = { "+", "-", "*", "/", "^" };
            Stack<string> stack = new();

            foreach (string token in rpn) {
                if (decimal.TryParse(token, out _)) {
                    stack.Push(token);

                } else if (operators.Contains(token)) {
                    //it's an operator

                    if (stack.Count == 0)
                        throw new Exception($"{token}: expected number, got nothing");
                    string pop = stack.Pop();
                    if (!decimal.TryParse(pop, out decimal n1))
                        throw new Exception($"{token}: expected number, got {pop}");

                    if (stack.Count == 0)
                        throw new Exception($"{token}: expected number, got nothing"); //now its the lexers fault not parsing the .
                    pop = stack.Pop();
                    if (!decimal.TryParse(pop, out decimal n2))
                        throw new Exception($"{token}: expected number, got {pop}");


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
                            stack.Push(DecimalEx.ACos(decimal.Parse(stack.Pop()) * DecimalEx.Pi / 180).ToString());
                            break;
                        case "arcsin":
                            stack.Push(DecimalEx.ASin(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darcsin":
                            stack.Push(DecimalEx.ASin(decimal.Parse(stack.Pop()) * DecimalEx.Pi / 180).ToString());
                            break;
                        case "arctan":
                            stack.Push(DecimalEx.ATan(decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darctan":
                            stack.Push(DecimalEx.ATan(decimal.Parse(stack.Pop()) * DecimalEx.Pi / 180).ToString());
                            break;
                        case "arctan2":
                            stack.Push(DecimalEx.ATan2(decimal.Parse(stack.Pop()), decimal.Parse(stack.Pop())).ToString());
                            break;
                        case "darctan2":
                            stack.Push(DecimalEx.ATan2(decimal.Parse(stack.Pop()) * DecimalEx.Pi / 180, decimal.Parse(stack.Pop()) * DecimalEx.Pi / 180).ToString());
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
                            stack.Push(DecimalEx.Cos(decimal.Parse(stack.Pop()) * DecimalEx.Pi / 180).ToString());
                            break;
                        case "mod": {
                                decimal divisor = decimal.Parse(stack.Pop());
                                decimal val = decimal.Parse(stack.Pop());
                                stack.Push((val % divisor).ToString());
                                break;
                            }
                        case "fact":
                            stack.Push(fact((uint)decimal.Parse(stack.Pop())).ToString());
                            break;
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
                        case "ncr": {
                            uint r = (uint)decimal.Parse(stack.Pop());
                            uint n = (uint)decimal.Parse(stack.Pop());
                            stack.Push(NCr(n, r).ToString());
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
    }
}

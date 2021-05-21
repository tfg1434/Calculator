using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using ExtensionMethods;
using System.Text;
using System.Threading.Tasks;
using DecimalMath;
using System.Text.RegularExpressions;
using Calculator.CAS;

namespace Calculator {
    static class Parser {
        //parse a string into reverse polish notation
        private static readonly Dictionary<string, int> precedence = new() {
            ["^"] = 5,
            ["~"] = 4,
            ["*"] = 3,
            ["/"] = 3,
            ["+"] = 2,
            ["-"] = 2,
        };
        private const int right_associative = 0;
        private const int left_associative = 1;
        private static readonly Dictionary<string, int> associativity = new() {
            ["^"] = right_associative,
            ["~"] = left_associative,
            ["*"] = left_associative,
            ["/"] = left_associative,
            ["+"] = left_associative,
            ["-"] = left_associative,
        };
        private static readonly string[] operators = { "+", "-", "*", "/", "^", "~" };
        //constants/variables
        //<string, string> so unary negate works ~
        private static readonly Dictionary<string, string> constants = new() {
            ["e"] = DecimalEx.E.ToString(),
            ["pi"] = DecimalEx.Pi.ToString(),
            ["tau"] = (DecimalEx.Pi * 2).ToString(),
        };

        public static List<string> Parse(string equation, Dictionary<string, string> variables) {
            List<string> tokens = new();
            Dictionary<string, string> variables_constants = new(constants);
            variables.ToList().ForEach(x => variables_constants[x.Key] = x.Value);

            foreach (string token in Lexer.Lex(equation, variables_constants)) {
                string str = token;

                ////constants and variables
                //if (variables_constants.ContainsKey(str)) {
                //    str = variables_constants[str];

                //    if (tokens.Count > 0 && tokens.Peek().All(char.IsDigit)) {
                //        tokens.Push("*");
                //    }

                //    //this is for constants/variables, thus not in Lexer
                //    if (str.Contains("~")) {
                //        str = str.Replace("~", "");
                //        tokens.Push("~");
                //    }
                //}


                //implicit multiplication
                //number -> open parenthesis 3(
                if (str == "(" && tokens.Count > 0 && Regex.Match(tokens[^1], @"\d(?:\.\d)?").Success)
                    tokens.Add("*");
                //close parenthesis -> number )3 || close parenthesis -> . ).01
                if (tokens.Count > 0 && tokens[^1] == ")" && Regex.Match(str, @"\d(?:\.\d)?").Success)
                    tokens.Add("*");
                //close parenthesis -> open parenthesis
                if (str == "(" && tokens.Count > 0 && tokens[^1] == ")")
                    tokens.Add("*");


                //error handling
                if (operators.Contains(str) && tokens.Count > 0 && operators.Contains(tokens[^1]))
                    throw new SyntaxException($"Unexpected operator {str}");

                //is it a function not followed by (?
                if (tokens.Count > 0 && tokens[^1].Any(char.IsLetter) && str != "(" && !variables_constants.ContainsKey(tokens[^1]))
                    throw new SyntaxException($"Expected (, got {str}");

                tokens.Add(str);
            }

            return tokens;
        }

        //check the first val for constant
        public static List<string> InsertVariablesConstants(List<string> equation, Dictionary<string, string> variables) {
            Dictionary<string, string> variables_constants = new(constants);
            variables.ToList().ForEach(x => variables_constants[x.Key] = x.Value);

            for (int i = 0; i < equation.Count - 1; i++) {
                string curr = equation[i];
                string next = equation[i + 1];

                if (variables_constants.TryGetValue(curr, out string val)) {
                    //3a
                    if (next.All(char.IsLetter) && Regex.Match(curr, @"\d(?:\.\d)?").Success) {
                        equation.Insert(i + 1, "*");
                        i++;
                        continue;
                    }

                    //a3
                    if (curr.All(char.IsLetter) && Regex.Match(next, @"\d(?:\.\d)?").Success) {
                        equation.Insert(i + 1, "*");
                        i++;
                        continue;
                    }

                    //aa
                    if (curr.All(char.IsLetter) && next.All(char.IsLetter)) {
                        equation.Insert(i + 1, "*");
                        i++;
                        continue;
                    }
                }
            }

            for (int i = 0; i < equation.Count; i++) {
                if (variables_constants.TryGetValue(equation[i], out string val_))
                    equation[i] = val_;
            }

            int index = equation.IndexOf("~");
            while (index != -1) {
                equation[index] = equation[index].Replace("~", "");
                if (index == 0)
                    equation = equation.Prepend("~").ToList();
                else
                    equation.Insert(index - 1, "~");

                index = equation.IndexOf("~");
            }

            return equation;
        }

        public static List<string> ShuntingYard(List<string> tokens) {
            Stack<string> operator_stack = new();
            List<string> rpn = new();

            while (tokens.Count > 0) {
                string token = tokens[^1];
                tokens.RemoveAt(tokens.Count - 1);
                
                if (decimal.TryParse(token, out _)) {
                    rpn.Add(token);

                } else if ((token ?? throw new InvalidOperationException()).All(char.IsLetter)) {
                    //function
                    operator_stack.Push(token);

                } else if (operators.Contains(token)) {
                    while (operator_stack.Count > 0 && operators.Contains(operator_stack.Peek()) &&
                        (precedence[operator_stack.Peek()] > precedence[token] || (precedence[operator_stack.Peek()] == precedence[token] && associativity[token] == left_associative)) &&
                        operator_stack.Peek() != "(") {
                        rpn.Add(operator_stack.Pop());
                    }

                    operator_stack.Push(token);

                } else {
                    switch (token) {
                        case "(":
                            operator_stack.Push(token);
                            break;
                        case ")": {
                            while (operator_stack.Count > 0 && operator_stack.Peek() != "(") {
                                rpn.Add(operator_stack.Pop());
                            }

                            //If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                            if (operator_stack.Count == 0) {
                                throw new SyntaxException("Mismatched parenthesis");
                            }

                            if (operator_stack.Count > 0 && operator_stack.Peek() == "(") {
                                operator_stack.Pop();
                            }

                            if (operator_stack.Count > 0 && operator_stack.Peek().All(char.IsLetter)) {
                                rpn.Add(operator_stack.Pop());
                            }

                            break;
                        }

                    }
                }
            }

            if (tokens.Count != 0) return rpn;
            while (operator_stack.Count > 0) {
                //If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
                if (operator_stack.Peek() == "(" || operator_stack.Peek() == ")") {
                    throw new SyntaxException("Mismatched parentheses");
                }

                rpn.Add(operator_stack.Pop());
            }

            return rpn;
        }
    }
}

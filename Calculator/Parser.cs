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

        private static Dictionary<string, string> variables_constants = new(constants);

        public static List<string> Parse(string equation, Dictionary<string, string> variables) {
            foreach ((string key, string value) in variables)
                variables_constants[key] = value;

            Stack<string> tokens = new();

            foreach (string token in Lexer.Lex(equation)) {
                string str = token;

                //constants and variables
                if (variables_constants.ContainsKey(str)) {
                    str = variables_constants[str];

                    if (tokens.Count > 0 && tokens.Peek().All(char.IsDigit)) {
                        tokens.Push("*");
                    }

                    //this is for constants/variables, thus not in Lexer
                    if (str.Contains("~")) {
                        str = str.Replace("~", "");
                        tokens.Push("~");
                    }
                }


                //implicit multiplication
                //number -> open parenthesis 3(
                if (str == "(" && tokens.Count > 0 && Regex.Match(tokens.Peek(), @"\d(?:\.\d)?").Success)
                    tokens.Push("*");
                //close parenthesis -> number )3 || close parenthesis -> . ).01
                if (tokens.Count > 0 && tokens.Peek() == ")" && Regex.Match(str, @"\d(?:\.\d)?").Success)
                    tokens.Push("*");
                //close parenthesis -> open parenthesis
                if (str == "(" && tokens.Count > 0 && tokens.Peek() == ")")
                    tokens.Push("*");


                //error handling
                if (operators.Contains(str) && tokens.Count > 0 && operators.Contains(tokens.Peek()))
                    throw new ArgumentException($"Unexpected operator {str}");

                //is it a function not followed by (?
                if (tokens.Count > 0 && tokens.Peek().Any(char.IsLetter) && str != "(")
                    throw new ArgumentException($"Expected (, got {str}");

                tokens.Push(str);
            }
            tokens = tokens.Reverse();
            variables_constants = new Dictionary<string, string>(constants);

            return shunting_yard(tokens);
        }

        private static List<string> shunting_yard(Stack<string> tokens) {
            Stack<string> operator_stack = new();
            List<string> rpn = new();

            while (tokens.Count > 0) {
                string token = tokens.Pop();
                if (decimal.TryParse(token, out _)) {
                    rpn.Add(token);

                } else if (token.All(char.IsLetter)) {
                    //operator
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
                                throw new ArgumentException("Mismatched parenthesis");
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
                    throw new ArgumentException("Mismatched Parentheses");
                }

                rpn.Add(operator_stack.Pop());
            }

            return rpn;
        }
    }
}

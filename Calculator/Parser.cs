using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using System.Text;
using System.Threading.Tasks;
using DecimalMath;
using System.Text.RegularExpressions;

namespace Calculator {
    class Parser {
        //parse a string into reverse polish notation
        private readonly Lexer lexer;
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
        private static Dictionary<string, decimal> constants = new() {
            ["e"] = DecimalEx.E,
            ["pi"] = DecimalEx.Pi,
            ["tau"] = DecimalEx.Pi * 2,
        };

        public List<string> Parse() {
            Stack<string> tokens = new();
            while (!lexer.empty) {
                tokens.Push(lexer.Next());
            }
            tokens = tokens.Reverse();
            lexer.Restart();
            return shunting_yard(tokens);
        }

        private List<string> shunting_yard(Stack<string> tokens) {
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

            if (tokens.Count == 0) {
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

        private string replace_constants(string equation) {
            foreach (KeyValuePair<string, decimal> constant in constants) {
                while (equation.Contains(constant.Key)) {
                    int index = equation.IndexOf(constant.Key);
                    if ((index == 0 || index == equation.Length - constant.Key.Length) || (!char.IsLetter(equation[index - 1]) && !char.IsLetter(equation[index + constant.Key.Length]))) {
                        equation = equation.Remove(index, constant.Key.Length);
                        equation = equation.Insert(index, constant.Value.ToString());
                    }
                }
            }

            return equation;
        }

        //private string parenthesis_mult(string equation) {
        //    /*
        //    number -> open parenthesis 3(
        //    close parenthesis -> number )3 || close parenthesis -> .
        //    close parenthesis -> open parenthesis )(  
        //    */

        //    string prev_str = equation;
        //    while (prev_str != equation) {
        //        prev_str = equation;
        //        //number -> open parenthesis 3(
        //        equation = Regex.Replace(equation, "([0-9])[(]", "$1*(");
        //        //close parenthesis -> number )3
        //        equation = Regex.Replace(equation, "[)]([0-9])", "")
        //    }
            





        //    Match close_num = Regex.Match(equation, "[)][0-9]");
        //    Match close_open = Regex.Match(equation, "[)][(]");
        //}

        public Parser(string str) {
            str = replace_constants(str);
            lexer = new Lexer(str);
        }
    }
}

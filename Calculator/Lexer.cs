﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ExtensionMethods;

namespace Calculator {
    //splits string into tokens
    //sin(max(22,3)/3*pi) => ["sin", "(", "max", "(", "22", ",", "3", ")", "/", "3", "*", "pi"]
    static class Lexer {
        private static string src;
        private static int cursor_begin;
        private static int cursor_end;
        private static bool empty => cursor_end >= src.Length;
        private static string substr => src[cursor_begin..cursor_end];

        private static string next(Dictionary<string, string> variables_constants) {
            reset();
            char x = src[cursor_end];

            if (char.IsLetter(x)) {
                advance_while(y => char.IsLetter(y) && !variables_constants.TryGetValue(src[cursor_begin..cursor_end], out _));
                return substr;

            }

            if (char.IsDigit(x)) {
                advance_while(char.IsDigit);
                //this instead of one lambda so you only get 1 .
                if (empty || src[cursor_end] != '.') return substr;
                cursor_end++;
                advance_while(char.IsDigit);
                return substr;

            }

            //fractional num (.01)
            cursor_end++;
            if (x == '.' && !empty && char.IsDigit(src[cursor_end])) {
                advance_while(char.IsDigit);
                return substr;
            }

            if (x is not ('+' or '-' or '/' or '*' or '^' or ',' or '(' or ')'))
                throw new SyntaxException($"Unexpected char {x}");

            return x.ToString();
        }

        private static void restart() {
            cursor_begin = cursor_end = 0;
            src = "";
        }

        private static void reset() {
            cursor_begin = cursor_end;
        }

        private static void advance_while(Predicate<char> p) {
            while (!empty) {
                if (p(src[cursor_end])) {
                    cursor_end++;

                } else {
                    break;

                }
            }
        }

        public static Stack<string> Lex(string equation, Dictionary<string, string> variables_constants) {
            src = equation;

            Stack<string> stack = new();
            while (!empty) {
                stack.Push(next(variables_constants));
            }

            stack = stack.Reverse();
            restart();
            return stack;
        }
    }
}

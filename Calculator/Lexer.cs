using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator {
    class Lexer {
        //split string into tokens
        //sin(max(22,3)/3*pi) => ["sin", "(", "max", "(", "22", ",", "3", ")", "/", "3", "*", "pi"]

        private readonly string src;
        private int cursor_begin;
        private int cursor_end;
        public bool empty => cursor_end >= src.Length;
        private string substr => src[cursor_begin..cursor_end];

        //return next token
        //override the for loop and return next()
        public string Next() {
            reset();
            char x = src[cursor_end];
            if (char.IsLetter(x)) {
                advance_while(char.IsLetter);
                return substr;

            } else if (char.IsDigit(x)) {
                advance_while(char.IsDigit);
                //this instead of one lambda so you only get 1 .
                if (!empty && src[cursor_end] == '.') {
                    cursor_end++;
                    advance_while(char.IsDigit);
                }
                return substr;

            } else {
                cursor_end++;
                //fractional num (.01)
                if (x == '.' && char.IsDigit(src[cursor_end])) {
                    return substr;
                } else {
                    return x.ToString();
                }
            }
        }

        private void reset() {
            cursor_begin = cursor_end;
        }

        private void advance_while(Predicate<char> p) {
            while (!empty) {
                if (p(src[cursor_end])) {
                    cursor_end++;

                } else {
                    break;

                }
            }
        }

        public Lexer(string src) {
            this.src = src;
            cursor_begin = 0;
            cursor_end = cursor_begin;
        }
    }
}

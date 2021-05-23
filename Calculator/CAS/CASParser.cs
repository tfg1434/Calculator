using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator.CAS {
    class CASParser {
        public int Power(string term, string variable) {
            Match match = Regex.Match(term, $@"{variable}\^([0-9]+)");

            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return term == variable ? 1 : 0;
        }

        public string TermArrToString(Term[] array) {
            string print = "";
            foreach ((int coefficient, string term) in array) {
                string print_like_term = term.Replace("^1", "");
                string print_coefficient = coefficient.ToString();
                if (term.Contains("^0")) {
                    //print_coefficient = "1";
                    print_like_term = "";
                }
                if (print_coefficient == "-1" && print_like_term != "") print_coefficient = "-";
                if (print_coefficient == "1" && print_like_term != "") print_coefficient = "";
                if (print_coefficient == "0")
                    print_coefficient = print_like_term = "";

                print += print_coefficient.StartsWith("-") ? print_coefficient + print_like_term : "+" + print_coefficient + print_like_term;
            }
            if (print.StartsWith("+")) print = print.Remove(0, 1);

            return print;
        }

        public string IntArrToString(int[] array, string variable) {
            StringBuilder str = new();
            int ii = array.Length;
            foreach (int co in array) {
                ii--;

                if (co == 0)
                    continue;

                string print_co = co switch {
                    -1 when ii != 0 => "-",
                    1 when ii != 0 => "+",
                    _ => co.ToString()[0] == '-' ? co.ToString() : "+" + co.ToString(),
                };

                string append = ii switch {
                    > 1 => $"{print_co}{variable}^{ii}",
                    1 => $"{print_co}{variable}",
                    0 => print_co,
                    _ => throw new ArgumentOutOfRangeException(),
                };

                str.Append(append);
            }

            if (str[0] == '+')
                str = str.Remove(0, 1);

            return str.ToString();
        }

        public Term[] Parse(string equation) {
            //[+-]?[^+-]+
            //$@"([+-]?(?:(?:(?:\d+)?{variable}\^\d+)|(?:\d+{variable})|(?:\d+)|(?:{variable})))" SPECIFIC VARIABLE
            //([+-]?(?:(?:(?:\d+)?(?:[a-zA-Z]+(?:\^\d+)?)*)*|(?:\d+[a-zA-Z]*)|(?:\d+)|(?:[a-zA-Z]))) FIRST REGEX
            //test if it's a valid polynomial
            if (!IsPolynomial(equation, out MatchCollection matches))
                throw new NotPolynomialException("Input was not a polynomial");

            var ret = new Term[matches.Count - 1];
            for (int i = 0; i < ret.Length; i++) {
                string match = matches[i].Value.Replace("+", "");
                if (!match.StartsWith("-") && !char.IsDigit(match[0])) match = "1" + match;
                else if (match.StartsWith("-") && !char.IsDigit(match[1]))
                    match = match.Insert(1, "1");

                int num = int.Parse(new string(match.SkipWhile(x => x == '-').TakeWhile(char.IsDigit).ToArray()));
                if (match.StartsWith("-"))
                    num *= -1;

                ret[i] = new Term(num, match.Remove(0, num.ToString().Length));
            }
            return ret;
        }

        //is polynomial with any amount of variables?
        public bool IsPolynomial(string str, out MatchCollection matches) {
            matches = Regex.Matches(str, @"([+-]?(?:(?:(?:[0-9]+)?(?:[a-zA-Z]+(?:\^[0-9]+)?)*)*|(?:[0-9]+[a-zA-Z]*)|(?:[0-9]+)|(?:[a-zA-Z])))");

            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            return test == str;
        }

        //is it a polynomial with a specific variable?
        public bool IsPolynomial1Variable(string str, string variable, out MatchCollection matches) {
            matches = Regex.Matches(str, $@"([+-]?(?:(?:(?:[0-9]+)?{variable}\^[0-9]+)|(?:[0-9]+{variable})|(?:[0-9]+)|(?:{variable})))");

            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            return test == str;
        }

        public int[] GetCoefficients(Term[] terms, string variable) {
            SortByExponent(terms, variable);
            int max_exp = Power(terms[0].term, variable);
            int[] coefficients = new int[max_exp + 1]; //a polynomial to the power of 5 should have 6 coefficients

            for (var i = 0; i < coefficients.Length; i++) {
                int index = Array.FindIndex(terms, t => Power(t.term, variable) == max_exp - i);
                if (index != -1)
                    coefficients[i] = terms[index].coefficient;
                else
                    coefficients[i] = 0;
            }

            return coefficients;


            void SortByExponent(Term[] array, string var) {
                int comparer(Term t1, Term t2) {
                    int a = Power(t1.term, var);
                    int b = Power(t2.term, var);

                    return b - a;
                }

                Array.Sort(array, comparer);
            }
        }
    }
}

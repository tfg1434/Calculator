using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static Calculator.Solver;

namespace Calculator {
    static class CAS {
        public readonly struct Term {
            public int coefficient { get; }
            public string term { get; }

            //there's no access modifier to do exactly what i want, so we live with internal :p
            internal Term(int coefficient, string term) {
                this.coefficient = coefficient;
                this.term = term;
            }
        }

        private static int power(string term, string variable) {
            if (term.Contains("^"))
                return int.Parse(term.Split("^")[1]);
            else
                return term == variable ? 1 : 0;
        }

        //is polynomial with any amount of variables?
        private static bool is_polynomial(string str, out MatchCollection matches) {
            matches = Regex.Matches(str, @"([+-]?(?:(?:(?:\d+)?(?:[a-zA-Z]+(?:\^\d+)?)*)*|(?:\d+[a-zA-Z]*)|(?:\d+)|(?:[a-zA-Z])))");

            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            return test == str;
        }

        //is it a polynomial with a specific variable?
        private static bool is_polynomial_1variable(string str, string variable, out MatchCollection matches) {
            matches = Regex.Matches(str, $@"([+-]?(?:(?:(?:\d+)?{variable}\^\d+)|(?:\d+{variable})|(?:\d+)|(?:{variable})))");

            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            return test == str;
        }

        //ints only pls
        private static Term[] parse(string equation) {
            //[+-]?[^+-]+
            //$@"([+-]?(?:(?:(?:\d+)?{variable}\^\d+)|(?:\d+{variable})|(?:\d+)|(?:{variable})))" SPECIFIC VARIABLE
            //([+-]?(?:(?:(?:\d+)?(?:[a-zA-Z]+(?:\^\d+)?)*)*|(?:\d+[a-zA-Z]*)|(?:\d+)|(?:[a-zA-Z]))) FIRST REGEX
            //test if it's a valid polynomial
            if (!is_polynomial(equation, out MatchCollection matches))
                throw new Exception("Invalid input");

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

        private static void sort_by_exponent(Term[] array, string variable) {
            int comparer(Term t1, Term t2) {
                int a = power(t1.term, variable);
                int b = power(t2.term, variable);

                return b - a;
            }

            Array.Sort(array, comparer);
        }

        //applies factor theorem and returns a in x - a
        private static int factor_theorem(string equation, string variable, out bool possible) {
            //*= -1 +- 1
            int cur = 0;
            Dictionary<string, string> sub = new() {
                [variable] = cur.ToString().Replace("-", "~"),
            };

            while (Solve(equation, sub) != 0) {
                cur = -(cur + (cur >= 0 ? 1 : 0));
                sub[variable] = cur.ToString().Replace("-", "~");

                if (cur > 9999) {
                    possible = false;
                    return 0;
                }
            }

            possible = true;
            return cur;
        }

        //you can infer powers from here since there are 0s
        //last term is remainder (if not 0 error)
        //consider adding an out print parameter to this and making it public
        private static int[] synthetic_div(int zero, int[] coefficients, out int rem) {
            var ans = new int[coefficients.Length - 1]; //-1 because last term is remainder
            ans[0] = coefficients[0];

            for (int i = 1; i < ans.Length; i++) {
                ans[i] = coefficients[i] + ans[i - 1] * zero;
            }
            rem = coefficients[^1] + ans[^1] * zero;

            return ans;
        }

        private static int[] get_coefficients(Term[] terms, string variable) {
            sort_by_exponent(terms, variable);
            int max_exp = power(terms[0].term, variable);
            int[] coefficients = new int[max_exp + 1]; //a polynomial to the power of 5 should have 6 coefficients

            for (var i = 0; i < coefficients.Length; i++) {
                int index = Array.FindIndex(terms, t => power(t.term, variable) == max_exp - i);
                if (index != -1)
                    coefficients[i] = terms[index].coefficient;
                else
                    coefficients[i] = 0;
            }

            return coefficients;
        }

        //so that i can use Solve with an int[]
        //[1, 5, 6] => x^2+5x+6
        private static string int_arr_to_str(int[] array, string variable) {
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
                    _ => throw new Exception("wtf"),
                };

                str.Append(append);
            }

            if (str[0] == '+')
                str = str.Remove(0, 1);

            return str.ToString();
        }

        //public version (returns a string to print)
        public static string SyntheticDiv(string equation, string variable, int zero, out int rem) {
            if (!is_polynomial_1variable(equation, variable, out _))
                throw new Exception("Invalid input");
            Term[] terms = CombineLikeTerms(equation, out _);
            sort_by_exponent(terms, variable);
            int[] coefficients = get_coefficients(terms, variable);
            int[] ans = synthetic_div(zero, coefficients, out int remainder);
            rem = remainder;

            return int_arr_to_str(ans, variable);
        }

        //ints only pls
        public static string PolyFactor(string equation, string variable) {
            if (!is_polynomial_1variable(equation, variable, out _))
                throw new Exception("Invalid input");

            Term[] unfactored = CombineLikeTerms(equation, out _);
            sort_by_exponent(unfactored, variable);

            int[] coefficients = get_coefficients(unfactored, variable);

            int zero = factor_theorem(equation, variable, out _);

            //looks like x^2+5x+6 => x+3 => null
            int[] curr_equation = synthetic_div(zero, coefficients, out int rem);
            //looks like (x+3) => (x+3)(x+2)
            string ans = $"({variable}{(-zero < 0 ? -zero : "+" + -zero)})";
            if (rem != 0)
                throw new Exception("Not factorable!");

            bool broke = false;
            while (true) {
                //factor theorem will break if it's not possible
                zero = factor_theorem(int_arr_to_str(curr_equation, variable), variable, out bool possible);
                if (!possible) {
                    broke = true;
                    break;
                }
                curr_equation = synthetic_div(zero, curr_equation, out _);
                ans += $"({variable}{(-zero < 0 ? -zero : "+" + -zero)})";

                if (curr_equation.Length > 0 && curr_equation[0] == 1)
                    break;
            }
            if (broke) {
                ans += $"({int_arr_to_str(curr_equation, variable)})";
            }

            return ans;
        }

        //ints only pls
        public static Term[] CombineLikeTerms(string equation, out string print) {
            Dictionary<string, int> like_terms = new();
            Term[] uncombined = parse(equation);
            foreach (Term term in uncombined) {
                if (like_terms.ContainsKey(term.term))
                    like_terms[term.term] += term.coefficient;
                else
                    like_terms[term.term] = term.coefficient;
            }

            var ret = new Term[like_terms.Count];
            int i = 0;
            foreach (KeyValuePair<string, int> pair in like_terms) {
                ret[i++] = new Term(pair.Value, pair.Key);
            }

            //out print
            print = "";
            foreach (Term term in ret) {
                string print_like_term = term.term.Replace("^1", "");
                string print_coefficient = term.coefficient.ToString();
                if (term.term.Contains("^0")) {
                    //print_coefficient = "1";
                    print_like_term = "";
                }
                if (print_coefficient == "-1" && print_like_term != "") print_coefficient = "-";
                if (print_coefficient == "1" && print_like_term != "") print_coefficient = "";

                print += print_coefficient.StartsWith("-") ? print_coefficient + print_like_term : "+" + print_coefficient + print_like_term;
            }
            if (print.StartsWith("+")) print = print.Remove(0, 1);

            return ret;
        }
    }
}

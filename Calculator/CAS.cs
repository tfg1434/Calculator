using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator {
    static class CAS {
        public struct Term {
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
            if (test != str)
                return false;

            return true;
        }

        //is it a polynomial with a specific variable?
        private static bool is_polynomial_1variable(string str, string variable, out MatchCollection matches) {
            matches = Regex.Matches(str, $@"([+-]?(?:(?:(?:\d+)?{variable}\^\d+)|(?:\d+{variable})|(?:\d+)|(?:{variable})))");

            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            if (test != str)
                return false;

            return true;
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

        //ints only pls
        public static void/*(int coefficient, string term)[]*/ PolyFactor(string equation, string variable, out string print) {
            if (!is_polynomial_1variable(equation, variable, out _))
                throw new Exception("Invalid input");

            Term[] unfactored = parse(equation);
            sort_by_exponent(unfactored, variable);
            int max_exp = power(unfactored[0].term, variable);
            int[] coefficients = new int[max_exp + 1]; //a polynomial to the power of 5 should have 6 coefficients
            coefficients[0] = unfactored[0].coefficient;

            for (var i = 0; i < coefficients.Length; i++) {
                int index = Array.FindIndex(unfactored, t => power(t.term, variable) == max_exp - i);
                if (index != -1)
                    coefficients[i] = unfactored[index].coefficient;
                else
                    coefficients[i] = 0;
            }


            print = "";
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

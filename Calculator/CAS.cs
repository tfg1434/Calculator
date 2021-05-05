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

        //ints only pls
        private static Term[] parse(string equation, string variable) {
            // [+-]?[^+-]+ doesn't work, but try to make parse work for any \d<wildcards> pair
            MatchCollection matches = Regex.Matches(equation, $@"([+-]?(?:(?:(?:\d+)?{variable}\^\d+)|(?:\d+{variable})|(?:\d+)|(?:{variable})))");
            //test if it's a valid polynomial
            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            if (test != equation)
                throw new Exception("Invalid input");

            var ret = new Term[matches.Count];
            for (int i = 0; i < matches.Count; i++) {
                string match = matches[i].Value.Replace("+", "");
                if (match.StartsWith(variable)) match = "1" + match;
                else if (match.StartsWith("-" + variable)) 
                    match = match.Insert(1, "1");
                ret[i] = new Term(int.Parse(match.Split(variable)[0]), match.Remove(0, match.Split(variable)[0].Length));
            }
            return ret;
        }

        private static void sort_by_exponent(Term[] array, string variable) {
            int comparer(Term t1, Term t2) {
                int a = default;
                if (t1.term.Contains("^"))
                    a = int.Parse(t1.term.Split("^")[1]);
                else
                    a = t1.term == variable ? 1 : 0;
                int b = default;
                if (t2.term.Contains("^"))
                    b = int.Parse(t2.term.Split("^")[1]);
                else
                    b = t2.term == variable ? 1 : 0;

                return b - a;
            }

            Array.Sort(array, comparer);
        }

        //ints only pls
        public static void/*(int coefficient, string term)[]*/ PolyFactor(string equation, string variable, out string print) {
            Term[] unfactored = parse(equation, variable);
            sort_by_exponent(unfactored, variable);



            print = "";
        }

        //ints only pls
        public static Term[] CombineLikeTerms(string equation, string variable, out string print) {
            Dictionary<string, int> like_terms = new();
            Term[] uncombined = parse(equation, variable);
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

            sort_by_exponent(ret, variable);

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

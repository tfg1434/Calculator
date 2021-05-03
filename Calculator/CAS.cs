using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Calculator {
    static class CAS {
        //ints only pls
        private static (int coefficient, string term)[] parse(string equation, string variable="x") {
            MatchCollection matches = Regex.Matches(equation, $@"([+-]?(?:(?:(?:\d+)?{variable}\^\d+)|(?:\d+{variable})|(?:\d+)|(?:{variable})))");
            //test if it's a valid polynomial
            string test = "";
            foreach (Match match in matches)
                test += match.Value;
            if (test != equation)
                throw new Exception("Invalid input");

            var ret = new (int coefficient, string term)[matches.Count];
            for (int i = 0; i < matches.Count; i++) {
                string match = matches[i].Value.Replace("+", "");
                if (match.StartsWith(variable)) match = "1" + match;
                else if (match.StartsWith("-" + variable)) 
                    match = match.Insert(1, "1");
                ret[i].coefficient = int.Parse(match.Split(variable)[0]);
                ret[i].term = match.Remove(0, match.Split(variable)[0].Length);
            }
            return ret;
        }

        //ints only pls
        public static string PolyFactor(string equation, string variable) {

        }

        //ints only pls
        public static (int coefficient, string like_term)[] CombineLikeTerms(string equation, string variable, out string print) {
            Dictionary<string, int> like_terms = new();
            (int coefficient, string term)[] uncombined = parse(equation, variable);
            foreach ((int coefficient, string term) val in uncombined) {
                if (like_terms.ContainsKey(val.term))
                    like_terms[val.term] += val.coefficient;
                else
                    like_terms[val.term] = val.coefficient;
            }

            var ret = new (int coefficient, string like_term)[like_terms.Count];
            int i = 0;
            foreach (KeyValuePair<string, int> pair in like_terms) {
                ret[i++] = (pair.Value, pair.Key);
            }

            //out print
            print = "";
            foreach ((int coefficient, string like_term) term in ret) {
                string print_like_term = term.like_term.Replace("^1", "");
                string print_coefficient = term.coefficient.ToString();
                if (term.like_term.Contains("^0")) {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static Calculator.Solver;

namespace Calculator.CAS {
    class BinomialTheorem {
        public string Expand(string equation) {
            Match match = Regex.Match(equation, @"\((\-?(?:[a-zA-Z]{1,}|\d[a-zA-Z]*))([+-](?:[a-zA-Z]{1,}|\d[a-zA-Z]*))\)\^(\d)");
            if (!match.Success)
                throw new NotPolynomialException("Input wasn't a binomial to a power.");

            return binomial_theorem(equation, match);
        }

        private static string binomial_theorem(string equation, Match match) {
            StringBuilder ans = new();
            uint max_exp = uint.Parse(match.Groups[^1].Value);
            bool negative = match.Groups[1].Value[0] == '-' ^ match.Groups[2].Value[0] == '-';

            for (uint i = 0; i < max_exp + 1; i++) {
                uint pow1 = max_exp - i;

                int co = (int)NCr(max_exp, max_exp - i);
                //if (negative)
                //    co = i % 2 == 0 ? co : -co;
                Term term1 = power(match.Groups[1].Value, pow1);
                Term term2 = power(match.Groups[2].Value, i);


                string print_term1 = term1.term.EndsWith("^0") ? "" : term1.term;
                print_term1 = print_term1.Replace("^1", "");
                string print_term2 = term2.term.EndsWith("^0") ? "" : term2.term;
                print_term2 = print_term2.Replace("^1", "");
                string print_co = co * term1.coefficient * term2.coefficient == 1
                    ? ""
                    : (co * term1.coefficient * term2.coefficient).ToString();
                if (print_co != "" && print_co[0] != '-')
                    print_co = print_co.Insert(0, "+");

                ans.Append($"{print_co}{print_term1}{print_term2}");
            }

            if (ans[0] == '+')
                ans.Remove(0, 1);

            int index = equation.IndexOf(match.Value, StringComparison.Ordinal);
            equation = equation.Remove(index, match.Length).Insert(index, ans.ToString());

            return equation;
        }

        //raises a monomial to a power
        private static Term power(string val, uint pow) {
            if (val.All(char.IsDigit))
                return new Term(IntPow(int.Parse(val), pow), "");

            if (val.All(char.IsLetter))
                return new Term(1, $"{val}^{pow}");

            string concat = get_first_num(val);
            int parsed = concat == "" ? 1 : int.Parse(concat);
            int co = IntPow(parsed, pow);

            string terms_string = concat != "" ? val.Replace(concat, "") : concat;
            var terms = new string[terms_string.Count(char.IsLetter)];
            //x^2y^2z => [x^2, y^2, z]
            //slow af
            int i = 0;
            MatchCollection matches = Regex.Matches(terms_string, @"[a-zA-Z]\^\d");
            foreach (Match match in matches) {
                terms[i++] = match.Value;
                terms_string = terms_string.Remove(terms_string.IndexOf(match.Value, StringComparison.Ordinal), match.Length);
            }
            foreach (char var in terms_string) {
                terms[i++] = var.ToString();
            }


            i = 0;
            foreach (string term in terms) {
                //term looks like this: x^2 or z
                char power = term.FirstOrDefault(char.IsDigit);
                string other = string.Concat(term.Where(x => !char.IsDigit(x)));

                terms[i] = power switch {
                    '0' => "",
                    default(char) or '1' => $"{term}^{pow}",
                    _ => other + (power * pow).ToString(),
                };

                i++;
            }


            StringBuilder ans_terms = new();
            foreach (string term in terms)
                ans_terms.Append(term);
            return new Term(co, ans_terms.ToString());
        }

        private static string get_first_num(string input) {
            Match match = Regex.Match(input, @"^(?:\d|\+|\-)+");
            return match.Success ? match.Value : "";
        }
    }
}

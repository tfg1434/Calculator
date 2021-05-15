using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static Calculator.Solver;
// ReSharper disable All

namespace Calculator.CAS {
    class Simplifier {
        private readonly CASParser parser = new();

        public Term[] Simplify(string equation, out string print) {
            equation = binomial_theorem(equation);

            if (!parser.IsPolynomial(equation, out _))
                throw new Exception("Invalid input");

            Term[] terms = parser.Parse(equation);
            Term[] ans = combine_like_terms(terms);

            print = parser.TermArrToString(ans);
            return ans;
        }

        public Term[] Simplify1Variable(string equation, string variable, out string print) {
            if (!parser.IsPolynomial1Variable(equation, variable, out _))
                throw new Exception("Invalid input");

            Term[] terms = parser.Parse(equation);
            Term[] ans = combine_like_terms(terms);
            sort_by_exponent(ans, variable);

            print = parser.TermArrToString(ans);
            return ans;
        }

        private static string binomial_theorem(string equation) {
            MatchCollection matches = Regex.Matches(equation, @"\(([a-zA-Z]*|\d|\d[a-zA-Z]*)([+-])([a-zA-Z]*|\d|\d[a-zA-Z]*)\)\^(\d)");
            StringBuilder ans = new();

            foreach (Match match in matches) {
                int max_exp = int.Parse(match.Groups[^1].Value);

                for (int i = 0; i < max_exp + 1; i++) {
                    int pow1 = max_exp - i;
                    int pow2 = i;

                    int co = NCr(max_exp, max_exp - i);
                    Term term1 = power(match.Groups[1].Value, pow1);
                    Term term2 = power(match.Groups[3].Value, pow2);

                    ans.Append($"{co * term1.coefficient * term2.coefficient}{term1.term}{term2.term}");
                }
            }

            //pow1 = new int[]
            return "";
            //TODO: FIX BUGS
            //TODO: NEGATIVE (x-a)^2
        }

        private static Term power(string val, int pow) {
            if (val.All(char.IsDigit))
                return new Term(int_pow(int.Parse(val), (uint)pow), "");

                //int co = int_pow(int.Parse(string.Concat(val.TakeWhile(char.IsDigit))), (uint)pow);
            string concat = string.Concat(val.TakeWhile(char.IsDigit));
            int parsed = concat == "" ? 1 : int.Parse(concat);
            int co = int_pow(parsed, (uint)pow);

            string terms_string = val.Replace(co.ToString(), "");
            var terms = new string[terms_string.Count(char.IsLetter)];
            //x^2y^2z => [x^2, y^2, z]
            //slow af
            int i = 0;
            MatchCollection matches = Regex.Matches(terms_string, @"[a-zA-Z]\^\d");
            foreach (Match match in matches) {
                terms[i++] = match.Value;
                terms_string = terms_string.Remove(terms_string.IndexOf(match.Value), match.Length);
            }
            foreach (char var in terms_string.ToCharArray()) {
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

        private static int int_pow(int x, uint pow) {
            int ret = 1;
            while (pow != 0) {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }

        private void sort_by_exponent(Term[] array, string variable) {
            int comparer(Term t1, Term t2) {
                int a = parser.Power(t1.term, variable);
                int b = parser.Power(t2.term, variable);

                return b - a;
            }

            Array.Sort(array, comparer);
        }

        private static Term[] combine_like_terms(Term[] terms) {
            Dictionary<string, int> like_terms = new();
            foreach (Term term in terms) {
                if (like_terms.ContainsKey(term.term))
                    like_terms[term.term] += term.coefficient;
                else
                    like_terms[term.term] = term.coefficient;
            }

            var ret = new Term[like_terms.Count];
            int i = 0;
            foreach ((string key, int value) in like_terms) {
                ret[i++] = new Term(value, key);
            }

            return ret;
        }
    }
}

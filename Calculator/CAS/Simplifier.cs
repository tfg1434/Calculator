using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static Calculator.Solver;

namespace Calculator.CAS {
    class Simplifier {
        private readonly CASParser parser = new();

        public Term[] Simplify(string equation, out string print) {
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

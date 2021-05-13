using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class Simplifier {
        private readonly CASParser parser = new();

        public Term[] Simplify(string equation, out string print) {
            Term[] ans = combine_like_terms(equation);

            print = parser.TermArrToString(ans);
            return ans;
        }

        public Term[] Simplify1Variable(string equation, string variable, out string print) {
            if (!parser.IsPolynomial1Variable(equation, variable, out _))
                throw new Exception("Invalid input");

            Term[] ans = combine_like_terms(equation);
            SortByExponent(ans, variable);

            print = parser.TermArrToString(ans);
            return ans;
        }

        public void SortByExponent(Term[] array, string variable) {
            int comparer(Term t1, Term t2) {
                int a = parser.Power(t1.term, variable);
                int b = parser.Power(t2.term, variable);

                return b - a;
            }

            Array.Sort(array, comparer);
        }

        private Term[] combine_like_terms(string what) {
            Dictionary<string, int> like_terms = new();
            Term[] uncombined = parser.Parse(what);
            foreach (Term term in uncombined) {
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

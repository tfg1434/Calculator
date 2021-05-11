using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class Simplifier : CAS {
        private readonly CASParser parser = new();

        private Term[] combine_like_terms(string equation, out string print) {
            Dictionary<string, int> like_terms = new();
            Term[] uncombined = parser.Parse(equation);
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

            print = parser.TermArrToString(ret);
            return ret;
        }

        public Term[] Simplify(string equation, out string print) {
            Term[] ans = combine_like_terms(equation, out print);

            return ans;
        }
    }
}

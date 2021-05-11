using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class SyntheticDiv : CAS {
        private readonly CASParser parser = new();
        private readonly Simplifier simplify = new();

        public int[] Divide(int zero, int[] coefficients, out int rem) {
            var ans = new int[coefficients.Length - 1]; //-1 because last term is remainder
            ans[0] = coefficients[0];

            for (int i = 1; i < ans.Length; i++) {
                ans[i] = coefficients[i] + ans[i - 1] * zero;
            }
            rem = coefficients[^1] + ans[^1] * zero;

            return ans;
        }

        public string Divide(string equation, string variable, int zero, out int rem) {
            if (!parser.IsPolynomial1Variable(equation, variable, out _))
                throw new Exception("Invalid input");

            Term[] terms = simplify.Simplify(equation, out _);
            parser.SortByExponent(terms, variable);
            int[] coefficients = parser.GetCoefficients(terms, variable);
            int[] ans = Divide(zero, coefficients, out int remainder);
            rem = remainder;

            return parser.IntArrToString(ans, variable);
        }
    }
}

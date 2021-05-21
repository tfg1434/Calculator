using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class SyntheticDiv {
        private readonly CASParser parser = new();
        private readonly Simplifier simplifier = new();

        public string Div(string equation, string variable, int zero, out int rem) {
            if (!parser.IsPolynomial1Variable(equation, variable, out _))
                throw new NotPolynomial1VariableException("Invalid input, wasn't a polynomial with 1 variable");

            Term[] terms = simplifier.Simplify1Variable(equation, variable, out _);
            int[] coefficients = parser.GetCoefficients(terms, variable);
            int[] ans = Div(zero, coefficients, out int remainder);
            rem = remainder;

            return parser.IntArrToString(ans, variable);
        }

        public int[] Div(int zero, int[] coefficients, out int rem) {
            var ans = new int[coefficients.Length - 1]; //-1 because last term is remainder
            ans[0] = coefficients[0];

            for (int i = 1; i < ans.Length; i++) {
                ans[i] = coefficients[i] + ans[i - 1] * zero;
            }
            rem = coefficients[^1] + ans[^1] * zero;

            return ans;
        }
    }
}

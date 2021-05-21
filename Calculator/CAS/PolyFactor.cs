using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class PolyFactor {
        private readonly CASParser parser = new();
        private readonly Simplifier simplifier = new();
        private readonly SyntheticDiv synthetic_divider = new();

        public string Factor(string equation, string variable) {
            if (!parser.IsPolynomial1Variable(equation, variable, out _))
                throw new NotPolynomial1VariableException("Invalid input");

            Term[] unfactored = simplifier.Simplify1Variable(equation, variable, out _);

            int[] coefficients = parser.GetCoefficients(unfactored, variable);

            int zero = get_factor(coefficients, out _);

            //looks like x^2+5x+6 => x+3 => null
            int[] curr_equation = synthetic_divider.Div(zero, coefficients, out int rem);
            //looks like (x+3) => (x+3)(x+2)
            string ans = $"({variable}{(-zero < 0 ? -zero : "+" + -zero)})";
            if (rem != 0)
                throw new NotPossibleException("Polynomial was not factorable!");

            while (true) {
                //factor theorem will break if it's not possible
                zero = get_factor(curr_equation, out bool possible);
                if (!possible) {
                    break;
                }
                curr_equation = synthetic_divider.Div(zero, curr_equation, out _);
                ans += $"({variable}{(-zero < 0 ? -zero : "+" + -zero)})";

                if (curr_equation.Length == 1 && curr_equation[0] == 1)
                    break;
            }
            if (!(curr_equation.Length == 1 && curr_equation[0] == 1))
                ans += $"({parser.IntArrToString(curr_equation, variable)})";

            return ans;
        }

        private int get_factor(int[] co, out bool possible) {
            int cur = 0;

            int remainder = 1;
            while (remainder != 0) {
                cur = -(cur + (cur >= 0 ? 1 : 0));
                synthetic_divider.Div(cur, co, out remainder);
                //SyntheticDiv(string equation, string var, int zero, out int rem)

                if (cur > 9999) {
                    possible = false;
                    return 0;
                }
            }

            possible = true;
            return cur;
        }
    }
}

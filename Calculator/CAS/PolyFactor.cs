using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class PolyFactor : CAS {
        private readonly SyntheticDiv synthetic_divider = new();
        private readonly CASParser parser = new();
        private readonly Simplifier simplify = new();

        private int get_factor(string equation, string variable, out bool possible) {
            int cur = 0;

            int rem = 1;
            while (rem != 0) {
                cur = -(cur + (cur >= 0 ? 1 : 0));
                synthetic_divider.Divide(equation, variable, cur, out rem);

                if (cur > 9999) {
                    possible = false;
                    return 0;
                }
            }

            possible = true;
            return cur;
        }

        public string Factor(string equation, string variable) {
            if (!parser.IsPolynomial1Variable(equation, variable, out _))
                throw new Exception("Invalid input");

            Term[] unfactored = simplify.Simplify(equation, out _);
            parser.SortByExponent(unfactored, variable);

            int[] coefficients = parser.GetCoefficients(unfactored, variable);

            int zero = get_factor(equation, variable, out _);

            //looks like x^2+5x+6 => x+3 => null
            int[] curr_equation = synthetic_divider.Divide(zero, coefficients, out int rem);
            //looks like (x+3) => (x+3)(x+2)
            string ans = $"({variable}{(-zero < 0 ? -zero : "+" + -zero)})";
            if (rem != 0)
                throw new Exception("Not factorable!");

            bool broke = false;
            while (true) {
                //factor theorem will break if it's not possible
                zero = get_factor(parser.IntArrToString(curr_equation, variable), variable, out bool possible);
                if (!possible) {
                    broke = true;
                    break;
                }
                curr_equation = synthetic_divider.Divide(zero, curr_equation, out _);
                ans += $"({variable}{(-zero < 0 ? -zero : "+" + -zero)})";

                if (curr_equation.Length > 0 && curr_equation[0] == 1)
                    break;
            }
            if (broke) {
                ans += $"({parser.IntArrToString(curr_equation, variable)})";
            }

            return ans;
        }
    }
}

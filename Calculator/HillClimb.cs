﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Calculator.Solver;

namespace Calculator {
    static class HillClimb {
        private static decimal[] neighbourhood(decimal curr, decimal dist) => new[] { curr + dist, curr - dist };

        public static decimal HillClimbing(string equation, Dictionary<string, string> variables, string unknown, decimal resolution = 0.1M) {
            decimal val = 0;

            //parse/replace everything except unknown
            Dictionary<string, string> variables_no_unknown = new Dictionary<string, string>(variables);
            variables[unknown] = val.ToString();
            List<string> parsed = Parser.InsertVariablesConstants(Parser.Parse(equation, new Dictionary<string, string>(variables)), variables_no_unknown);

            decimal step = resolution;
            decimal min = Math.Abs(Solve(Parser.ShuntingYard(Parser.InsertVariablesConstants(new List<string>(parsed), variables))));

            while (true) {
                bool new_neighbour = false;

                foreach (decimal neighbour in neighbourhood(val, step)) {
                    variables[unknown] = neighbour.ToString();
                    decimal new_min = Math.Abs(Solve(Parser.ShuntingYard(Parser.InsertVariablesConstants(new List<string>(parsed), variables))));

                    if (new_min < min) {
                        min = new_min;
                        val = neighbour;
                        new_neighbour = true;
                    }
                }

                if (min == 0)
                    return val;

                if (!new_neighbour) {
                    if (step <= new decimal(1, 0, 0, false, 28))
                        //safe to return, hit epsilon
                        return min <= new decimal(1, 0, 0, false, 28)
                            ? val
                            : throw new NotPossibleException("no solution found");

                    step /= 2;
                }
            }
        }
    }
}

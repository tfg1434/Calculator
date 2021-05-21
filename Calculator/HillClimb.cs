using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Calculator.Solver;

namespace Calculator {
    static class HillClimb {
        private static decimal[] neighbourhood(decimal curr, decimal dist) => new[] { curr + dist, curr - dist };

        //public static decimal HillClimbing(string equation, Dictionary<string, string> variables, string unknown, decimal resolution=0.1M) {
        //    decimal step = resolution;
        //    decimal min = Solve(Parser.Parse(equation, variables));

        //    while (true) {
        //        bool new_neighbour = false;

        //        foreach (decimal neighbour in neighbourhood())
        //    }
        //}

        //write an overload that takes no variables, just unknown and List<string>
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using DecimalMath;
using System.Text.RegularExpressions;
using static Calculator.Solver;

namespace Calculator {
    class Program {
        static void Main(string[] args) {
            string command = args[0];
            string equation = args[1];

            switch (command) {
                case "eval": {
                    Dictionary<string, string> variables = new();
                    for (var i = 2; i < args.Length; i++) {
                        //variable looks like a=5
                        variables.Add(
                            args[i].Split("=")[0],
                            args[i].Split("=")[1]
                        );
                    }

                    Console.WriteLine(Solve(equation, variables));
                    break;
                }
                case "cas":
                    switch (args[2]) {
                        case "combineliketerms":
                            CAS.CombineLikeTerms(equation, out string print);
                            Console.WriteLine(print);
                            break;
                        case "polyfactor":
                        {
                            string variable = args.ElementAtOrDefault(3);
                            if (string.IsNullOrEmpty(variable)) variable = "x";
                            CAS.PolyFactor(equation, variable, out _);
                            break;
                        }
                    }

                    break;
            }
        }

        
    }
}

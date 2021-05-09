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
                        case "combineliketerms": {
                            /*
                            syntax: cas <equation> combineliketerms
                            description: combines like terms
                            notes: ints only
                            */

                            CAS.CombineLikeTerms(equation, out string print);
                            Console.WriteLine(print);
                            break;
                        }
                            
                        case "polyfactor": {
                            /*
                            syntax: cas <equation> polyfactor <variable=x>
                            description: factors a polynomial using factor theorem and synthetic division
                            notes: ints only
                            */

                            string variable = args.ElementAtOrDefault(3);
                            if (string.IsNullOrEmpty(variable)) 
                                variable = "x";
                            Console.WriteLine(CAS.PolyFactor(equation, variable));
                            break;
                        }
                        case "syntheticdiv": {
                            /*
                            syntax: cas <equation> syntheticdiv <zero> <variable=x>
                            description: uses synthetic division to divide a polynomial by x-a
                            notes: ints only
                            */

                            int zero = int.Parse(args[3]);
                            string variable = args.ElementAtOrDefault(4);
                            if (string.IsNullOrEmpty(variable))
                                variable = "x";
                            string print = CAS.SyntheticDiv(equation, variable, zero, out int rem);
                            Console.WriteLine($"{print} | remainder: {rem}");
                            break;
                        }
                        case "binomialtheorem": {
                            /*
                            syntax: cas <equation> binomialtheorem
                            description: expands a binomial that was put to a power
                            notes: ints only
                            */
                            CAS.BinomialTheorem(equation, out _);
                            break;
                        }
                    }

                    break;
            }
        }

        
    }
}

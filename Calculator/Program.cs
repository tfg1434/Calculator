using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Calculator.CAS;
using static Calculator.Solver;

namespace Calculator {
    static class Program {
        static void Main(string[] args) {
            string command = args.ElementAtOrDefault(0);
            string equation = args.ElementAtOrDefault(1);

            try {
                switch (command) {
                    case "eval": {
                        if (equation == default) {
                            Console.WriteLine("usage:\n  eval <expression> [<variables>]");
                            break;
                        }

                        Dictionary<string, string> variables = new();
                        foreach (string variable in args.Skip(2)) {
                            string[] split = variable.Split("=");
                            if (split.Length != 2)
                                throw new ArgumentException($"malformed variable assignment: {variable}");
                            variables[split[0]] = split[1];
                        }

                        Console.WriteLine(Solve(Parser.ShuntingYard(Parser.InsertVariablesConstants(Parser.Parse(equation, variables), variables))));
                        break;
                    }
                    case "cas": {
                        if (equation == default) {
                            Console.WriteLine("usage:\n  cas <equation> <command> [<args>]");
                            break;
                        }

                        switch (args[2]) {
                            case "simplify": {
                                /*
                                syntax: cas <equation> combineliketerms
                                description: combines like terms
                                notes: ints only
                                */
                                Simplifier simplifier = new();
                                simplifier.Simplify(equation, out string print);
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
                                PolyFactor poly_factorizer = new();
                                Console.WriteLine(poly_factorizer.Factor(equation, variable));

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
                                SyntheticDiv divider = new();
                                string print = divider.Div(equation, variable, zero, out int rem);
                                Console.WriteLine($"{print} | remainder: {rem}");
                                break;
                            }
                            case "binomialtheorem": {
                                /*
                                syntax: cas <equation> binomialtheorem
                                description: expands a binomial that was put to a power
                                notes: ints only
                                */

                                BinomialTheorem binomial_theorem = new();
                                Console.WriteLine(binomial_theorem.Expand(equation));
                                break;
                            }
                            default:
                                Console.WriteLine("usage:\n  dotnet run cas <subcommand> <args>");
                                break;
                        }

                        break;
                    }

                    default:
                        Console.WriteLine($"unknown command {command}");
                        Console.WriteLine("\navailable commands:\n  eval\n  cas");
                        Console.WriteLine("\nusage:\n  dotnet run <command> <args>");
                        break;
                }

            } catch (Exception e) {
                Console.WriteLine($"error: {e.Message}");
            }
        }
    }
}

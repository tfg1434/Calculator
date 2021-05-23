using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Calculator.CAS;
using static Calculator.Solver;

namespace Calculator {
    static class Program {
        private static void Main(string[] args) {
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
                        for (int i = 2; i < args.Length; i++) {
                            string[] split = args[i].Split("=");
                            if (split.Length != 2)
                                throw new MalformedVariableException($"malformed variable assignment: {args[i]}");
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
                                syntax: cas <equation> simplify
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

                                string variable = args.ElementAtOrDefault(3) ?? "x";
                                PolyFactor poly_factorizer = new();
                                Console.WriteLine(poly_factorizer.Factor(equation, variable));

                                break;

                            }
                            case "syntheticdiv": {
                                /*
                                syntax: cas <equation> syntheticdiv zero <variable=x>
                                description: uses synthetic division to divide a polynomial by x-a
                                notes: ints only
                                */

                                if (!int.TryParse(args.ElementAtOrDefault(3), out int zero)) {
                                    Console.WriteLine("usage:\n  dotnet run cas syntheticdiv zero <variable=x>");
                                    break;
                                }

                                string variable = args.ElementAtOrDefault(4) ?? "x";
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
                            case "shoelace": {
                                /*
                                cas <vertices> shoelace
                                description: calculates the area of a polygon
                                notes:
                                    polygon can be concave or convex, just not intersecting
                                    vertices must be defined ccw or cw
                                    vertices look like (x,y)(x,y)(x,y)
                                 */
                                ShoelaceAlgorithm shoelace = new();
                                Console.WriteLine(shoelace.Shoelace(args[1]));
                                break;
                            }
                            default:
                                Console.WriteLine("usage:\n  dotnet run cas <subcommand> <args>");
                                break;
                        }

                        break;
                    }
                    case "hillclimb": {
                        /*
                        hillclimb <expression> <unknown> [<variables>]
                        */

                        string unknown = args.ElementAtOrDefault(2);
                        if (equation == default || unknown == default) {
                            Console.WriteLine("usage:\n  hillclimb <expression> <unknown> [<variables>]");
                            break;
                        }

                        Dictionary<string, string> variables = new();
                        for (int i = 3; i < args.Length; i++) {
                            string[] split = args[i].Split("=");
                            if (split.Length != 2)
                                throw new MalformedVariableException($"malformed variable assignment: {args[i]}");
                            variables[split[0]] = split[1];
                        }

                        //now hillclimb
                        Console.WriteLine(HillClimb.HillClimbing(equation, variables, unknown));

                        break;
                    }

                    default:
                        Console.WriteLine($"unknown command {command}");
                        Console.WriteLine("\navailable commands:\n  eval\n  cas\n  hillclimb");
                        Console.WriteLine("\nusage:\n  dotnet run <command> <args>");
                        break;
                }

            } catch (InvalidInputException e) {
                Console.WriteLine($"An error occured!\n{e.GetType()}: {e.Message}");
            }
        }
    }
}

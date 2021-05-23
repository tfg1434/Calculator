using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Calculator.CAS {
    class ShoelaceAlgorithm {
        //works with any convex or concave polygon that does not intersect
        //points must be defined ccw or cw

        private static string[] get_vertices(string vertices) {
            //vertices look like (x,y),(x,y),(x,y)
            if (!Regex.IsMatch(vertices, @"(?:\(\d\,\d\))+"))
                throw new MalformedVerticeException("Vertice was not in a correct format");
            string str = vertices[1..^1];

            return str.Split(")(");
        }

        private static (decimal[] xcoords, decimal[] ycoords) get_coords(string[] vertices) {
            //vertice looks like x,y
            var xcoords = new decimal[vertices.Length];
            var ycoords = new decimal[vertices.Length];

            int i = 0;
            foreach (string vertice in vertices) {
                string[] split = vertice.Split(",");

                decimal[] dec_split = split.Select(decimal.Parse).ToArray();

                xcoords[i] = dec_split[0];
                ycoords[i] = dec_split[1];

                i++;
            }

            return (xcoords, ycoords);
        }

        public decimal Shoelace(string vertices) {
            (decimal[] x, decimal[] y) = get_coords(get_vertices(vertices));
            decimal sum1 = 0;
            decimal sum2 = 0;

            for (int i = 0; i < x.Length; i++) {
                sum1 += x[i % vertices.Length] * y[(i + 1) % x.Length];
                sum2 += y[i % vertices.Length] * x[(i + 1) % x.Length];
            }

            return Math.Abs(sum1 - sum2) / 2;
        }
    }
}

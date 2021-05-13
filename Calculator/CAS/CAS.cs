using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

//shared stuff in the whole CAS class

namespace Calculator.CAS {
    public readonly struct Term {
        public int coefficient { get; }
        public string term { get; }

        //there's no access modifier to do exactly what i want, so we live with internal :p
        internal Term(int coefficient, string term) {
            this.coefficient = coefficient;
            this.term = term;
        }
    }
}

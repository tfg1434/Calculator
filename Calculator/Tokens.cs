using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator {
    interface IToken {
        string Value { get; }
    }

    interface IFunction : IToken {
        IToken[] args { get; }
    }

    struct Max : IFunction {
        private readonly IToken[] _args;
        public IToken[] args => _args;
        public string Value { get {
                decimal max = decimal.MinValue;
                foreach (IToken token in _args) {
                    if (decimal.Parse(token.Value) > max) {
                        max = decimal.Parse(token.Value);
                    }
                }
                return max.ToString();
            } 
        }

        public Max(params IToken[] args) {
            _args = args;
        }
    }
}

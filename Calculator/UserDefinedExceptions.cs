using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator {
    class InvalidInputException : Exception {
        protected InvalidInputException() { }
        protected InvalidInputException(string message) : base(message) { }
        protected InvalidInputException(string message, Exception inner) : base(message, inner) { }
    }

    class MalformedVariableException : InvalidInputException {
        public MalformedVariableException() { }
        public MalformedVariableException(string message) : base(message) { }
        public MalformedVariableException(string message, Exception inner) : base(message, inner) { }
    }

    class NotPossibleException : InvalidInputException {
        public NotPossibleException() { }
        public NotPossibleException(string message) : base(message) { }
        public NotPossibleException(string message, Exception inner) : base(message, inner) { }
    }

    class NotPolynomialException : InvalidInputException {
        public NotPolynomialException() { }
        public NotPolynomialException(string message) : base(message) { }
        public NotPolynomialException(string message, Exception inner) : base(message, inner) { }
    }

    class NotPolynomial1VariableException : InvalidInputException {
        public NotPolynomial1VariableException() { }
        public NotPolynomial1VariableException(string message) : base(message) { }
        public NotPolynomial1VariableException(string message, Exception inner) : base(message, inner) { }
    }

    class SyntaxException : InvalidInputException {
        public SyntaxException() { }
        public SyntaxException(string message) : base(message) { }
        public SyntaxException(string message, Exception inner) : base(message, inner) { }
    }

    class OutOfRangeExpcetion : InvalidInputException {
        public OutOfRangeExpcetion() { }
        public OutOfRangeExpcetion(string message) : base(message) { }
        public OutOfRangeExpcetion(string message, Exception inner) : base(message, inner) { }
    }
}

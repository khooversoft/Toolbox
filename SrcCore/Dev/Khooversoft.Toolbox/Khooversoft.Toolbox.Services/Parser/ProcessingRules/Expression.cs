using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Expression<T> : INode where T : System.Enum
    {
        public Expression(T tokenType)
        {
            TokenType = tokenType;
        }

        public Expression(T tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public string Name => nameof(Expression<T>);

        public T TokenType { get; }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            var exp = obj as Expression<T>;
            if (exp == null)
            {
                return false;
            }

            bool test = TokenType.Equals(exp.TokenType);
            return test;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            if( string.IsNullOrWhiteSpace(Value))
            {
                return $"{nameof(Expression<T>)}: Name={TokenType}";
            }

            return $"[{TokenType}:{Value}]";
        }
    }
}

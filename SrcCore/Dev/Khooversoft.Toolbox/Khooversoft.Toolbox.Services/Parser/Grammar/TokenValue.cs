using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class TokenValue : IToken
    {
        public TokenValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            if( !(obj is TokenValue))
            {
                return false;
            }

            return ((TokenValue)obj).Value.Equals(Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"{nameof(TokenValue)}: Value={Value}";
    }
}

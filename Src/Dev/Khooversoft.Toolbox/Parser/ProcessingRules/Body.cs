using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Body<T> : IAstNode where T : System.Enum
    {
        public Body(Symbol<T> startSymbol, Symbol<T> endSymbol, bool supportNested = true)
        {
            StartSymbol = startSymbol;
            EndSymbol = endSymbol;
            SupportNested = supportNested;
        }

        public string Name => "Body";

        public Symbol<T> StartSymbol { get; }

        public Symbol<T> EndSymbol { get; }

        public bool SupportNested { get; }

        public override string ToString()
        {
            return $"{nameof(Body<T>)}: StartSymbol={StartSymbol}, EndSymbol={EndSymbol}, SupportNested={SupportNested}";
        }
    }
}

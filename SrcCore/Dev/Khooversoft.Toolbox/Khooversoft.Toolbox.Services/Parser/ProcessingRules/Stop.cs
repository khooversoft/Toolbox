using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Parser
{
    public class Stop : INode
    {
        public Stop(string name = null)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{nameof(Stop)}";
        }
    }
}

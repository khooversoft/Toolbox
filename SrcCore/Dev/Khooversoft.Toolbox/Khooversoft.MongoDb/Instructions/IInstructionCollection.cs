using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.MongoDb
{
    public interface IInstructionCollection : IInstructionNode, IEnumerable<IInstructionNode>
    {
    }
}

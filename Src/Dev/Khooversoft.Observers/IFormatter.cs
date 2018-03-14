using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Observers
{
    public interface IFormatter<TIn, TOut>
    {
        /// <summary>
        /// Format an object, transform from TIn to TOut
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        TOut Format(TIn obj);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.EventFlow
{
    public interface IEventDataBuffer : IObserver<EventData>
    {
        IEnumerable<EventData> SearchForBaseCv(string baseCv, int lastRowCount = 100);
    }
}

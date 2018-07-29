using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public struct EdgeId : IEqualityComparer<EdgeId>, IComparable<EdgeId>
    {
        public EdgeId(int sourceNodeId, int toNodeId)
        {
            SourceNodeId = sourceNodeId;
            ToNodeId = toNodeId;
        }

        public int SourceNodeId { get; }
        public int ToNodeId { get; }

        public long Value => (long)SourceNodeId << 32 | (long)(uint)ToNodeId;

        public override string ToString()
        {
            return $"EdgeId={Value}, SourceNodeId ={SourceNodeId}, ToNodeId={ToNodeId}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EdgeId))
            {
                return false;
            }

            return this == (EdgeId)obj;
        }

        public override int GetHashCode()
        {
            return SourceNodeId ^ ToNodeId;
        }

        public bool Equals(EdgeId x, EdgeId y)
        {
            return x == y;
        }

        public int GetHashCode(EdgeId obj)
        {
            return GetHashCode();
        }

        public int CompareTo(EdgeId other)
        {
            if (Value < other) { return -1; }
            if (Value > other) { return 1; }

            return 0;
        }

        public static implicit operator long(EdgeId edgeId)
        {
            return edgeId.Value;
        }

        public static bool operator ==(EdgeId edgeId1, EdgeId edgeId2)
        {
            return edgeId1.SourceNodeId == edgeId2.SourceNodeId
                && edgeId1.ToNodeId == edgeId2.ToNodeId;
        }

        public static bool operator !=(EdgeId edgeId1, EdgeId edgeId2)
        {
            return !(edgeId1 == edgeId2);
        }

        public static bool operator <(EdgeId edgeId1, EdgeId edgeId2)
        {
            return edgeId1.Value < edgeId2.Value;
        }

        public static bool operator >(EdgeId edgeId1, EdgeId edgeId2)
        {
            return edgeId1.Value > edgeId2.Value;
        }
    }
}

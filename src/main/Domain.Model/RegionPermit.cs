using SQLite;
using System;

namespace works.ei8.Cortex.Sentry.Domain.Model
{
    public class RegionPermit
    {
        [PrimaryKey, AutoIncrement]
        public long SequenceId { get; set; }

        public Guid UserNeuronId { get; set; }

        public Guid RegionNeuronId { get; set; }

        public int WriteLevel { get; set; }

        public bool CanRead { get; set; }

        public bool Equals(RegionPermit other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (object.ReferenceEquals(null, other)) return false;
            return this.SequenceId.Equals(other.SequenceId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RegionPermit);
        }

        public override int GetHashCode()
        {
            return this.SequenceId.GetHashCode();
        }
    }
}

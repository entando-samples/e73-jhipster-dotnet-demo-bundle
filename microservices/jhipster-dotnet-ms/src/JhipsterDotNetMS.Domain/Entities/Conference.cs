using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JhipsterDotNetMS.Domain.Entities
{
    [Table("conference")]
    public class Conference : BaseEntity<long>
    {
        public string Name { get; set; }
        public string Location { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var conference = obj as Conference;
            if (conference?.Id == null || conference?.Id == 0 || Id == 0) return false;
            return EqualityComparer<long>.Default.Equals(Id, conference.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Conference{" +
                    $"ID='{Id}'" +
                    $", Name='{Name}'" +
                    $", Location='{Location}'" +
                    "}";
        }
    }
}

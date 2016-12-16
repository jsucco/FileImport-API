namespace MCS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("eventLog")]
    public partial class eventLog
    {
        public int Id { get; set; }

        public int? SourceId { get; set; }

        public string Action { get; set; }

        public string Description { get; set; }

        public DateTime? Timestamp { get; set; }

        public virtual Source Source { get; set; }
    }
}

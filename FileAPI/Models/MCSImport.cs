namespace MCS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MCSImport")]
    public partial class MCSImport
    {
        public int id { get; set; }

        public string LineBlob { get; set; }

        public int? DocNumber { get; set; }

        [StringLength(300)]
        public string Location { get; set; }

        public DateTime? Timestamp { get; set; }

        public int? SourceId { get; set; }

        public virtual Source Source { get; set; }
    }
}

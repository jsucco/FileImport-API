namespace MCS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MCSBleacher")]
    public partial class MCSBleacher
    {
        public int id { get; set; }

        [StringLength(5)]
        public string Operation { get; set; }

        [StringLength(20)]
        public string MachineNo { get; set; }

        [StringLength(15)]
        public string JobOrderNo { get; set; }

        public DateTime? StartDate { get; set; }

        [StringLength(10)]
        public string Location { get; set; }

        public string stock { get; set; }

        public decimal? KgOfFabric { get; set; }

        [StringLength(10)]
        public string NoProgram1 { get; set; }

        [StringLength(10)]
        public string NoProgram2 { get; set; }

        [StringLength(10)]
        public string NoProgram3 { get; set; }

        [StringLength(10)]
        public string NoProgram4 { get; set; }

        [StringLength(10)]
        public string NoProgram5 { get; set; }

        public decimal? StandardTimeMin { get; set; }

        [StringLength(10)]
        public string FreeFieldcode1 { get; set; }

        [StringLength(10)]
        public string FreeFieldcode2 { get; set; }

        [StringLength(10)]
        public string FreeFieldcode3 { get; set; }

        [StringLength(30)]
        public string FreeFieldDesc1 { get; set; }

        [StringLength(30)]
        public string FreeFieldDesc2 { get; set; }

        [StringLength(30)]
        public string FreeFieldDesc3 { get; set; }

        [StringLength(30)]
        public string Note1 { get; set; }

        [StringLength(30)]
        public string Note2 { get; set; }

        public decimal? MachineEfficiency { get; set; }

        public int? ActualWorkTime_S { get; set; }

        public int? WorkDuration_S { get; set; }

        public DateTime? WorkEndDate { get; set; }

        public DateTime? WorkEndTime { get; set; }

        [StringLength(20)]
        public string FreeFieldCode4 { get; set; }

        [StringLength(20)]
        public string FreeFieldDesc4 { get; set; }

        public int? IntWRespToPrevBatch_M { get; set; }

        public int? WaterCons_Lt10 { get; set; }

        public int? SteamCons { get; set; }

        public int? EnergyCons_kWh { get; set; }
    }
}

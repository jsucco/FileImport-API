namespace MCS.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BleachersContext : DbContext
    {
        public BleachersContext()
            : base("name=BleachersContext")
        {
        }

        public virtual DbSet<MCSBleacher> MCSBleachers { get; set; }
        public virtual DbSet<MCSImport> MCSImports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.Operation)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.MachineNo)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.JobOrderNo)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.Location)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.stock)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.KgOfFabric)
                .HasPrecision(18, 1);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.NoProgram1)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.NoProgram2)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.NoProgram3)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.NoProgram4)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.NoProgram5)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.StandardTimeMin)
                .HasPrecision(18, 1);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldcode1)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldcode2)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldcode3)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldDesc1)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldDesc2)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldDesc3)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.Note1)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.Note2)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.MachineEfficiency)
                .HasPrecision(18, 1);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldCode4)
                .IsUnicode(false);

            modelBuilder.Entity<MCSBleacher>()
                .Property(e => e.FreeFieldDesc4)
                .IsUnicode(false);

            modelBuilder.Entity<MCSImport>()
                .Property(e => e.LineBlob)
                .IsUnicode(false);

            modelBuilder.Entity<MCSImport>()
                .Property(e => e.Location)
                .IsUnicode(false);
        }
    }
}

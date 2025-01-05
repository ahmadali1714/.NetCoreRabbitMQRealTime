using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TechnicalAssessment.Model;

namespace TechnicalAssessment.Data;

public partial class DtechLoggerContext : DbContext
{
    public DtechLoggerContext()
    {
    }

    public DtechLoggerContext(DbContextOptions<DtechLoggerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MyLogger> MyLoggers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=DtechLogger;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyLogger>(entity =>
        {
            entity.ToTable("myLogger");

            entity.Property(e => e.FileName)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LogDate).HasColumnType("datetime");
            entity.Property(e => e.Originator)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.EntityFrameworkCore;
using Skolehjemsamtale.Domain.Entities;
using Skolehjemsamtale.Infrastructure.Integration;

namespace Skolehjemsamtale.Infrastructure.Persistence;

public class SkoleDbContext : DbContext
{
    public SkoleDbContext(DbContextOptions<SkoleDbContext> options) : base(options) { }

    public DbSet<Elev> Elever => Set<Elev>();
    public DbSet<Observation> Observationer => Set<Observation>();
    public DbSet<Samtale> Samtaler => Set<Samtale>();
    public DbSet<SamtaleFaseNote> SamtaleFaseNoter => Set<SamtaleFaseNote>();
    public DbSet<SamtaleDeltager> SamtaleDeltagere => Set<SamtaleDeltager>();
    public DbSet<Handlingsaftale> Handlingsaftaler => Set<Handlingsaftale>();
    public DbSet<SamtaleObservation> SamtaleObservationer => Set<SamtaleObservation>();
    public DbSet<HjemmeIndsigt> HjemmeIndsigter => Set<HjemmeIndsigt>();
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Alle id'er genereres klient-side som Guid i domænet. Uden dette ville EF opfatte
        // en ny underentitet (fx en hjemmeindsigt) med et allerede sat nøglefelt som "Modified",
        // fordi nøglen er konfigureret som genereret af lageret.
        foreach (var et in b.Model.GetEntityTypes())
        {
            var key = et.FindPrimaryKey();
            if (key is { Properties.Count: 1 } k && k.Properties[0].ClrType == typeof(Guid))
                k.Properties[0].ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
        }

        b.Entity<Elev>(e =>
        {
            e.ToTable("Elever");
            e.Property(x => x.Navn).HasMaxLength(200).IsRequired();
            e.Property(x => x.Klasse).HasMaxLength(100);
            e.Property(x => x.EksterntId).HasMaxLength(200);
            e.HasIndex(x => x.EksterntId);
            e.HasMany(x => x.Observationer).WithOne(o => o.Elev!).HasForeignKey(o => o.ElevId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Samtaler).WithOne(s => s.Elev!).HasForeignKey(s => s.ElevId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.HjemmeIndsigter).WithOne(h => h.Elev!).HasForeignKey(h => h.ElevId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Observation>(e =>
        {
            e.ToTable("Observationer");
            e.Property(x => x.Beskrivelse).HasMaxLength(4000).IsRequired();
            e.Property(x => x.Kontekst).HasMaxLength(300);
            e.Property(x => x.ObserveretAf).HasMaxLength(200);
            e.Property(x => x.HvadHjalp).HasMaxLength(2000);
            e.HasIndex(x => new { x.ElevId, x.Dato });
        });

        b.Entity<Samtale>(e =>
        {
            e.ToTable("Samtaler");
            e.Property(x => x.Formaal).HasMaxLength(1000).IsRequired();
            e.Property(x => x.SkabelonKode).HasMaxLength(100).IsRequired();
            e.Property(x => x.Sted).HasMaxLength(200);
            e.Property(x => x.Aflysningsaarsag).HasMaxLength(500);
            e.HasMany(x => x.Noter).WithOne(n => n.Samtale!).HasForeignKey(n => n.SamtaleId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Deltagere).WithOne(d => d.Samtale!).HasForeignKey(d => d.SamtaleId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Aftaler).WithOne(a => a.Samtale!).HasForeignKey(a => a.SamtaleId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Observationer).WithOne(o => o.Samtale!).HasForeignKey(o => o.SamtaleId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.HjemmeIndsigter).WithOne(h => h.Samtale).HasForeignKey(h => h.SamtaleId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<SamtaleFaseNote>(e =>
        {
            e.ToTable("SamtaleFaseNoter");
            e.Property(x => x.Indhold).HasMaxLength(8000).IsRequired();
        });

        b.Entity<SamtaleDeltager>(e =>
        {
            e.ToTable("SamtaleDeltagere");
            e.Property(x => x.Navn).HasMaxLength(200).IsRequired();
        });

        b.Entity<Handlingsaftale>(e =>
        {
            e.ToTable("Handlingsaftaler");
            e.Property(x => x.Beskrivelse).HasMaxLength(2000).IsRequired();
            e.Property(x => x.AnsvarligNavn).HasMaxLength(200);
            e.Property(x => x.Evaluering).HasMaxLength(2000);
        });

        b.Entity<SamtaleObservation>(e =>
        {
            e.ToTable("SamtaleObservationer");
            e.HasOne(x => x.Observation).WithMany().HasForeignKey(x => x.ObservationId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.SamtaleId, x.ObservationId }).IsUnique();
        });

        b.Entity<HjemmeIndsigt>(e =>
        {
            e.ToTable("HjemmeIndsigter");
            e.Property(x => x.Spoergsmaal).HasMaxLength(1000);
            e.Property(x => x.Svar).HasMaxLength(4000).IsRequired();
            e.Property(x => x.BidragetAf).HasMaxLength(200);
        });

        b.Entity<OutboxMessage>(e =>
        {
            e.ToTable("OutboxMessages");
            e.Property(x => x.EventType).HasMaxLength(200).IsRequired();
            e.Property(x => x.AggregateType).HasMaxLength(200).IsRequired();
            e.Property(x => x.Payload).IsRequired();
            e.HasIndex(x => new { x.Behandlet, x.OprettetUtc });
        });
    }
}

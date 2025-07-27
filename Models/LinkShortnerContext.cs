using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LinkShorterAPI.Models;

public partial class LinkShortnerContext : DbContext
{
    public LinkShortnerContext(DbContextOptions<LinkShortnerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Apikey> Apikeys { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Click> Clicks { get; set; }

    public virtual DbSet<ClickUtm> ClickUtms { get; set; }

    public virtual DbSet<Domain> Domains { get; set; }

    public virtual DbSet<SecurityLog> SecurityLogs { get; set; }

    public virtual DbSet<ShortLink> ShortLinks { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activity__3214EC078F7FE3FB");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<Apikey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__APIKeys__3214EC07E207431C");

            entity.ToTable("APIKeys");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Apikeys)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__APIKeys__Created__6B24EA82");

            entity.HasOne(d => d.Team).WithMany(p => p.Apikeys)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__APIKeys__TeamId__6A30C649");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC07094FE558");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.EntityId).HasMaxLength(100);
            entity.Property(e => e.EntityType).HasMaxLength(100);
        });

        modelBuilder.Entity<Click>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clicks__3214EC0733D5746B");

            entity.HasIndex(e => new { e.ShortLinkId, e.ClickedAt }, "IX_Clicks_ShortLinkId_ClickedAt");

            entity.Property(e => e.Browser).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ClickedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.Device).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.IsBot).HasDefaultValue(false);
            entity.Property(e => e.Os)
                .HasMaxLength(100)
                .HasColumnName("OS");
            entity.Property(e => e.Referrer).HasMaxLength(2083);
            entity.Property(e => e.UserAgent).HasMaxLength(512);

            entity.HasOne(d => d.ShortLink).WithMany(p => p.Clicks)
                .HasForeignKey(d => d.ShortLinkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Clicks__ShortLin__5EBF139D");
        });

        modelBuilder.Entity<ClickUtm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClickUTM__3214EC07220BB896");

            entity.ToTable("ClickUTMs");

            entity.Property(e => e.UtmCampaign).HasMaxLength(100);
            entity.Property(e => e.UtmContent).HasMaxLength(100);
            entity.Property(e => e.UtmMedium).HasMaxLength(100);
            entity.Property(e => e.UtmSource).HasMaxLength(100);
            entity.Property(e => e.UtmTerm).HasMaxLength(100);

            entity.HasOne(d => d.Click).WithMany(p => p.ClickUtms)
                .HasForeignKey(d => d.ClickId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClickUTMs__Click__6E01572D");
        });

        modelBuilder.Entity<Domain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Domains__3214EC0787EB37AC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DomainName).HasMaxLength(255);
            entity.Property(e => e.IsVerified).HasDefaultValue(false);

            entity.HasOne(d => d.Team).WithMany(p => p.Domains)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__Domains__TeamId__5165187F");
        });

        modelBuilder.Entity<SecurityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Security__3214EC07F956D242");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.EventType).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(512);
        });

        modelBuilder.Entity<ShortLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShortLin__3214EC076147533D");

            entity.HasIndex(e => new { e.Slug, e.DomainId }, "IX_ShortLinks_Slug_Domain").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ClickCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Slug).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ShortLinks)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShortLink__Creat__59063A47");

            entity.HasOne(d => d.Domain).WithMany(p => p.ShortLinks)
                .HasForeignKey(d => d.DomainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShortLink__Domai__59FA5E80");

            entity.HasOne(d => d.Team).WithMany(p => p.ShortLinks)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__ShortLink__TeamI__5812160E");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC077C2C0F11");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BillingEmail).HasMaxLength(256);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.SubscriptionPlan).HasMaxLength(50);

            entity.HasOne(d => d.Team).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__TeamI__6477ECF3");
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemLo__3214EC07D3C7C8DC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Level).HasMaxLength(20);
            entity.Property(e => e.Source).HasMaxLength(100);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teams__3214EC07FC5F59BE");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Owner).WithMany(p => p.Teams)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Teams__OwnerId__44FF419A");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeamMemb__3214EC0780CC1B95");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Member");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamMembe__TeamI__4AB81AF0");

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamMembe__UserI__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07ED9A9BBB");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105340B84CFB5").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

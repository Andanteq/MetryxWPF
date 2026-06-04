using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MetryxWPF;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Devicetype> Devicetypes { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Measurementdevice> Measurementdevices { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Verification> Verifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=super29");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgagent", "pgagent");

        modelBuilder.Entity<Devicetype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("devicetype_pk");

            entity.ToTable("devicetype");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("document_pk");

            entity.ToTable("document");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Filename)
                .HasColumnType("character varying")
                .HasColumnName("filename");
            entity.Property(e => e.Filepath)
                .HasColumnType("character varying")
                .HasColumnName("filepath");
            entity.Property(e => e.Measurementdeviceid).HasColumnName("measurementdeviceid");
            entity.Property(e => e.Verificationid).HasColumnName("verificationid");

            entity.HasOne(d => d.Measurementdevice).WithMany(p => p.Documents)
                .HasForeignKey(d => d.Measurementdeviceid)
                .HasConstraintName("document_measurementdevice_fk");

            entity.HasOne(d => d.Verification).WithMany(p => p.Documents)
                .HasForeignKey(d => d.Verificationid)
                .HasConstraintName("document_verification_fk");
        });

        modelBuilder.Entity<Measurementdevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("measurementdevice_pk");

            entity.ToTable("measurementdevice");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Installationlocation)
                .HasColumnType("character varying")
                .HasColumnName("installationlocation");
            entity.Property(e => e.Lastverificationdate).HasColumnName("lastverificationdate");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Nextverificationdate).HasColumnName("nextverificationdate");
            entity.Property(e => e.Note)
                .HasColumnType("character varying")
                .HasColumnName("note");
            entity.Property(e => e.Releasedate).HasColumnName("releasedate");
            entity.Property(e => e.Serialnumber)
                .HasColumnType("character varying")
                .HasColumnName("serialnumber");
            entity.Property(e => e.Typeid).HasColumnName("typeid");
            entity.Property(e => e.Suitable).HasColumnName("unsuitable");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Responsible)
                .HasColumnType("character varying")
                .HasColumnName("responsible");
            entity.Property(e => e.Verificationinterval).HasColumnName("verificationinterval");

            entity.HasOne(d => d.Type).WithMany(p => p.Measurementdevices)
                .HasForeignKey(d => d.Typeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("measurementdevice_devicetype_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Measurementdevices)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("measurementdevice_user_fk");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notification_pk");

            entity.ToTable("notification");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Createdat).HasColumnName("createdat");
            entity.Property(e => e.Isread).HasColumnName("isread");
            entity.Property(e => e.Measurementdeviceid).HasColumnName("measurementdeviceid");
            entity.Property(e => e.Message)
                .HasColumnType("character varying")
                .HasColumnName("message");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Measurementdevice).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Measurementdeviceid)
                .HasConstraintName("notification_measurementdevice_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_user_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pk");

            entity.ToTable("role");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Firstname)
                .HasColumnType("character varying")
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasColumnType("character varying")
                .HasColumnName("lastname");
            entity.Property(e => e.Middlename)
                .HasColumnType("character varying")
                .HasColumnName("middlename");
            entity.Property(e => e.Passwordhash)
                .HasColumnType("character varying")
                .HasColumnName("passwordhash");
            entity.Property(e => e.Phonenumber)
                .HasColumnType("character varying")
                .HasColumnName("phonenumber");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
            entity.Property(e => e.Fullname)
                .HasColumnType("character varying")
                .HasColumnName("fullname");
            entity.Property(e => e.IsThrowPassword).HasColumnName("isthrowpassword");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_fk");
        });

        modelBuilder.Entity<Verification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("verification_pk");

            entity.ToTable("verification");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Measurementdeviceid).HasColumnName("measurementdeviceid");
            entity.Property(e => e.Nextverificationdate).HasColumnName("nextverificationdate");
            entity.Property(e => e.Result).HasColumnType("character varying");
            entity.Property(e => e.Suitable).HasColumnName("unsuitable");
            entity.Property(e => e.Verificationdate).HasColumnName("verificationdate");
            entity.Property(e => e.Organization).HasColumnType("character varying");
            entity.Property(e => e.Certificatenumber).HasColumnType("character varying");

            entity.HasOne(d => d.Measurementdevice).WithMany(p => p.Verifications)
                .HasForeignKey(d => d.Measurementdeviceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("verification_measurementdevice_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

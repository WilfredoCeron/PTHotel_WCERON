using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Data;

public class HotelBookingDbContext : DbContext
{
    public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : base(options)
    {
    }
    
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<RoomInventory> RoomInventories => Set<RoomInventory>();
    public DbSet<RatePlan> RatePlans => Set<RatePlan>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        HotelBookingDbInitializer.Seed(modelBuilder);
        
        // Hotel Configuration
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.HasMany(e => e.RoomTypes).WithOne(r => r.Hotel).OnDelete(DeleteBehavior.Cascade);
        });
        
        // RoomType Configuration
        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.HotelId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.Hotel).WithMany(h => h.RoomTypes).HasForeignKey(e => e.HotelId);
            entity.HasMany(e => e.RoomInventories).WithOne(r => r.RoomType).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.RatePlans).WithOne(r => r.RoomType).OnDelete(DeleteBehavior.Cascade);
        });
        
        // RoomInventory Configuration
        modelBuilder.Entity<RoomInventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.AvailableRooms).IsRequired();
            entity.Property(e => e.ReservedRooms).IsRequired();
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasOne(e => e.RoomType).WithMany(r => r.RoomInventories).HasForeignKey(e => e.RoomTypeId);
            entity.HasIndex(e => new { e.RoomTypeId, e.Date }).IsUnique();
        });
        
        // RatePlan Configuration
        modelBuilder.Entity<RatePlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasOne(e => e.RoomType).WithMany(r => r.RatePlans).HasForeignKey(e => e.RoomTypeId);
        });
        
        // Guest Configuration
        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasMany(e => e.Bookings).WithOne(b => b.Guest).OnDelete(DeleteBehavior.NoAction);
        });
        
        // Booking Configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.HotelId).ValueGeneratedOnAdd();
                entity.Property(e => e.RoomTypeId).ValueGeneratedOnAdd();
                entity.Property(e => e.GuestId).ValueGeneratedOnAdd();
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasOne(e => e.Hotel).WithMany().OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.RoomType).WithMany().OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Guest).WithMany(g => g.Bookings).OnDelete(DeleteBehavior.NoAction).HasForeignKey(e => e.GuestId);
            entity.HasMany(e => e.Payments).WithOne(p => p.Booking).OnDelete(DeleteBehavior.Cascade);
        });
        
        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(e => e.Booking).WithMany(b => b.Payments).HasForeignKey(e => e.BookingId);
        });
        
        // IdempotencyRecord Configuration
        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Key).IsUnique();
        });
        
        // AuditLog Configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.EntityId);
        });
    }
}

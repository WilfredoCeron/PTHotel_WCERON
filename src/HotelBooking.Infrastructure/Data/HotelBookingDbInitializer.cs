using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Data
{
    public static class HotelBookingDbInitializer
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Hoteles
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { Id = 1, Name = "Hotel Paradise", Address = "123 Main St", City = "San Salvador", Country = "El Salvador", Phone = "+503 1234 5678", Email = "info@paradise.com", IsActive = true },
                new Hotel { Id = 2, Name = "Hotel Central", Address = "456 Central Ave", City = "Santa Ana", Country = "El Salvador", Phone = "+503 8765 4321", Email = "info@central.com", IsActive = true }
            );

            // RoomTypes (3 por hotel)
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { Id = 1, HotelId = 1, Name = "Suite", Description = "Habitación de lujo", Capacity = 2, TotalRooms = 10, IsActive = true },
                new RoomType { Id = 2, HotelId = 1, Name = "Doble", Description = "Habitación doble estándar", Capacity = 2, TotalRooms = 20, IsActive = true },
                new RoomType { Id = 3, HotelId = 1, Name = "Familiar", Description = "Habitación familiar", Capacity = 4, TotalRooms = 5, IsActive = true },
                new RoomType { Id = 4, HotelId = 2, Name = "Individual", Description = "Habitación individual económica", Capacity = 1, TotalRooms = 15, IsActive = true },
                new RoomType { Id = 5, HotelId = 2, Name = "Doble", Description = "Habitación doble", Capacity = 2, TotalRooms = 10, IsActive = true },
                new RoomType { Id = 6, HotelId = 2, Name = "Suite", Description = "Suite ejecutiva", Capacity = 3, TotalRooms = 7, IsActive = true }
            );

            // RoomInventory para 30 días para cada RoomType
            var startDate = DateTime.UtcNow.Date;
            var days = 30;
            var inventories = new List<RoomInventory>();
            var roomTypes = new[] {
                new { Id = 1, TotalRooms = 10 },
                new { Id = 2, TotalRooms = 20 },
                new { Id = 3, TotalRooms = 5 },
                new { Id = 4, TotalRooms = 15 },
                new { Id = 5, TotalRooms = 10 },
                new { Id = 6, TotalRooms = 7 }
            };
            int inventoryId = 1;
            foreach (var rt in roomTypes)
            {
                for (int i = 0; i < days; i++)
                {
                    var date = startDate.AddDays(i);
                    inventories.Add(new RoomInventory {
                        Id = inventoryId++,
                        RoomTypeId = rt.Id,
                        Date = date,
                        AvailableRooms = rt.TotalRooms,
                        ReservedRooms = 0
                    });
                }
            }
            modelBuilder.Entity<RoomInventory>().HasData(inventories);

            // Guests
            modelBuilder.Entity<Guest>().HasData(
                new Guest { Id = 1, FirstName = "Juan", LastName = "Pérez", Email = "juan@example.com", PhoneNumber = "+503 1111 1111", IsActive = true },
                new Guest { Id = 2, FirstName = "Ana", LastName = "López", Email = "ana@example.com", PhoneNumber = "+503 2222 2222", IsActive = true }
            );

            // Bookings de ejemplo (5)
            var bookings = new List<Booking> {
                new Booking { Id = 1, HotelId = 1, RoomTypeId = 1, GuestId = 1, CheckInDate = startDate.AddDays(1), CheckOutDate = startDate.AddDays(3), NumberOfRooms = 1, NumberOfGuests = 2, TotalPrice = 200, Status = BookingStatus.Confirmed, SpecialRequests = "", CreatedAt = DateTime.UtcNow },
                new Booking { Id = 2, HotelId = 1, RoomTypeId = 2, GuestId = 2, CheckInDate = startDate.AddDays(2), CheckOutDate = startDate.AddDays(4), NumberOfRooms = 2, NumberOfGuests = 4, TotalPrice = 300, Status = BookingStatus.Confirmed, SpecialRequests = "", CreatedAt = DateTime.UtcNow },
                new Booking { Id = 3, HotelId = 2, RoomTypeId = 4, GuestId = 1, CheckInDate = startDate.AddDays(5), CheckOutDate = startDate.AddDays(7), NumberOfRooms = 1, NumberOfGuests = 1, TotalPrice = 120, Status = BookingStatus.Confirmed, SpecialRequests = "", CreatedAt = DateTime.UtcNow },
                new Booking { Id = 4, HotelId = 2, RoomTypeId = 5, GuestId = 2, CheckInDate = startDate.AddDays(10), CheckOutDate = startDate.AddDays(12), NumberOfRooms = 1, NumberOfGuests = 2, TotalPrice = 180, Status = BookingStatus.Confirmed, SpecialRequests = "", CreatedAt = DateTime.UtcNow },
                new Booking { Id = 5, HotelId = 1, RoomTypeId = 3, GuestId = 1, CheckInDate = startDate.AddDays(15), CheckOutDate = startDate.AddDays(18), NumberOfRooms = 1, NumberOfGuests = 3, TotalPrice = 400, Status = BookingStatus.Confirmed, SpecialRequests = "Cuna para bebé", CreatedAt = DateTime.UtcNow }
            };
            modelBuilder.Entity<Booking>().HasData(bookings);
        }
    }
}

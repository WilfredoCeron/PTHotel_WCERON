using Microsoft.EntityFrameworkCore;
using HotelBooking.Infrastructure.Data;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Services;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.Application.Commands;
using HotelBooking.Application.Queries;
using HotelBooking.Application.Validators;
using HotelBooking.Application.Mappings;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<HotelBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Registro de repositorio genérico para RoomType
builder.Services.AddScoped<IRepository<RoomType>, RepositoryBase<RoomType>>();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateHotelCommand).Assembly);
});

// Validators - Register manually
builder.Services.AddScoped<IValidator<CreateHotelCommand>, CreateHotelCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateHotelCommand>, UpdateHotelCommandValidator>();
builder.Services.AddScoped<IValidator<CreateGuestCommand>, CreateGuestCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateGuestCommand>, UpdateGuestCommandValidator>();
builder.Services.AddScoped<IValidator<CreateBookingCommand>, CreateBookingCommandValidator>();
builder.Services.AddScoped<IValidator<CreateRoomTypeCommand>, CreateRoomTypeCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateRoomTypeCommand>, UpdateRoomTypeCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateRoomTypeCommand>, UpdateRoomTypeCommandValidator>();
// Registro de IdempotencyRepository
builder.Services.AddScoped<IdempotencyRepository>();

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();
    try
    {
        // Create database and tables if they don't exist
        await db.Database.EnsureCreatedAsync();
        
        // Seed data
        await SeedData(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
        throw;
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Seed data function
static async Task SeedData(HotelBookingDbContext db)
{
    if (db.Hotels.Any())
        return;

    var hotels = new[]
    {
        Hotel.Create("Hotel Paradise", "123 Main St", "San Salvador", "El Salvador", "+503 1234 5678", "info@paradise.com"),
        Hotel.Create("Hotel Atlantida", "456 Beach Ave", "La Libertad", "El Salvador", "+503 2345 6789", "contact@atlantida.com")
    };

    foreach (var hotel in hotels)
    {
        db.Hotels.Add(hotel);
    }
    await db.SaveChangesAsync();

    // Crear RoomTypes
    var roomTypes = new List<HotelBooking.Domain.Entities.RoomType>();
    foreach (var hotel in hotels)
    {
        var rt1 = HotelBooking.Domain.Entities.RoomType.Create(hotel.Id, "Suite", "Habitación de lujo", 2, 10);
        var rt2 = HotelBooking.Domain.Entities.RoomType.Create(hotel.Id, "Doble", "Habitación doble", 2, 20);
        db.RoomTypes.Add(rt1);
        db.RoomTypes.Add(rt2);
        roomTypes.Add(rt1);
        roomTypes.Add(rt2);
    }
    await db.SaveChangesAsync();

    // Crear tarifas para los próximos 30 días
    var today = DateTime.UtcNow.Date;
    foreach (var rt in roomTypes)
    {
        var ratePlan = HotelBooking.Domain.Entities.RatePlan.Create(rt.Id, 100, today, today.AddDays(30));
        db.RatePlans.Add(ratePlan);
    }
    await db.SaveChangesAsync();

    // Crear Guests
    var guests = new List<HotelBooking.Domain.Entities.Guest>();
    for (int i = 1; i <= 5; i++)
    {
        var guest = HotelBooking.Domain.Entities.Guest.Create($"Guest{i}", $"Test{i}", $"guest{i}@mail.com");
        db.Guests.Add(guest);
        guests.Add(guest);
    }
    await db.SaveChangesAsync();

    // Crear 5 reservas, cada una en un hotel y una habitación de ese hotel
    for (int i = 0; i < hotels.Length; i++)
    {
        var hotel = hotels[i];
        var rt = roomTypes.First(r => r.HotelId == hotel.Id);
        var guest = guests[i % guests.Count];
        var checkIn = today.AddDays(i);
        var checkOut = checkIn.AddDays(2);
        var booking = HotelBooking.Domain.Entities.Booking.Create(hotel.Id, rt.Id, guest.Id, checkIn, checkOut, 1, 2, 200);
        db.Bookings.Add(booking);
    }
    await db.SaveChangesAsync();
}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG CACHEBUST=1

WORKDIR /app

COPY ["src/HotelBooking.Domain/HotelBooking.Domain.csproj", "src/HotelBooking.Domain/"]
COPY ["src/HotelBooking.Application/HotelBooking.Application.csproj", "src/HotelBooking.Application/"]
COPY ["src/HotelBooking.Infrastructure/HotelBooking.Infrastructure.csproj", "src/HotelBooking.Infrastructure/"]
COPY ["src/HotelBooking.Api/HotelBooking.Api.csproj", "src/HotelBooking.Api/"]

RUN dotnet restore "src/HotelBooking.Api/HotelBooking.Api.csproj"

COPY . .

RUN echo "Building at $(date) with CACHEBUST=$CACHEBUST" && dotnet build "src/HotelBooking.Api/HotelBooking.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/HotelBooking.Api/HotelBooking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "HotelBooking.Api.dll"]

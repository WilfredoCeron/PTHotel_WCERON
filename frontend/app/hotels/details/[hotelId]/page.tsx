"use client";
import { useEffect, useState } from "react";
import { useParams, useSearchParams, useRouter } from "next/navigation";
import apiClient from "@/lib/api";
import { getRatePlanPrice } from "@/lib/rateplans";

export default function HotelDetailsPage() {
  const params = useParams();
  const searchParams = useSearchParams();
  const router = useRouter();
  const hotelId = params?.hotelId;
  const checkIn = searchParams.get("checkIn") || "";
  const checkOut = searchParams.get("checkOut") || "";
  const guests = Number(searchParams.get("guests")) || 1;

  const [hotel, setHotel] = useState<any>(null);
  const [roomTypes, setRoomTypes] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [prices, setPrices] = useState<{ [roomTypeId: number]: any }>({});

  useEffect(() => {
    if (!hotelId) return;
    const fetchData = async () => {
      try {
        const hotelRes = await apiClient.get(`/hotels/${hotelId}`);
        setHotel(hotelRes.data);
        const roomTypesRes = await apiClient.get(`/room-types?hotelId=${hotelId}`);
        const filtered = roomTypesRes.data.filter((rt: any) => rt.capacity >= guests && rt.isActive);
        setRoomTypes(filtered);
        // Consultar precios para cada tipo de habitación
        const priceMap: { [roomTypeId: number]: any } = {};
        for (const rt of filtered) {
          try {
            priceMap[rt.id] = await getRatePlanPrice(rt.id, checkIn, checkOut);
          } catch {
            priceMap[rt.id] = null;
          }
        }
        setPrices(priceMap);
      } catch {
        setError("No se pudo cargar el hotel o habitaciones");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [hotelId, checkIn, checkOut, guests]);

  const handleReserve = (roomTypeId: number) => {
    router.push(`/bookings/create?hotelId=${hotelId}&roomTypeId=${roomTypeId}&checkIn=${checkIn}&checkOut=${checkOut}&guests=${guests}`);
  };

  if (loading) return <div className="text-center py-5"><div className="spinner-border" role="status"><span className="visually-hidden">Cargando...</span></div></div>;
  if (error) return <div className="alert alert-danger">{error}</div>;
  if (!hotel) return null;

  return (
    <div className="py-4">
      <h2 className="fw-bold mb-3">{hotel.name}</h2>
      <p className="text-muted">{hotel.city}, {hotel.country}</p>
      <p>{hotel.address}</p>
      <h4 className="mt-4 mb-3">Habitaciones Disponibles</h4>
      {roomTypes.length === 0 ? (
        <div className="alert alert-info">No hay habitaciones disponibles para la cantidad de huéspedes seleccionada.</div>
      ) : (
        <div className="row g-4">
          {roomTypes.map((rt) => (
            <div key={rt.id} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body">
                  <h5 className="card-title">{rt.name}</h5>
                  <p className="card-text">{rt.description}</p>
                  <p className="mb-1">Capacidad: {rt.capacity}</p>
                  {prices[rt.id] ? (
                    <>
                      <p className="mb-1">Precio total: <strong>${prices[rt.id].total.toFixed(2)}</strong> ({prices[rt.id].nights} noches)</p>
                      <button className="btn btn-primary" onClick={() => handleReserve(rt.id)}>Reservar</button>
                    </>
                  ) : (
                    <p className="text-danger">No hay tarifa disponible para las fechas seleccionadas.</p>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

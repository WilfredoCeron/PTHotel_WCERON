'use client';
function formatDate(dateStr: string) {
  // Si dateStr ya viene en formato ISO, solo extraer yyyy-mm-dd
  if (typeof dateStr === 'string' && dateStr.length >= 10) {
    return dateStr.substring(0, 10);
  }
  // Fallback por si viene Date
  const d = new Date(dateStr);
  return d.toLocaleDateString('es-MX');
}


import Link from "next/link";
import apiClient from "../../lib/api";
import { useState, useEffect } from "react";

interface Booking {
  id: number;
  hotelName: string;
  roomTypeName: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfRooms: number;
  numberOfGuests: number;
  specialRequests: string;
}

export default function BookingsPage() {
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchBookings = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await apiClient.get("/bookings");
      // Si la respuesta es un objeto con data, usar data; si es array, usar directamente
      const data = Array.isArray(res.data) ? res.data : res.data?.data || [];
      setBookings(data);
    } catch {
      setError("Error al cargar reservas");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBookings();
  }, []);

  const handleDelete = async (id: number) => {
    if (!confirm("¿Cancelar esta reserva?")) return;
    try {
      await apiClient.delete(`/bookings/${id}`);
      setBookings(bookings.filter(b => b.id !== id));
    } catch {
      alert("Error al cancelar la reserva");
    }
  };

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border" role="status">
          <span className="visually-hidden">Cargando...</span>
        </div>
        <p className="mt-3">Cargando tus reservas...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="alert alert-danger mt-4" role="alert">
        <h4 className="alert-heading">¡Error!</h4>
        <p>{error}</p>
      </div>
    );
  }

  return (
    <div className="container py-4">
      <h2 className="mb-4">Mis Reservas</h2>
      <Link href="/hotels" className="btn btn-secondary mb-3">Buscar hoteles</Link>
      <table className="table">
        <thead>
          <tr>
            <th>Hotel</th>
            <th>Habitación</th>
            <th>Entrada</th>
            <th>Salida</th>
            <th>Habitaciones</th>
            <th>Huéspedes</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {bookings.map(b => (
            <tr key={b.id}>
              <td>{b.hotelName}</td>
              <td>{b.roomTypeName}</td>
              <td>{formatDate(b.checkInDate)}</td>
              <td>{formatDate(b.checkOutDate)}</td>
              <td>{b.numberOfRooms}</td>
              <td>{b.numberOfGuests}</td>
              <td>
                <button className="btn btn-danger btn-sm" onClick={() => handleDelete(b.id)}>
                  Cancelar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {bookings.length === 0 && !loading && <div>No tienes reservas.</div>}
    </div>
  );
}

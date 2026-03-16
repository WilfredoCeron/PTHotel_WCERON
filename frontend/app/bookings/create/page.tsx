"use client";
import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import apiClient from "../../../lib/api";

export default function CreateBookingPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const hotelId = searchParams.get("hotelId");
  const roomTypeId = searchParams.get("roomTypeId");
  const checkIn = searchParams.get("checkIn") || "";
  const checkOut = searchParams.get("checkOut") || "";
  const guests = Number(searchParams.get("guests")) || 1;

  const [form, setForm] = useState({
    guestId: 1, // Simulado, en real sería el usuario logueado
    numberOfRooms: 1,
    specialRequests: ""
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);

  const handleChange = (e: any) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: any) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    setSuccess(false);
    try {
      await apiClient.post("/bookings", {
        hotelId: Number(hotelId),
        roomTypeId: Number(roomTypeId),
        guestId: form.guestId,
        checkInDate: checkIn,
        checkOutDate: checkOut,
        numberOfRooms: Number(form.numberOfRooms),
        numberOfGuests: guests,
        specialRequests: form.specialRequests
      });
      setSuccess(true);
      setTimeout(() => router.push("/bookings"), 1500);
    } catch {
      setError("Error al crear la reserva. Verifica disponibilidad.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="card p-4" onSubmit={handleSubmit}>
      <h3 className="mb-3">Reservar Habitación</h3>
      <div className="mb-2">
        <label>Fecha Entrada</label>
        <input type="date" value={checkIn} disabled className="form-control" />
      </div>
      <div className="mb-2">
        <label>Fecha Salida</label>
        <input type="date" value={checkOut} disabled className="form-control" />
      </div>
      <div className="mb-2">
        <label>Huéspedes</label>
        <input type="number" value={guests} disabled className="form-control" />
      </div>
      <div className="mb-2">
        <label>Habitaciones</label>
        <input name="numberOfRooms" type="number" min="1" value={form.numberOfRooms} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Solicitudes Especiales</label>
        <input name="specialRequests" value={form.specialRequests} onChange={handleChange} className="form-control" />
      </div>
      {error && <div className="alert alert-danger">{error}</div>}
      {success && <div className="alert alert-success">Reserva realizada con éxito</div>}
      <button className="btn btn-primary mt-2" disabled={loading}>
        {loading ? "Guardando..." : "Reservar"}
      </button>
    </form>
  );
}

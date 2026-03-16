"use client";
import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import apiClient from "@/lib/api";

export default function CreateRatePlanPage() {
  const router = useRouter();
  const [form, setForm] = useState({
    hotelId: 0,
    roomTypeId: 0,
    price: 0,
    startDate: "",
    endDate: "",
    isActive: true,
  });
  const [hotels, setHotels] = useState([]);
  const [roomTypes, setRoomTypes] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    apiClient.get("/hotels").then(res => setHotels(res.data));
  }, []);

  useEffect(() => {
    if (!form.hotelId) {
      setRoomTypes([]);
      return;
    }
    apiClient.get(`/room-types?hotelId=${form.hotelId}`).then(res => setRoomTypes(res.data));
  }, [form.hotelId]);

  const handleChange = (e: any) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: any) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    // Validación de fechas
    if (form.endDate < form.startDate) {
      setError("La fecha de fin no puede ser menor que la fecha de inicio.");
      setLoading(false);
      return;
    }
    try {
      await apiClient.post("/rateplans", {
        ...form,
        roomTypeId: Number(form.roomTypeId),
        price: Number(form.price),
      });
      router.push("/rateplans");
    } catch {
      setError("Error al crear la tarifa");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="card p-4" onSubmit={handleSubmit}>
      <h3 className="mb-3">Agregar Tarifa</h3>
      <div className="mb-2">
        <label>Hotel</label>
        <select name="hotelId" value={form.hotelId} onChange={handleChange} className="form-control" required>
          <option value="">Seleccione...</option>
          {hotels.map((hotel: any) => (
            <option key={hotel.id} value={hotel.id}>{hotel.name}</option>
          ))}
        </select>
      </div>
      <div className="mb-2">
        <label>Tipo de Habitación</label>
        <select name="roomTypeId" value={form.roomTypeId} onChange={handleChange} className="form-control" required disabled={!form.hotelId}>
          <option value="">Seleccione...</option>
          {roomTypes.map((rt: any) => (
            <option key={rt.id} value={rt.id}>{rt.name}</option>
          ))}
        </select>
      </div>
      {/* Campos eliminados: Nombre y Descripción */}
      <div className="mb-2">
        <label>Precio</label>
        <input name="price" type="number" min="0" value={form.price} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Fecha Inicio</label>
        <input name="startDate" type="date" value={form.startDate} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Fecha Fin</label>
        <input name="endDate" type="date" value={form.endDate} onChange={handleChange} className="form-control" required />
      </div>
      <div className="form-check mb-2">
        <input className="form-check-input" type="checkbox" name="isActive" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} id="isActiveCheck" />
        <label className="form-check-label" htmlFor="isActiveCheck">Activo</label>
      </div>
      {error && <div className="alert alert-danger">{error}</div>}
      <button className="btn btn-primary mt-2" disabled={loading}>
        {loading ? "Guardando..." : "Crear"}
      </button>
    </form>
  );
}

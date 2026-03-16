"use client";
import { useState } from "react";
import apiClient from "../../lib/api";
import { useRouter, useSearchParams } from "next/navigation";

export default function CreateRoomTypeForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const hotelId = searchParams.get("hotelId") || "";
  const [form, setForm] = useState({
    hotelId: hotelId,
    name: "",
    description: "",
    capacity: 1,
    totalRooms: 1
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      await apiClient.post("/room-types", {
        ...form,
        hotelId: Number(form.hotelId),
        capacity: Number(form.capacity),
        totalRooms: Number(form.totalRooms)
      });
      router.push(`/room-types?hotelId=${form.hotelId}`);
    } catch (err: any) {
      if (err?.response?.status === 409 && err?.response?.data?.message) {
        setError("Error al registrar el tipo de habitación, nombre de habitación ya utilizado");
      } else {
        setError("Error al registrar el tipo de habitación, nombre ya utilizado");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="card p-4" onSubmit={handleSubmit}>
      <h3 className="mb-3">Registrar Tipo de Habitación</h3>
      <div className="mb-2">
        <label>Nombre</label>
        <input name="name" value={form.name} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Descripción</label>
        <input name="description" value={form.description} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Capacidad</label>
        <input name="capacity" type="number" min="1" value={form.capacity} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Total de habitaciones</label>
        <input name="totalRooms" type="number" min="1" value={form.totalRooms} onChange={handleChange} className="form-control" required />
      </div>
      {error && <div className="alert alert-danger">{error}</div>}
      <button className="btn btn-primary mt-2" disabled={loading}>
        {loading ? "Guardando..." : "Registrar"}
      </button>
    </form>
  );
}

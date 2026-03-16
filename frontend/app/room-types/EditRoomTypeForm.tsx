"use client";
import { useState, useEffect } from "react";
import axios from "axios";
import { useRouter, useParams, useSearchParams } from "next/navigation";

export default function EditRoomTypeForm() {
  const router = useRouter();
  const params = useParams();
  const searchParams = useSearchParams();
  const hotelId = searchParams.get("hotelId") || "";
  const roomTypeId = params?.roomTypeId;
  const [form, setForm] = useState({
    name: "",
    description: "",
    capacity: 1,
    totalRooms: 1
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!roomTypeId) return;
    const fetchRoomType = async () => {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
        const res = await axios.get(`${apiUrl}/api/v1/room-types/${roomTypeId}`);
        setForm(res.data);
      } catch {
        setError("No se pudo cargar el tipo de habitación");
      }
    };
    fetchRoomType();
  }, [roomTypeId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
      await axios.put(`${apiUrl}/api/v1/room-types/${roomTypeId}`, {
        ...form,
        capacity: Number(form.capacity),
        totalRooms: Number(form.totalRooms)
      });
      router.push(`/room-types?hotelId=${hotelId}`);
    } catch {
      setError("Error al actualizar el tipo de habitación");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="card p-4" onSubmit={handleSubmit}>
      <h3 className="mb-3">Editar Tipo de Habitación</h3>
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
        {loading ? "Guardando..." : "Actualizar"}
      </button>
    </form>
  );
}

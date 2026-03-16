"use client";
import { useState, useEffect } from "react";
import axios from "axios";
import { useRouter, useParams } from "next/navigation";

export default function EditHotelForm() {
  const router = useRouter();
  const params = useParams();
  const hotelId = params?.hotelId;
  const [form, setForm] = useState({
    name: "",
    address: "",
    city: "",
    country: "",
    phone: "",
    email: ""
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!hotelId) return;
    const fetchHotel = async () => {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
        const res = await axios.get(`${apiUrl}/api/v1/hotels/${hotelId}`);
        setForm(res.data);
      } catch {
        setError("No se pudo cargar el hotel");
      }
    };
    fetchHotel();
  }, [hotelId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
      await axios.put(`${apiUrl}/api/v1/hotels/${hotelId}`, form);
      router.push("/hotels");
    } catch {
      setError("Error al actualizar el hotel, nombre de hotel ya utilizado");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="card p-4" onSubmit={handleSubmit}>
      <h3 className="mb-3">Editar Hotel</h3>
      <div className="mb-2">
        <label>Nombre</label>
        <input name="name" value={form.name} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Dirección</label>
        <input name="address" value={form.address} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Ciudad</label>
        <input name="city" value={form.city} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>País</label>
        <input name="country" value={form.country} onChange={handleChange} className="form-control" required />
      </div>
      <div className="mb-2">
        <label>Teléfono</label>
        <input name="phone" value={form.phone} onChange={handleChange} className="form-control" />
      </div>
      <div className="mb-2">
        <label>Email</label>
        <input name="email" value={form.email} onChange={handleChange} className="form-control" />
      </div>
      {error && <div className="alert alert-danger">{error}</div>}
      <button className="btn btn-primary mt-2" disabled={loading}>
        {loading ? "Guardando..." : "Actualizar"}
      </button>
    </form>
  );
}

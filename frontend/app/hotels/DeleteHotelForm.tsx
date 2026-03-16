"use client";
import { useRouter, useParams } from "next/navigation";
import axios from "axios";
import { useState } from "react";

export default function DeleteHotelForm() {
  const router = useRouter();
  const params = useParams();
  const hotelId = params?.hotelId;
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleDelete = async () => {
    setLoading(true);
    setError("");
    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
      await axios.delete(`${apiUrl}/api/v1/hotels/${hotelId}`);
      router.push("/hotels");
    } catch {
      setError("Error al eliminar el hotel");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card p-4">
      <h3 className="mb-3">Eliminar Hotel</h3>
      <p>¿Seguro que deseas eliminar este hotel?</p>
      {error && <div className="alert alert-danger">{error}</div>}
      <button className="btn btn-danger mt-2" onClick={handleDelete} disabled={loading}>
        {loading ? "Eliminando..." : "Eliminar"}
      </button>
      <button className="btn btn-secondary mt-2 ms-2" onClick={() => router.push("/hotels")}>Cancelar</button>
    </div>
  );
}

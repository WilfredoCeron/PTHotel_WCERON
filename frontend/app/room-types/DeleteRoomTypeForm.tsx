"use client";
import { useRouter, useParams, useSearchParams } from "next/navigation";
import axios from "axios";
import { useState } from "react";

export default function DeleteRoomTypeForm() {
  const router = useRouter();
  const params = useParams();
  const searchParams = useSearchParams();
  const hotelId = searchParams.get("hotelId") || "";
  const roomTypeId = params?.roomTypeId;
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleDelete = async () => {
    setLoading(true);
    setError("");
    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
      await axios.delete(`${apiUrl}/api/v1/room-types/${roomTypeId}`);
      router.push(`/room-types?hotelId=${hotelId}`);
    } catch {
      setError("Error al eliminar el tipo de habitación");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card p-4">
      <h3 className="mb-3">Eliminar Tipo de Habitación</h3>
      <p>¿Seguro que deseas eliminar este tipo de habitación?</p>
      {error && <div className="alert alert-danger">{error}</div>}
      <button className="btn btn-danger mt-2" onClick={handleDelete} disabled={loading}>
        {loading ? "Eliminando..." : "Eliminar"}
      </button>
      <button className="btn btn-secondary mt-2 ms-2" onClick={() => router.push(`/room-types?hotelId=${hotelId}`)}>Cancelar</button>
    </div>
  );
}

"use client";
import { useEffect, useState } from 'react';
import axios from 'axios';
import { useSearchParams } from 'next/navigation';

interface RoomType {
  id: number;
  hotelId: number;
  name: string;
  description: string;
  capacity: number;
  totalRooms: number;
  isActive: boolean;
}

export default function RoomTypesPage() {
  const searchParams = useSearchParams();
  const hotelId = searchParams.get('hotelId');
  const [roomTypes, setRoomTypes] = useState<RoomType[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!hotelId) return;
    const fetchRoomTypes = async () => {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
        const response = await axios.get(`${apiUrl}/api/v1/room-types?hotelId=${hotelId}`);
        setRoomTypes(response.data);
      } catch (err) {
        setError('No se pudieron cargar los tipos de habitación');
      } finally {
        setLoading(false);
      }
    };
    fetchRoomTypes();
  }, [hotelId]);

  return (
    <div className="py-4">
      <h2 className="display-5 fw-bold mb-4">🛏️ Tipos de Habitación</h2>
      <a href={`/room-types/create?hotelId=${hotelId}`} className="btn btn-success mb-3">Agregar Tipo de Habitación</a>
      {loading ? (
        <div className="text-center py-5">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Cargando...</span>
          </div>
        </div>
      ) : error ? (
        <div className="alert alert-danger">{error}</div>
      ) : roomTypes.length === 0 ? (
        <div className="alert alert-info">No hay tipos de habitación registrados para este hotel.</div>
      ) : (
        <div className="row g-4">
          {roomTypes.map((rt) => (
            <div key={rt.id} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-header bg-light">
                  <h5 className="card-title mb-0">{rt.name}</h5>
                </div>
                <div className="card-body">
                  <p className="text-muted">{rt.description}</p>
                  <p className="text-muted">Capacidad: {rt.capacity}</p>
                  <p className="text-muted">Total de habitaciones: {rt.totalRooms}</p>
                </div>
                <div className="card-footer bg-light d-flex gap-2">
                  <a href={`/room-types/edit/${rt.id}?hotelId=${hotelId}`} className="btn btn-warning btn-sm">Editar</a>
                  <a href={`/room-types/delete/${rt.id}?hotelId=${hotelId}`} className="btn btn-danger btn-sm">Eliminar</a>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

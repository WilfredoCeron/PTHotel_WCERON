"use client";
import Link from 'next/link';
import { useEffect, useState } from 'react';
import axios from 'axios';
import apiClient from '../../lib/api';

interface Hotel {
  id: number;
  name: string;
  address: string;
  city: string;
  country: string;
  phone?: string;
  email?: string;
  latitude?: number;
  longitude?: number;
  isActive: boolean;
}

export default function HotelsPage() {
  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchHotels = async () => {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
        const response = await axios.get(`${apiUrl}/api/v1/hotels`);
        setHotels(response.data);
      } catch (err) {
        setError('No se pudieron cargar los hoteles');
      } finally {
        setLoading(false);
      }
    };
    fetchHotels();
  }, []);

  // Función para activar/inactivar hotel
  const toggleActive = async (hotelId: number, isActive: boolean) => {
    // Cambio inmediato en UI (optimista)
    setHotels((prev) => prev.map(h => h.id === hotelId ? { ...h, isActive: !isActive } : h));
    // Llamada a la API (opcional, pero sin alert ni rollback visual)
    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      await axios.patch(`${apiUrl}/api/v1/hotels/${hotelId}/${isActive ? 'inactivate' : 'activate'}`);
    } catch (err) {
      // No mostrar alertas ni revertir visualmente
    }
  };

  return (
    <div className="py-4">
      <h2 className="display-5 fw-bold mb-4">🏨 Hoteles</h2>
      <Link href="/hotels/create" className="btn btn-success mb-3">Agregar Hotel</Link>
      {loading ? (
        <div className="text-center py-5">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Cargando...</span>
          </div>
        </div>
      ) : error ? (
        <div className="alert alert-danger">{error}</div>
      ) : hotels.length === 0 ? (
        <div className="alert alert-info">No hay hoteles registrados.</div>
      ) : (
        <div className="row g-4">
          {hotels.map((hotel) => (
            <div key={hotel.id} className="col-md-6 col-lg-4">
              <div
                className={`card h-100 shadow-sm ${!hotel.isActive ? 'border border-danger bg-danger bg-opacity-10' : ''}`}
              >
                <div className="card-header bg-light d-flex align-items-center justify-content-between">
                  <h5 className="card-title mb-0">{hotel.name}</h5>
                  {!hotel.isActive && (
                    <span className="badge bg-danger text-light">Inactivo</span>
                  )}
                </div>
                <div className="card-body">
                  <p className="text-muted small mb-2">📍 {hotel.city}, {hotel.country}</p>
                  <p className="text-muted">{hotel.address}</p>
                  <p className="text-muted">Tel: {hotel.phone || 'N/A'}</p>
                  <p className="text-muted">Email: {hotel.email || 'N/A'}</p>
                </div>
                <div className="card-footer bg-light d-flex gap-2 flex-wrap">
                  <Link href={`/hotels/edit/${hotel.id}`} className="btn btn-warning btn-sm">Editar</Link>
                  <Link href={`/hotels/delete/${hotel.id}`} className="btn btn-danger btn-sm">Eliminar</Link>
                  <Link href={`/room-types?hotelId=${hotel.id}`} className="btn btn-info btn-sm">Tipos de Habitación</Link>
                  {hotel.isActive ? (
                    <button
                      className="btn btn-outline-danger btn-sm"
                      onClick={() => toggleActive(hotel.id, true)}
                    >
                      Deshabilitar
                    </button>
                  ) : (
                    <button
                      className="btn btn-outline-success btn-sm"
                      onClick={() => toggleActive(hotel.id, false)}
                    >
                      Habilitar
                    </button>
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

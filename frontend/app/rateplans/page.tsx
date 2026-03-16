"use client";
import Link from "next/link";
import { useEffect, useState } from "react";
import apiClient from "@/lib/api";

interface RatePlan {
  id: number;
  roomTypeId: number;
  name: string;
  description?: string;
  price: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export default function RatePlansPage() {
  const [ratePlans, setRatePlans] = useState<RatePlan[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRatePlans = async () => {
      try {
        const response = await apiClient.get("/rateplans");
        setRatePlans(response.data);
      } catch {
        setError("No se pudieron cargar las tarifas");
      } finally {
        setLoading(false);
      }
    };
    fetchRatePlans();
  }, []);

  return (
    <div className="py-4">
      <h2 className="display-5 fw-bold mb-4">💲 Tarifas (RatePlans)</h2>
      <Link href="/rateplans/create" className="btn btn-success mb-3">Agregar Tarifa</Link>
      {loading ? (
        <div className="text-center py-5">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Cargando...</span>
          </div>
        </div>
      ) : error ? (
        <div className="alert alert-danger">{error}</div>
      ) : ratePlans.length === 0 ? (
        <div className="alert alert-info">No hay tarifas registradas.</div>
      ) : (
        <div className="table-responsive">
          <table className="table table-bordered table-hover">
            <thead>
              <tr>
                <th>ID</th>
                <th>RoomType</th>
                <th>Nombre</th>
                <th>Precio</th>
                <th>Vigencia</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {ratePlans.map((rate) => (
                <tr key={rate.id}>
                  <td>{rate.id}</td>
                  <td>{rate.roomTypeId}</td>
                  <td>{rate.name}</td>
                  <td>${rate.price.toFixed(2)}</td>
                  <td>{rate.startDate} a {rate.endDate}</td>
                  <td>{rate.isActive ? "Activo" : "Inactivo"}</td>
                  <td>
                    <Link href={`/rateplans/edit/${rate.id}`} className="btn btn-sm btn-primary me-2">Editar</Link>
                    <Link href={`/rateplans/delete/${rate.id}`} className="btn btn-sm btn-danger">Eliminar</Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

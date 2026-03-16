"use client";
import { useRouter, useParams } from "next/navigation";
import { useState, useEffect } from "react";
import apiClient from "@/lib/api";

export default function DeleteRatePlanPage() {
  const router = useRouter();
  const params = useParams();
  const ratePlanId = params?.ratePlanId;
  const [ratePlan, setRatePlan] = useState<any>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!ratePlanId) return;
    apiClient.get(`/rateplans/${ratePlanId}`).then(res => setRatePlan(res.data)).catch(() => setError("No se pudo cargar la tarifa"));
  }, [ratePlanId]);

  const handleDelete = async () => {
    setLoading(true);
    setError("");
    try {
      await apiClient.delete(`/rateplans/${ratePlanId}`);
      router.push("/rateplans");
    } catch {
      setError("Error al eliminar la tarifa");
    } finally {
      setLoading(false);
    }
  };

  if (!ratePlan) return null;

  return (
    <div className="card p-4">
      <h3 className="mb-3">Eliminar Tarifa</h3>
      <p>¿Estás seguro que deseas eliminar esta tarifa?</p>
      {error && <div className="alert alert-danger">{error}</div>}
      <button className="btn btn-danger me-2" onClick={handleDelete} disabled={loading}>
        {loading ? "Eliminando..." : "Eliminar"}
      </button>
      <button className="btn btn-secondary" onClick={() => router.push("/rateplans")}>Cancelar</button>
    </div>
  );
}

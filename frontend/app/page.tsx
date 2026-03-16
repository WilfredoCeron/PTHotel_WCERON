'use client'

import { useState } from 'react'
import axios from 'axios'

interface HotelSearchForm {
  checkIn: string
  checkOut: string
  guests: number
}

export default function Home() {
  const [searchForm, setSearchForm] = useState<HotelSearchForm>({
    checkIn: '',
    checkOut: '',
    guests: 1,
  })
  const [searchResults, setSearchResults] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setError(null)

    // Validar que la fecha de salida sea posterior a la de entrada
    if (searchForm.checkOut <= searchForm.checkIn) {
      setError('La fecha de salida debe ser posterior a la fecha de entrada')
      setLoading(false)
      return
    }

    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000'
      const params = new URLSearchParams({
        checkIn: searchForm.checkIn,
        checkOut: searchForm.checkOut,
        guests: String(searchForm.guests),
      });
      const response = await axios.get(`${apiUrl}/api/v1/hotels/search-availability?${params.toString()}`);
      setSearchResults(response.data)
    } catch (err) {
      setError('No se pudieron buscar hoteles disponibles. Por favor, intenta de nuevo.')
      console.error(err)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="py-4">
      {/* Header Hero */}
      <div className="bg-gradient-primary text-white p-5 rounded-3 mb-5" style={{ background: 'linear-gradient(135deg, #0066cc 0%, #0052a3 100%)' }}>
        <h2 className="display-4 fw-bold mb-2">🌍 Encuentra Tu Hotel Perfecto</h2>
        <p className="lead">Busca y reserva hoteles increíbles a precios excelentes</p>
      </div>

      {/* Search Form */}
      <div className="card shadow-sm mb-5">
        <div className="card-header bg-primary text-white">
          <h5 className="mb-0">Buscar Hoteles</h5>
        </div>
        <div className="card-body">
          <form onSubmit={handleSearch}>
            <div className="row g-3 mb-3">
              {/* Check-In Date */}
              <div className="col-md-3">
                <label className="form-label fw-semibold">Fecha de Entrada</label>
                <input
                  type="date"
                  value={searchForm.checkIn}
                  onChange={(e) =>
                    setSearchForm({ ...searchForm, checkIn: e.target.value })
                  }
                  required
                  className="form-control form-control-lg"
                />
              </div>

              {/* Check-Out Date */}
              <div className="col-md-3">
                <label className="form-label fw-semibold">Fecha de Salida</label>
                <input
                  type="date"
                  value={searchForm.checkOut}
                  onChange={(e) =>
                    setSearchForm({ ...searchForm, checkOut: e.target.value })
                  }
                  required
                  className="form-control form-control-lg"
                />
              </div>

              {/* Number of Guests */}
              <div className="col-md-3">
                <label className="form-label fw-semibold">Huéspedes</label>
                <select
                  value={searchForm.guests}
                  onChange={(e) =>
                    setSearchForm({
                      ...searchForm,
                      guests: parseInt(e.target.value),
                    })
                  }
                  className="form-select form-select-lg"
                >
                  {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((n) => (
                    <option key={n} value={n}>{n} {n === 1 ? 'Huésped' : 'Huéspedes'}</option>
                  ))}
                </select>
              </div>

              {/* Search Button */}
              <div className="col-md-3 d-flex align-items-end">
                <button
                  type="button"
                  onClick={handleSearch}
                  disabled={loading}
                  className="btn btn-primary btn-lg w-100"
                >
                  {loading ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                      Buscando...
                    </>
                  ) : (
                    '🔍 Buscar'
                  )}
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <div className="alert alert-danger alert-dismissible fade show mb-4" role="alert">
          <strong>¡Error!</strong> {error}
          <button type="button" className="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
      )}

      {/* No Results Message */}
      {!loading && searchResults.length === 0 && !error && (
        <div className="alert alert-info">
          <h5 className="alert-heading">¡Bienvenido!</h5>
          <p>Usa el formulario de arriba para encontrar hoteles disponibles en tus fechas.</p>
        </div>
      )}

      {/* Search Results */}
      {searchResults.length > 0 && (
        <div>
          <h3 className="mb-4">Hoteles Disponibles ({searchResults.length})</h3>
          <div className="row g-4">
            {searchResults.map((hotel: any) => (
              <div key={hotel.id} className="col-md-6 col-lg-4">
                <div className="card h-100 shadow-sm hover-shadow" style={{ transition: 'box-shadow 0.3s' }}>
                  <div className="card-header bg-light">
                    <h5 className="card-title mb-0">{hotel.name}</h5>
                  </div>
                  <div className="card-body">
                    <p className="text-muted small mb-2">📍 {hotel.city}, {hotel.country}</p>
                    <p className="text-muted">{hotel.address}</p>
                    <div className="d-flex justify-content-between align-items-center mt-3">
                      <span className="badge bg-success fs-6">${hotel.price}/noche</span>
                      <small className="text-muted">⭐ 4.5/5</small>
                    </div>
                  </div>
                  <div className="card-footer bg-light">
                    <button className="btn btn-primary btn-sm w-100">Ver Detalles</button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}

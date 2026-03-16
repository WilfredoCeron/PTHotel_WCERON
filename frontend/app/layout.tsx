import type { Metadata } from "next"
import 'bootstrap/dist/css/bootstrap.min.css'
import "./layout.css"

export const metadata: Metadata = {
  title: "Plataforma de Reservas Hoteleras",
  description: "Reserva tu hotel perfecto en línea",
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body className="d-flex flex-column min-vh-100">
        {/* Navbar */}
        <nav className="navbar navbar-expand-lg navbar-dark bg-primary sticky-top">
          <div className="container-fluid">
            <a className="navbar-brand fs-4 fw-bold" href="/">
              🏨 Plataforma de Reservas Hoteleras
            </a>
            <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#navbarNav"
              aria-controls="navbarNav"
              aria-expanded="false"
              aria-label="Toggle navigation"
            >
              <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbarNav">
              <ul className="navbar-nav ms-auto">
                <li className="nav-item">
                  <a className="nav-link active" href="/">
                    Inicio
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="/bookings">
                    Mis Reservas
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="/">
                    Buscar Hoteles
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="/hotels">
                      CRUD Hoteles
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="/rateplans">
                      CRUD Tarifas
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        {/* Main Content */}
        <main className="flex-grow-1 d-flex flex-column">
          <div className="container-fluid">
            {children}
          </div>
        </main>

        {/* Footer */}
        <footer className="bg-dark text-white py-4 mt-auto">
          <div className="container-fluid">
            <div className="text-center text-white-50">
              <p>&copy; 2026 Plataforma de Reservas Hoteleras. Todos los derechos reservados.</p>
            </div>
          </div>
        </footer>

        {/* Bootstrap JS */}
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
      </body>
    </html>
  )
}

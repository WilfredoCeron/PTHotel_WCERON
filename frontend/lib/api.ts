import axios from 'axios'

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000'

export const apiClient = axios.create({
  baseURL: `${API_URL}/api/v1`,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add request interceptor for Idempotency-Key
apiClient.interceptors.request.use((config) => {
  if (config.method === 'post') {
    config.headers['Idempotency-Key'] = `${Date.now()}-${Math.random()}`
  }
  return config
})

// Add response interceptor for CorrelationId
apiClient.interceptors.response.use((response) => {
  const correlationId = response.headers['x-correlation-id']
  if (correlationId) {
    console.log(`Request CorrelationId: ${correlationId}`)
  }
  return response
})

export default apiClient

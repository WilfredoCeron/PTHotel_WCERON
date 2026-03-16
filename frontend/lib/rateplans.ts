import apiClient from './api';

export async function getRatePlanPrice(roomTypeId: number, checkIn: string, checkOut: string) {
  const res = await apiClient.get('/rateplans/pricing', {
    params: { roomTypeId, checkIn, checkOut }
  });
  return res.data;
}

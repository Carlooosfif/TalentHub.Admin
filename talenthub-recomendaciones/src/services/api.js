const API_BASE_URL = process.env.REACT_APP_API_URL;

export async function obtenerRecomendaciones(vacanteId) {
  const response = await fetch(
    `${API_BASE_URL}/api/vacantes/${vacanteId}/recomendaciones`
  );

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.mensaje || "Error al obtener recomendaciones");
  }

  return await response.json();
}

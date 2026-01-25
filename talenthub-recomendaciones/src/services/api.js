const API_BASE_URL = "https://talenthub-admin-carlos.azurewebsites.net/api";

export async function obtenerRecomendaciones(vacanteId) {
  const response = await fetch(`${API_BASE_URL}/vacantes/${vacanteId}/recomendaciones`);

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.mensaje || "Error al obtener recomendaciones");
  }

  return await response.json();
}

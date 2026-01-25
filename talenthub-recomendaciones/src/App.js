import { useState } from "react";
import VacanteForm from "./components/VacanteForm";
import RecomendacionesTable from "./components/RecomendacionesTable";
import { obtenerRecomendaciones } from "./services/api";

function App() {
  const [recomendaciones, setRecomendaciones] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const buscar = async (vacanteId) => {
    setLoading(true);
    setError("");
    setRecomendaciones([]);

    try {
      const data = await obtenerRecomendaciones(vacanteId);
      setRecomendaciones(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: "30px", fontFamily: "Arial" }}>
      <h2>Recomendaciones por Vacante</h2>

      <VacanteForm onBuscar={buscar} />

      {loading && <p>Cargando...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}

      <RecomendacionesTable data={recomendaciones} />
    </div>
  );
}

export default App;

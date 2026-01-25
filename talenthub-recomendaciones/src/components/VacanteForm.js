import { useState } from "react";

export default function VacanteForm({ onBuscar }) {
  const [vacanteId, setVacanteId] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (vacanteId.trim() === "") return;
    onBuscar(vacanteId);
  };

  return (
    <form onSubmit={handleSubmit} style={{ marginBottom: "20px" }}>
      <label>
        ID de Vacante:
        <input
          type="number"
          value={vacanteId}
          onChange={(e) => setVacanteId(e.target.value)}
          style={{ marginLeft: "10px" }}
        />
      </label>
      <button type="submit" style={{ marginLeft: "10px" }}>
        Buscar
      </button>
    </form>
  );
}

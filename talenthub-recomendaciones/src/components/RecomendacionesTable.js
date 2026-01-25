export default function RecomendacionesTable({ data }) {
  if (data.length === 0) return null;

  return (
    <table border="1" cellPadding="8">
      <thead>
        <tr>
          <th>Empleado</th>
          <th>Correo</th>
          <th>Score Supervisor</th>
        </tr>
      </thead>
      <tbody>
        {data.map((e) => (
          <tr key={e.empleadoId}>
            <td>{e.nombreCompleto}</td>
            <td>{e.correo}</td>
            <td>{e.scoreSupervisor}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

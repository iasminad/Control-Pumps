const API_BASE = "http://localhost:5000/api";

export async function getStatus() {
  const res = await fetch(`${API_BASE}/pompa`);
  return res.json();
}

export async function togglePompa(id) {
  const res = await fetch(`${API_BASE}/pompa/${id}`, {
    method: "POST",
  });
  return res.json();
}

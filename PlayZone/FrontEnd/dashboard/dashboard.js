const API = "https://localhost:7239/api/Dashboard/resumen";

const TOKEN = localStorage.getItem("token");

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN,
};

async function cargarDashboard() {
    const respuesta = await fetch(API, { headers: HEADERS });

    if (!respuesta.ok) {
        document.getElementById("mensajeDashboard").textContent = "Error " + respuesta.status;
        return;
    }

    const resumen = await respuesta.json();

    // Tarjetas
    document.getElementById("totalOrdenes").textContent = resumen.totalOrdenes;
    document.getElementById("ordenesPendientes").textContent = resumen.ordenesPendientes;
    document.getElementById("ordenesCompletadas").textContent = resumen.ordenesCompletadas;
    document.getElementById("totalProductos").textContent = resumen.totalProductos;
    document.getElementById("totalUsuarios").textContent = resumen.totalUsuarios;
    document.getElementById("totalReembolsos").textContent = resumen.totalReembolsos;

    // Barra de distribución
    const total = resumen.totalOrdenes;
    const otros = total - resumen.ordenesPendientes - resumen.ordenesCompletadas;

    if (total > 0) {
        document.getElementById("barraPendientes").style.width = (resumen.ordenesPendientes / total) * 100 + "%";
        document.getElementById("barraCompletadas").style.width = (resumen.ordenesCompletadas / total) * 100 + "%";
        document.getElementById("barraOtros").style.width = (otros / total) * 100 + "%";
    }

    document.getElementById("mensajeDashboard").textContent = "";
}

cargarDashboard();
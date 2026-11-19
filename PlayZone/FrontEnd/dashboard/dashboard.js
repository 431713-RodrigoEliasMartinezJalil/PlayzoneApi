const API = "https://localhost:7239/api/Dashboard/resumen";

const TOKEN = localStorage.getItem("token");

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN,
};

// Texto claro para que se lea sobre el fondo oscuro
Chart.defaults.color = "#ffffff";

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

    // Gráfico de torta: órdenes según su estado
    const otros = resumen.totalOrdenes - resumen.ordenesPendientes - resumen.ordenesCompletadas;

    new Chart(document.getElementById("graficoTorta"), {
        type: "pie",
        data: {
            labels: ["Pendientes", "Completadas", "Otros"],
            datasets: [{
                data: [resumen.ordenesPendientes, resumen.ordenesCompletadas, otros],
                backgroundColor: ["#ffc107", "#198754", "#6c757d"],
            }],
        },
    });

    // Gráfico de barras: resumen general
    new Chart(document.getElementById("graficoBarras"), {
        type: "bar",
        data: {
            labels: ["Órdenes", "Productos", "Usuarios", "Reembolsos"],
            datasets: [{
                label: "Cantidad",
                data: [
                    resumen.totalOrdenes,
                    resumen.totalProductos,
                    resumen.totalUsuarios,
                    resumen.totalReembolsos,
                ],
                backgroundColor: "#55d6e8",
            }],
        },
        options: {
            scales: { y: { beginAtZero: true, ticks: { precision: 0 } } },
        },
    });

    document.getElementById("mensajeDashboard").textContent = "";
}

cargarDashboard();

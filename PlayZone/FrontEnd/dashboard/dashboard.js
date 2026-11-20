
const API_BASE = "https://localhost:7239/api/Dashboard";
const TOKEN = localStorage.getItem("token");

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN
};

Chart.defaults.color = "#ffffff";

let graficoTorta = null;
let graficoBarras = null;
let graficoIngresos = null;
let graficoVideojuegos = null;

const meses = [
    "Enero", "Febrero", "Marzo", "Abril",
    "Mayo", "Junio", "Julio", "Agosto",
    "Septiembre", "Octubre", "Noviembre", "Diciembre"
];

async function consultarAPI(url) {
    const respuesta = await fetch(url, { headers: HEADERS });

    if (!respuesta.ok) {
        throw new Error("Error HTTP " + respuesta.status);
    }

    return await respuesta.json();
}

function formatoNumero(valor) {
    return Number(valor || 0).toLocaleString("es-AR");
}

function formatoImporte(valor) {
    return Number(valor || 0).toLocaleString("es-AR", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function escaparHTML(valor) {
    return String(valor ?? "").replace(/[&<>"']/g, caracter => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': "&quot;",
        "'": "&#039;"
    })[caracter]);
}

function mostrarMensaje(id, mensaje) {
    document.getElementById(id).textContent = mensaje;
}

// ========================================
// RESUMEN GENERAL
// ========================================

async function cargarResumen() {
    const resumen = await consultarAPI(`${API_BASE}/resumen`);

    document.getElementById("totalOrdenes").textContent =
        formatoNumero(resumen.totalOrdenes);

    document.getElementById("ordenesPendientes").textContent =
        formatoNumero(resumen.ordenesPendientes);

    document.getElementById("ordenesCompletadas").textContent =
        formatoNumero(resumen.ordenesCompletadas);

    // El DTO utiliza ProductosPublicados, no TotalProductos.
    document.getElementById("totalProductos").textContent =
        formatoNumero(resumen.productosPublicados);

    document.getElementById("totalUsuarios").textContent =
        formatoNumero(resumen.totalUsuarios);

    document.getElementById("totalReembolsos").textContent =
        formatoNumero(resumen.totalReembolsos);

    document.getElementById("reembolsosPendientes").textContent =
        `Pendientes: ${formatoNumero(resumen.reembolsosPendientes)}`;

    const otros = Math.max(
        0,
        resumen.totalOrdenes -
        resumen.ordenesPendientes -
        resumen.ordenesCompletadas
    );

    if (graficoTorta) graficoTorta.destroy();

    graficoTorta = new Chart(document.getElementById("graficoTorta"), {
        type: "doughnut",
        data: {
            labels: ["Pendientes", "Completadas", "Otros"],
            datasets: [{
                data: [
                    resumen.ordenesPendientes,
                    resumen.ordenesCompletadas,
                    otros
                ],
                backgroundColor: ["#ffc107", "#198754", "#6c757d"],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: "bottom"
                }
            }
        }
    });

    if (graficoBarras) graficoBarras.destroy();

    graficoBarras = new Chart(document.getElementById("graficoBarras"), {
        type: "bar",
        data: {
            labels: ["Órdenes", "Productos", "Usuarios", "Reembolsos"],
            datasets: [{
                label: "Cantidad",
                data: [
                    resumen.totalOrdenes,
                    resumen.productosPublicados,
                    resumen.totalUsuarios,
                    resumen.totalReembolsos
                ],
                backgroundColor: "#55d6e8",
                borderRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { precision: 0 }
                }
            }
        }
    });
}

// ========================================
// INGRESOS MENSUALES
// ========================================

function inicializarSelectorAnio() {
    const selector = document.getElementById("filtroAnio");
    const anioActual = new Date().getFullYear();

    for (let anio = anioActual; anio >= 2020; anio--) {
        const opcion = document.createElement("option");
        opcion.value = anio;
        opcion.textContent = anio;
        selector.appendChild(opcion);
    }

    selector.value = anioActual;
    selector.addEventListener("change", cargarIngresosMensuales);
}

async function cargarIngresosMensuales() {
    const anio = document.getElementById("filtroAnio").value;
    const cuerpo = document.getElementById("tablaIngresos");

    try {
        mostrarMensaje("mensajeIngresos", "Cargando ingresos...");

        const ingresos = await consultarAPI(
            `${API_BASE}/ingresos-mensuales?anio=${anio}`
        );

        if (!Array.isArray(ingresos) || ingresos.length === 0) {
            cuerpo.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center">
                        No hay ingresos registrados para ${anio}.
                    </td>
                </tr>`;

            if (graficoIngresos) {
                graficoIngresos.destroy();
                graficoIngresos = null;
            }

            mostrarMensaje("mensajeIngresos", "No hay datos para mostrar.");
            return;
        }

        // Agrupamos los importes por moneda para no sumar monedas distintas.
        const monedas = [...new Set(ingresos.map(x => x.codigoMoneda || x.moneda))];

        const datasets = monedas.map((moneda, indice) => {
            const datos = meses.map((_, i) => {
                const registro = ingresos.find(x =>
                    x.mes === i + 1 &&
                    (x.codigoMoneda || x.moneda) === moneda
                );

                return registro ? registro.ingresoNeto : null;
            });

            return {
                label: `Ingreso neto (${moneda})`,
                data: datos,
                borderColor: [
                    "#55d6e8", "#ffc107", "#a78bfa",
                    "#4ade80", "#fb7185"
                ][indice % 5],
                backgroundColor: "transparent",
                tension: 0.3,
                spanGaps: false
            };
        });

        if (graficoIngresos) graficoIngresos.destroy();

        graficoIngresos = new Chart(
            document.getElementById("graficoIngresos"),
            {
                type: "line",
                data: {
                    labels: meses,
                    datasets
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: {
                        intersect: false,
                        mode: "index"
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                callback: valor => formatoNumero(valor)
                            }
                        }
                    }
                }
            }
        );

        cuerpo.innerHTML = ingresos
            .slice()
            .sort((a, b) => a.mes - b.mes)
            .map(x => `
                <tr>
                    <td>${escaparHTML(x.nombreMes)}</td>
                    <td>${escaparHTML(x.codigoMoneda || x.moneda || "-")}</td>
                    <td>${formatoNumero(x.cantidadOrdenes)}</td>
                    <td>${formatoNumero(x.juegosVendidos)}</td>
                    <td>${formatoImporte(x.montoBruto)}</td>
                    <td>${formatoImporte(x.descuentos)}</td>
                    <td class="text-success fw-bold">${formatoImporte(x.ingresoNeto)}</td>
                </tr>
            `).join("");

        mostrarMensaje("mensajeIngresos", "");
    } catch (error) {
        console.error(error);
        mostrarMensaje("mensajeIngresos", "No se pudieron cargar los ingresos mensuales.");
        cuerpo.innerHTML = `
            <tr>
                <td colspan="7" class="text-center">
                    Error al cargar los datos.
                </td>
            </tr>`;
    }
}

// ========================================
// VIDEOJUEGOS MAS VENDIDOS
// ========================================

async function cargarVideojuegosMasVendidos() {
    const limite = document.getElementById("limiteVideojuegos").value;
    const cuerpo = document.getElementById("mensajeVideojuegos");

    try {
        mostrarMensaje("mensajeVideojuegos", "Cargando ranking...");

        const videojuegos = await consultarAPI(
            `${API_BASE}/videojuegos-mas-vendidos?limite=${limite}`
        );

        if (!Array.isArray(videojuegos) || videojuegos.length === 0) {
            if (graficoVideojuegos) {
                graficoVideojuegos.destroy();
                graficoVideojuegos = null;
            }

            mostrarMensaje("mensajeVideojuegos", "Todavía no hay ventas para mostrar.");
            return;
        }

        // Mostramos cantidad de ventas: los ingresos pueden mezclar monedas.
        if (graficoVideojuegos) graficoVideojuegos.destroy();

        graficoVideojuegos = new Chart(
            document.getElementById("graficoVideojuegos"),
            {
                type: "bar",
                data: {
                    labels: videojuegos.map(x => x.titulo),
                    datasets: [{
                        label: "Cantidad de ventas",
                        data: videojuegos.map(x => x.cantidadVentas),
                        backgroundColor: "#55d6e8",
                        borderRadius: 6
                    }]
                },
                options: {
                    indexAxis: "y",
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            beginAtZero: true,
                            ticks: { precision: 0 }
                        }
                    },
                    plugins: {
                        legend: { display: false }
                    }
                }
            }
        );

        mostrarMensaje("mensajeVideojuegos", "");
    } catch (error) {
        console.error(error);
        mostrarMensaje("mensajeVideojuegos", "No se pudo cargar el ranking de videojuegos.");
    }
}

// ========================================
// CLIENTES CON MAYOR GASTO
// ========================================

async function cargarClientesMayorGasto() {
    const limite = document.getElementById("limiteClientes").value;
    const cuerpo = document.getElementById("tablaClientes");

    try {
        mostrarMensaje("mensajeClientes", "Cargando clientes...");

        const clientes = await consultarAPI(
            `${API_BASE}/clientes-mayor-gasto?limite=${limite}`
        );

        if (!Array.isArray(clientes) || clientes.length === 0) {
            cuerpo.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center">
                        Todavía no hay compras para mostrar.
                    </td>
                </tr>`;

            mostrarMensaje("mensajeClientes", "");
            return;
        }

        cuerpo.innerHTML = clientes.map((cliente, indice) => `
            <tr>
                <td>
                    <span class="ranking-number">${indice + 1}</span>
                </td>
                <td>
                    ${escaparHTML(cliente.nombre)} ${escaparHTML(cliente.apellido)}
                </td>
                <td>${escaparHTML(cliente.nombreUsuario)}</td>
                <td>${formatoNumero(cliente.idMoneda)}</td>
                <td>${formatoNumero(cliente.cantidadCompras)}</td>
                <td class="text-success fw-bold">
                    ${formatoImporte(cliente.totalGastado)}
                </td>
                <td>${formatoImporte(cliente.ticketPromedio)}</td>
            </tr>
        `).join("");

        mostrarMensaje(
            "mensajeClientes",
            "Los importes se muestran separados por ID de moneda."
        );
    } catch (error) {
        console.error(error);
        cuerpo.innerHTML = `
            <tr>
                <td colspan="7" class="text-center">
                    Error al cargar los clientes.
                </td>
            </tr>`;

        mostrarMensaje("mensajeClientes", "No se pudo cargar el ranking de clientes.");
    }
}

// ========================================
// INICIALIZACIÓN
// ========================================

async function cargarDashboard() {
    const mensaje = document.getElementById("mensajeDashboard");

    if (!TOKEN) {
        mensaje.textContent = "No se encontró el token. Iniciá sesión nuevamente.";
        return;
    }

    try {
        await cargarResumen();
        mensaje.textContent = "";
    } catch (error) {
        console.error(error);
        mensaje.textContent = "No se pudo cargar el resumen general. Revisá tu sesión.";
    }

    // Los reportes se cargan independientemente del resumen.
    await Promise.allSettled([
        cargarIngresosMensuales(),
        cargarVideojuegosMasVendidos(),
        cargarClientesMayorGasto()
    ]);
}

document.getElementById("limiteVideojuegos")
    .addEventListener("change", cargarVideojuegosMasVendidos);

document.getElementById("limiteClientes")
    .addEventListener("change", cargarClientesMayorGasto);

inicializarSelectorAnio();
cargarDashboard();


// =====================================
// VALIDACIÓN DE SESIÓN
// =====================================

const token = localStorage.getItem("token");

if (!token) {
    window.location.replace("../login/login.html");
}


// =====================================
// OBTENER DATOS DEL USUARIO
// =====================================

function obtenerPayload(token) {

    const partes = token.split(".");

    if (partes.length !== 3) {
        throw new Error("Token inválido");
    }

    // Convertimos Base64URL a Base64
    let base64 = partes[1]
        .replace(/-/g, "+")
        .replace(/_/g, "/");

    base64 += "=".repeat((4 - base64.length % 4) % 4);

    // Decodificación compatible con caracteres especiales
    const bytes = Uint8Array.from(
        atob(base64),
        caracter => caracter.charCodeAt(0)
    );

    const json = new TextDecoder().decode(bytes);

    return JSON.parse(json);
}


// =====================================
// MOSTRAR NOMBRE DEL USUARIO
// =====================================

try {

    const payload = obtenerPayload(token);

    // Verificamos si el token tiene fecha de vencimiento
    if (payload.exp && payload.exp * 1000 <= Date.now()) {
        throw new Error("La sesión ha expirado");
    }

    const nombre =
        payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
        || payload["name"]
        || payload["unique_name"]
        || "Usuario";

    document.getElementById("nombreUsuario").textContent = nombre;

    document.getElementById("tituloUsuario").textContent = nombre;

    // Primera letra para el avatar
    document.getElementById("userAvatar").textContent =
        nombre.charAt(0).toUpperCase();

} catch (error) {

    // console.error("Error al validar la sesión:", error);

    // localStorage.removeItem("token");

    // window.location.replace("../login/login.html");

}


// =====================================
// CERRAR SESIÓN
// =====================================

document.getElementById("btnCerrarSesion")
    .addEventListener("click", () => {

        const confirmar = confirm("¿Deseás cerrar tu sesión?");

        if (!confirmar) return;

        localStorage.removeItem("token");

        window.location.replace("../index.html");

    });


// =====================================
// MENÚ RESPONSIVE
// =====================================

const menuToggle = document.getElementById("menuToggle");
const navLinks = document.getElementById("navLinks");

menuToggle.addEventListener("click", () => {

    const abierto = navLinks.classList.toggle("open");

    menuToggle.setAttribute("aria-expanded", abierto);

    menuToggle.setAttribute(
        "aria-label",
        abierto ? "Cerrar menú" : "Abrir menú"
    );

});


// Cerrar menú al seleccionar una opción
document.querySelectorAll(".nav-link").forEach(link => {

    link.addEventListener("click", () => {

        navLinks.classList.remove("open");

        menuToggle.setAttribute("aria-expanded", "false");

    });

});

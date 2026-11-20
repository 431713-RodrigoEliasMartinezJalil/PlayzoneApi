
const API_URL = "https://localhost:7239/api/Reembolsos";

// Elementos del DOM
const tablaReembolsos = document.getElementById("tablaReembolsos");
const buscarReembolso = document.getElementById("buscarReembolso");
const filtroEstado = document.getElementById("filtroEstado");
const contadorReembolsos = document.getElementById("contadorReembolsos");

const modalElemento = document.getElementById("modalReembolso");
const modalReembolso = new bootstrap.Modal(modalElemento);

// Datos
let reembolsos = [];

// Token
function obtenerToken() {
    return localStorage.getItem("token")
        || localStorage.getItem("accessToken")
        || localStorage.getItem("jwtToken");
}

// Headers
function obtenerHeaders() {
    const token = obtenerToken();

    return {
        "Content-Type": "application/json",
        ...(token ? { "Authorization": `Bearer ${token}` } : {})
    };
}

// Petición HTTP
async function realizarPeticion(url, opciones = {}) {
    const respuesta = await fetch(url, {
        ...opciones,
        headers: {
            ...obtenerHeaders(),
            ...(opciones.headers || {})
        }
    });

    if (!respuesta.ok) {
        let mensaje = `Error HTTP ${respuesta.status}`;

        try {
            const error = await respuesta.json();
            mensaje = error.message || error.title || error.mensaje || mensaje;
        } catch {
            // La respuesta puede no contener JSON.
        }

        throw new Error(mensaje);
    }

    if (respuesta.status === 204) {
        return null;
    }

    return respuesta.json();
}

// Escape HTML
function escaparHTML(valor) {
    return String(valor ?? "").replace(/[&<>"']/g, caracter => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': "&quot;",
        "'": "&#039;"
    })[caracter]);
}

// Formato fecha
function formatearFecha(fecha) {
    if (!fecha) return "-";

    const fechaObjeto = new Date(fecha);

    if (Number.isNaN(fechaObjeto.getTime())) return "-";

    return fechaObjeto.toLocaleDateString("es-AR");
}

// Formato moneda
function formatearMoneda(monto) {
    return Number(monto || 0).toLocaleString("es-AR", {
        style: "currency",
        currency: "ARS"
    });
}

// Clase visual del estado
function obtenerClaseEstado(estado) {
    switch (estado) {
        case "Solicitado":
            return "estado-solicitado";
        case "Pendiente":
            return "estado-pendiente";
        case "Aprobado":
            return "estado-aprobado";
        case "Rechazado":
            return "estado-rechazado";
        default:
            return "";
    }
}

// Actualizar estadísticas
function actualizarEstadisticas() {
    document.getElementById("totalReembolsos").textContent =
        reembolsos.length;

    document.getElementById("totalPendientes").textContent =
        reembolsos.filter(r => r.estadoReembolso === "Pendiente").length;

    document.getElementById("totalAprobados").textContent =
        reembolsos.filter(r => r.estadoReembolso === "Aprobado").length;

    document.getElementById("totalRechazados").textContent =
        reembolsos.filter(r => r.estadoReembolso === "Rechazado").length;
}

// Obtener todos los reembolsos
async function cargarReembolsos() {
    tablaReembolsos.innerHTML = `
        <tr>
            <td colspan="7" class="text-center py-4">
                <div class="spinner-border spinner-border-sm text-light"></div>
                Cargando solicitudes...
            </td>
        </tr>`;

    try {
        const respuesta = await realizarPeticion(API_URL);

        reembolsos = Array.isArray(respuesta)
            ? respuesta
            : respuesta?.$values || [];

        actualizarEstadisticas();
        aplicarFiltros();

    } catch (error) {
        console.error("Error al cargar reembolsos:", error);

        tablaReembolsos.innerHTML = `
            <tr>
                <td colspan="7" class="text-center text-danger py-4">
                    ${escaparHTML(error.message)}
                </td>
            </tr>`;

        contadorReembolsos.textContent = "No se pudieron cargar las solicitudes";
    }
}

// Aplicar filtros
function aplicarFiltros() {
    const busqueda = buscarReembolso.value.trim().toLowerCase();
    const estadoSeleccionado = filtroEstado.value;

    const filtrados = reembolsos.filter(reembolso => {
        const coincideBusqueda =
            String(reembolso.idReembolso).includes(busqueda) ||
            (reembolso.nombreProducto || "").toLowerCase().includes(busqueda);

        const coincideEstado =
            !estadoSeleccionado ||
            reembolso.estadoReembolso === estadoSeleccionado;

        return coincideBusqueda && coincideEstado;
    });

    renderizarTabla(filtrados);
}

// Renderizar tabla
function renderizarTabla(lista) {
    contadorReembolsos.textContent =
        `${lista.length} solicitudes encontradas`;

    if (lista.length === 0) {
        tablaReembolsos.innerHTML = `
            <tr>
                <td colspan="7" class="text-center text-secondary py-4">
                    No se encontraron solicitudes de reembolso.
                </td>
            </tr>`;
        return;
    }

    tablaReembolsos.innerHTML = lista.map(reembolso => `
        <tr>
            <td>#${reembolso.idReembolso}</td>

            <td>
                <span class="product-name">
                    ${escaparHTML(reembolso.nombreProducto)}
                </span>
            </td>

            <td>#${reembolso.idDetalleOrden}</td>

            <td>${formatearFecha(reembolso.fechaSolicitud)}</td>

            <td class="fw-semibold">
                ${formatearMoneda(reembolso.montoReembolsado)}
            </td>

            <td>
                <span class="badge-estado ${obtenerClaseEstado(reembolso.estadoReembolso)}">
                    ${escaparHTML(reembolso.estadoReembolso)}
                </span>
            </td>

            <td class="text-center">
                <button class="btn-ver"
                        onclick="verDetalle(${Number(reembolso.idReembolso)})">
                    <i class="bi bi-eye me-1"></i>Ver
                </button>
            </td>
        </tr>
    `).join("");
}

// Consultar detalle
async function verDetalle(id) {
    try {
        const reembolso = await realizarPeticion(`${API_URL}/${id}`);

        document.getElementById("detalleId").textContent =
            `#${reembolso.idReembolso}`;

        document.getElementById("detalleProducto").textContent =
            reembolso.nombreProducto;

        document.getElementById("detalleOrden").textContent =
            `#${reembolso.idDetalleOrden}`;

        document.getElementById("detalleFecha").textContent =
            formatearFecha(reembolso.fechaSolicitud);

        document.getElementById("detalleMonto").textContent =
            formatearMoneda(reembolso.montoReembolsado);

        document.getElementById("detalleEstado").innerHTML = `
            <span class="badge-estado ${obtenerClaseEstado(reembolso.estadoReembolso)}">
                ${escaparHTML(reembolso.estadoReembolso)}
            </span>`;

        document.getElementById("detalleMotivo").textContent =
            reembolso.motivo || "Sin motivo especificado";

        renderizarAcciones(reembolso);

        modalReembolso.show();

    } catch (error) {
        console.error("Error al consultar reembolso:", error);
        alert(error.message);
    }
}

// Mostrar acciones según estado
function renderizarAcciones(reembolso) {
    const contenedor = document.getElementById("accionesReembolso");

    let botones = "";

    if (reembolso.estadoReembolso === "Solicitado") {
        botones = `
            <button class="btn btn-primary-custom"
                    onclick="actualizarEstado(${reembolso.idReembolso}, 'Pendiente')">
                <i class="bi bi-clock me-1"></i>Pasar a pendiente
            </button>`;
    }

    if (reembolso.estadoReembolso === "Pendiente") {
        botones = `
            <button class="btn btn-danger-custom"
                    onclick="actualizarEstado(${reembolso.idReembolso}, 'Rechazado')">
                <i class="bi bi-x-circle me-1"></i>Rechazar
            </button>

            <button class="btn btn-success-custom"
                    onclick="actualizarEstado(${reembolso.idReembolso}, 'Aprobado')">
                <i class="bi bi-check-circle me-1"></i>Aprobar
            </button>`;
    }

    contenedor.innerHTML = `
        <button type="button" class="btn btn-secondary"
                data-bs-dismiss="modal">
            Cerrar
        </button>
        ${botones}
    `;
}

// Actualizar estado
async function actualizarEstado(id, nuevoEstado) {
    const mensaje = nuevoEstado === "Aprobado"
        ? "¿Estás seguro de que querés aprobar este reembolso?"
        : nuevoEstado === "Rechazado"
            ? "¿Estás seguro de que querés rechazar este reembolso?"
            : "¿Querés pasar esta solicitud a pendiente?";

    if (!confirm(mensaje)) return;

    try {
        await realizarPeticion(`${API_URL}/${id}`, {
            method: "PUT",
            body: JSON.stringify({
                estado: nuevoEstado
            })
        });

        alert(`La solicitud fue actualizada a ${nuevoEstado}.`);

        modalReembolso.hide();

        await cargarReembolsos();

    } catch (error) {
        console.error("Error al actualizar reembolso:", error);
        alert(error.message);
    }
}

// Eventos
buscarReembolso.addEventListener("input", aplicarFiltros);

filtroEstado.addEventListener("change", aplicarFiltros);

document.getElementById("btnLimpiarFiltros")
    .addEventListener("click", () => {
        buscarReembolso.value = "";
        filtroEstado.value = "";
        aplicarFiltros();
    });

document.getElementById("btnActualizar")
    .addEventListener("click", cargarReembolsos);

// Inicialización
document.addEventListener("DOMContentLoaded", cargarReembolsos);

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


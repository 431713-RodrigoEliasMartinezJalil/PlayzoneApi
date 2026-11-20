
const API = "https://localhost:7239/api/OrdenesCompra";

const TOKEN = localStorage.getItem("token");

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN
};

// =========================================
// ELEMENTOS DEL DOM
// =========================================

const tabla = document.getElementById("tabla");
const buscarOrden = document.getElementById("buscarOrden");
const filtroEstado = document.getElementById("filtroEstado");
const fechaDesde = document.getElementById("fechaDesde");
const fechaHasta = document.getElementById("fechaHasta");
const contadorOrdenes = document.getElementById("contadorOrdenes");

const btnActualizar = document.getElementById("btnActualizar");
const btnLimpiarFiltros = document.getElementById("btnLimpiarFiltros");

const modalElemento = document.getElementById("modalOrden");
const modalOrden = new bootstrap.Modal(modalElemento);

let ordenes = [];


// =========================================
// CARGAR ÓRDENES
// =========================================

async function cargarOrdenes() {

    try {

        if (!TOKEN) {
            tabla.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-5">
                        No hay una sesión activa. Iniciá sesión nuevamente.
                    </td>
                </tr>`;
            return;
        }

        tabla.innerHTML = `
            <tr>
                <td colspan="8" class="text-center py-5">
                    Cargando órdenes...
                </td>
            </tr>`;

        const respuesta = await fetch(API, {
            headers: HEADERS
        });

        if (!respuesta.ok) {
            throw new Error(await obtenerError(respuesta));
        }

        ordenes = await respuesta.json();

        if (!Array.isArray(ordenes)) {
            throw new Error("La API no devolvió un listado válido.");
        }

        cargarEstados();
        aplicarFiltros();

    } catch (error) {

        console.error("Error al cargar órdenes:", error);

        tabla.innerHTML = `
            <tr>
                <td colspan="8" class="text-center py-5 text-danger">
                    ${escaparHTML(error.message)}
                </td>
            </tr>`;
    }
}


// =========================================
// CARGAR ESTADOS EN EL SELECT
// =========================================

function cargarEstados() {

    const estadoActual = filtroEstado.value;

    const estados = [
        ...new Set(
            ordenes
                .map(o => o.estadoOrden)
                .filter(Boolean)
        )
    ].sort();

    filtroEstado.innerHTML = `
        <option value="">Todos los estados</option>
    `;

    for (const estado of estados) {

        const option = document.createElement("option");

        option.value = estado;
        option.textContent = estado;

        filtroEstado.appendChild(option);
    }

    filtroEstado.value = estados.includes(estadoActual)
        ? estadoActual
        : "";
}


// =========================================
// APLICAR FILTROS
// =========================================

function aplicarFiltros() {

    const texto = buscarOrden.value.trim().toLowerCase();
    const estado = filtroEstado.value;
    const desde = fechaDesde.value;
    const hasta = fechaHasta.value;

    const filtradas = ordenes.filter(o => {

        const coincideTexto =
            String(o.idOrden).includes(texto) ||
            (o.nombreUsuario || "").toLowerCase().includes(texto) ||
            (o.emailUsuario || "").toLowerCase().includes(texto);

        const coincideEstado =
            !estado || o.estadoOrden === estado;

        const fecha = obtenerFechaLocal(o.fechaOrden);

        const coincideDesde =
            !desde || fecha >= desde;

        const coincideHasta =
            !hasta || fecha <= hasta;

        return coincideTexto &&
               coincideEstado &&
               coincideDesde &&
               coincideHasta;
    });

    renderizarOrdenes(filtradas);
}


// =========================================
// RENDERIZAR TABLA
// =========================================

function renderizarOrdenes(lista) {

    tabla.innerHTML = "";

    contadorOrdenes.textContent =
        `${lista.length} ${lista.length === 1 ? "orden" : "órdenes"}`;

    if (lista.length === 0) {

        tabla.innerHTML = `
            <tr>
                <td colspan="8" class="text-center py-5">
                    No se encontraron órdenes.
                </td>
            </tr>`;

        return;
    }

    for (const o of lista) {

        const fila = document.createElement("tr");

        const cantidadProductos = o.detalles?.length || 0;

        fila.innerHTML = `
            <td>
                <strong>#${escaparHTML(o.idOrden)}</strong>
            </td>

            <td>
                <strong>${escaparHTML(o.nombreUsuario)}</strong>
                <br>
                <small>${escaparHTML(o.emailUsuario)}</small>
            </td>

            <td>${formatearFecha(o.fechaOrden)}</td>

            <td>${escaparHTML(o.metodoPago)}</td>

            <td>${escaparHTML(o.moneda)}</td>

            <td>
                <span class="status-badge ${obtenerClaseEstado(o.estadoOrden)}">
                    ${escaparHTML(o.estadoOrden)}
                </span>
            </td>

            <td>${cantidadProductos}</td>

            <td class="text-end">
                <button class="btn-detail" data-orden-id="${Number(o.idOrden)}">
                    Ver detalle
                </button>
            <button class="btn btn-sm btn-outline-secondary" onclick="editar(${o.idOrden})">Editar</button>
            <button class="btn btn-sm btn-outline-danger" onclick="eliminar(${o.idOrden})">Cancelar</button>
            </td>
        `;

        tabla.appendChild(fila);
    }
}


// =========================================
// ABRIR DETALLE DE UNA ORDEN
// =========================================

async function verDetalle(id) {

    try {

        const respuesta = await fetch(`${API}/${id}`, {
            headers: HEADERS
        });

        if (!respuesta.ok) {
            throw new Error(await obtenerError(respuesta));
        }

        const orden = await respuesta.json();

        mostrarDetalle(orden);

    } catch (error) {

        console.error("Error al consultar la orden:", error);

        alert("No se pudo consultar la orden: " + error.message);
    }
}


// =========================================
// MOSTRAR DETALLE EN EL MODAL
// =========================================

function mostrarDetalle(o) {

    document.getElementById("detalleIdOrden").textContent =
        `#${o.idOrden}`;

    document.getElementById("detalleUsuario").textContent =
        o.nombreUsuario || "-";

    document.getElementById("detalleFecha").textContent =
        formatearFecha(o.fechaOrden);

    document.getElementById("detallePago").textContent =
        o.metodoPago || "-";

    document.getElementById("detalleMoneda").textContent =
        o.moneda || "-";

    document.getElementById("detalleEstado").textContent =
        o.estadoOrden || "-";


    const tablaDetalles = document.getElementById("tablaDetalles");

    tablaDetalles.innerHTML = "";

    let total = 0;

    const detalles = o.detalles || [];

    if (detalles.length === 0) {

        tablaDetalles.innerHTML = `
            <tr>
                <td colspan="4" class="text-center">
                    Esta orden no tiene productos.
                </td>
            </tr>`;
    }

    for (const d of detalles) {

        const precio = Number(d.precioUnitario) || 0;
        const descuento = Number(d.montoDescuento) || 0;

        const precioFinal = Math.max(0, precio - descuento);

        total += precioFinal;

        const fila = document.createElement("tr");

        fila.innerHTML = `
            <td>
                <strong>${escaparHTML(d.tituloProducto)}</strong>
                <br>
                <small class="text-secondary">
                    ID: ${escaparHTML(d.idProducto)}
                </small>
            </td>

            <td>${formatearImporte(precio, o.codigoMoneda)}</td>

            <td>${formatearImporte(descuento, o.codigoMoneda)}</td>

            <td>
                ${d.esRegalo
                    ? '<span class="text-success">Sí 🎁</span>'
                    : "No"}
            </td>
        `;

        tablaDetalles.appendChild(fila);
    }

    document.getElementById("detalleTotal").textContent =
        formatearImporte(total, o.codigoMoneda);

    modalOrden.show();
}


// =========================================
// FILTRAR POR ESTADO
// =========================================

function obtenerClaseEstado(estado) {

    const valor = (estado || "").toLowerCase();

    if (
        valor.includes("complet") ||
        valor.includes("aprob")
    ) {
        return "status-completed";
    }

    if (
        valor.includes("pend") ||
        valor.includes("proces")
    ) {
        return "status-pending";
    }

    if (
        valor.includes("cancel") ||
        valor.includes("rechaz") ||
        valor.includes("reembols")
    ) {
        return "status-cancelled";
    }

    return "status-default";
}


// =========================================
// FORMATEAR FECHAS
// =========================================

function formatearFecha(fecha) {

    if (!fecha) return "-";

    const valor = new Date(fecha);

    if (isNaN(valor.getTime())) return "-";

    return valor.toLocaleString("es-AR");
}


function obtenerFechaLocal(fecha) {

    if (!fecha) return "";

    const valor = new Date(fecha);

    if (isNaN(valor.getTime())) return "";

    const anio = valor.getFullYear();
    const mes = String(valor.getMonth() + 1).padStart(2, "0");
    const dia = String(valor.getDate()).padStart(2, "0");

    return `${anio}-${mes}-${dia}`;
}


// =========================================
// FORMATEAR IMPORTES
// =========================================

function formatearImporte(importe, codigoMoneda) {

    const valor = Number(importe) || 0;

    try {

        if (codigoMoneda) {

            return new Intl.NumberFormat("es-AR", {
                style: "currency",
                currency: codigoMoneda
            }).format(valor);
        }

    } catch (error) {
        console.warn("Código de moneda no reconocido:", codigoMoneda);
    }

    return `${valor.toFixed(2)} ${codigoMoneda || ""}`;
}


// =========================================
// ESCAPAR TEXTO RECIBIDO DE LA API
// =========================================

function escaparHTML(valor) {

    return String(valor ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}


// =========================================
// OBTENER MENSAJES DE ERROR
// =========================================

async function obtenerError(respuesta) {

    const error = await respuesta.json().catch(() => null);

    return error?.mensaje ||
           error?.title ||
           `Error ${respuesta.status}: ${respuesta.statusText}`;
}


// =========================================
// EVENTOS
// =========================================

// Buscar mientras se escribe.

buscarOrden.addEventListener("input", aplicarFiltros);

// Filtros.

filtroEstado.addEventListener("change", aplicarFiltros);
fechaDesde.addEventListener("change", aplicarFiltros);
fechaHasta.addEventListener("change", aplicarFiltros);


// Limpiar filtros.

btnLimpiarFiltros.addEventListener("click", () => {

    buscarOrden.value = "";
    filtroEstado.value = "";
    fechaDesde.value = "";
    fechaHasta.value = "";

    aplicarFiltros();
});


// Actualizar listado.

btnActualizar.addEventListener("click", cargarOrdenes);


// Delegación de eventos para consultar detalles.

tabla.addEventListener("click", (e) => {

    const boton = e.target.closest("[data-orden-id]");

    if (!boton) return;

    const id = Number(boton.dataset.ordenId);

    verDetalle(id);
});


// =========================================
// INICIALIZAR
// =========================================

cargarOrdenes();

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

const modalCrearOrdenElemento = document.getElementById("modalCrearOrden");
const modalCrearOrden = new bootstrap.Modal(modalCrearOrdenElemento);
const formulario = document.getElementById("formulario");
const contenedorDetalles = document.getElementById("detalles");
const btnGenerarOrden = document.getElementById("btnGenerarOrden");

btnGenerarOrden.addEventListener("click", () => {

        modalCrearOrden.show();

    });


function agregarDetalle(d = {}) {
    const fila = document.createElement("div");
    fila.className = "row g-2 mb-2 fila-detalle";
    fila.innerHTML = `
        <div class="col-md-4">
        <label class="form-label aquaText">Producto</label>
            <select id="idProducto" class="form-select" required>
                <option value="1">The Witcher 3: Wild Hunt</option>
                <option value="2">Cyberpunk 2077</option>
                <option value="3">Red Dead Redemption 2</option>
                <option value="4">Grand Theft Auto V</option>
                <option value="5">Elden Ring</option>
                <option value="6">Dark Souls III</option>
                <option value="7">Baldurs Gate 3</option>
                <option value="8">Divinity: Original Sin 2</option>
                <option value="9">The Elder Scrolls V: Skyrim</option>
                <option value="10">Fallout 4</option>
            </select>
        </div>
        <div class="col-md-4">
        <label class="form-label aquaText">Monto descuento</label>
        <input type="number" class="form-control montoDescuento" placeholder="Descuento" min="0" step="0.01" value="${d.montoDescuento ?? 0}">
        </div>
        <div class="col-md-2 d-flex align-items-center">
        <div class="form-check">
            <input type="checkbox" class="form-check-input esRegalo" ${d.esRegalo ? "checked" : ""}>
            <label class="form-check-label aquaText">Regalo</label>
        </div>
        </div>
        <div class="col-md-2">
        <button type="button" class="btn btn-outline-danger w-100 btnQuitar">Quitar</button>
        </div>`;
    fila.querySelector(".btnQuitar").addEventListener("click", () => fila.remove());
    contenedorDetalles.appendChild(fila);
}

document.getElementById("btnAgregar").addEventListener("click", () => agregarDetalle());

formulario.addEventListener("submit", async (e) => {
    e.preventDefault();

    const id = document.getElementById("idOrden").value;
    

    // Leemos todas las filas de productos del formulario
    const detalles = [];
    for (const fila of document.querySelectorAll(".fila-detalle")) {
        detalles.push({
        idProducto: Number(fila.querySelector("#idProducto").value),
        montoDescuento: Number(fila.querySelector(".montoDescuento").value),
        esRegalo: fila.querySelector(".esRegalo").checked,
        });
    }

    if (detalles.length === 0) {
        alert("Agregá al menos un producto a la orden.");
        return;
    }

    const orden = {
        idUsuario: Number(document.getElementById("idUsuario").value),
        idMetodoPago: Number(document.getElementById("idMetodoPago").value),
        idMoneda: Number(document.getElementById("idMoneda").value),
        detalles: detalles,
    };

    let respuesta;

    if (id) {
        // Si hay id, es una edición (PUT)
        respuesta = await fetch(`${API}/${id}`, {
        method: "PUT",
        headers: HEADERS,
        body: JSON.stringify(orden),
        });
    } else {
        // Si no hay id, es una orden nueva (POST)
        respuesta = await fetch(API, {
        method: "POST",
        headers: HEADERS,
        body: JSON.stringify(orden),
        });
    }

    // Si la API rechaza los datos, mostrar los mensajes de error
    if (!respuesta.ok) {
        const error = await respuesta.json().catch(() => null);
        alert(error?.errors ? Object.values(error.errors).flat().join("\n") : "Error " + respuesta.status);
        return;
    }

    limpiarFormulario();
    modalCrearOrden.hide();
    cargarOrdenes();
});


// ---------- EDITAR: pasa los datos de la orden al formulario ----------
function editar(id) {
    modalCrearOrden.show();
    const o = ordenes.find((x) => x.idOrden === id);

    limpiarFormulario();
    document.getElementById("idOrden").value = o.idOrden;
    document.getElementById("idUsuario").value = o.idUsuario;
    // La API no devuelve idMetodoPago ni idMoneda, hay que volver a escribirlos

    for (const d of o.detalles) {
        agregarDetalle(d);
    }
    window.scrollTo(0, 0);
}

async function eliminar(id) {
    if (!confirm("¿Eliminar esta orden?")) return;
    await fetch(`${API}/${id}`, { method: "DELETE", headers: HEADERS });
    cargarOrdenes();
}

function limpiarFormulario() {
    formulario.reset();
    document.getElementById("idOrden").value = "";
    contenedorDetalles.innerHTML = "";
}

btnGenerarOrden.addEventListener("click", () => {
        limpiarFormulario();
        modalCrearOrden.show();

    });


document.getElementById("btnCancelar").addEventListener("click", limpiarFormulario);


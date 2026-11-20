
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

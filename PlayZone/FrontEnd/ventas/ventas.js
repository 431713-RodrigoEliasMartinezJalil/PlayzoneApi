const API = "https://localhost:7239/api/OrdenesCompra";

const TOKEN = localStorage.getItem('token');

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN,
};

const tabla = document.getElementById("tabla");
const formulario = document.getElementById("formulario");
const contenedorDetalles = document.getElementById("detalles");

let ordenes = [];

async function cargarOrdenes() {
    const respuesta = await fetch(API, { headers: HEADERS });
    ordenes = await respuesta.json();

    tabla.innerHTML = "";
    for (const o of ordenes) {
        // Armamos una lista con los productos de la orden
        let productos = "";
        for (const d of o.detalles) {
        productos += `<li>${d.tituloProducto} - ${d.precioUnitario} ${o.codigoMoneda}
            (descuento: ${d.montoDescuento})${d.esRegalo ? " 🎁 Regalo" : ""}</li>`;
        }

        tabla.innerHTML += `
        <tr>
            <td>${o.idOrden}</td>
            <td>${o.nombreUsuario}<br><small>${o.emailUsuario}</small></td>
            <td>${new Date(o.fechaOrden).toLocaleString("es-AR")}</td>
            <td>${o.metodoPago}</td>
            <td>${o.moneda}</td>
            <td>${o.estadoOrden}</td>
            <td><ul class="mb-0 ps-3">${productos}</ul></td>
            <td>
            <button class="btn btn-sm btn-warning" onclick="editar(${o.idOrden})">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="eliminar(${o.idOrden})">Eliminar</button>
            </td>
        </tr>`;
    }
}

function agregarDetalle(d = {}) {
    const fila = document.createElement("div");
    fila.className = "row g-2 mb-2 fila-detalle";
    fila.innerHTML = `
        <div class="col-md-4">
        <label class="form-label aquaText">ID producto</label>
        <input type="number" class="form-control idProducto" placeholder="ID producto" min="1" required value="${d.idProducto ?? ""}">
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
        idProducto: Number(fila.querySelector(".idProducto").value),
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
    cargarOrdenes();
});

// ---------- EDITAR: pasa los datos de la orden al formulario ----------
function editar(id) {
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

document.getElementById("btnCancelar").addEventListener("click", limpiarFormulario);

cargarOrdenes();
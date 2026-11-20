
const API = "https://localhost:7239/api/Producto";

const TOKEN = localStorage.getItem("token");

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN
};

// =========================================
// ELEMENTOS DEL DOM
// =========================================

const tabla = document.getElementById("tabla");
const formulario = document.getElementById("formulario");
let productos = [];

const modalElemento = document.getElementById("modalProducto");
const modalProducto = bootstrap.Modal.getOrCreateInstance(modalElemento);

const contadorProductos = document.getElementById("contadorProductos");

const buscarTitulo = document.getElementById("buscarTitulo");
const filtroTipo = document.getElementById("filtroTipo");
const filtroEstado = document.getElementById("filtroEstado");
const filtroClasificacion = document.getElementById("filtroClasificacion");

let productos = [];


// =========================================
// UTILIDADES
// =========================================

function escaparHTML(valor) {
    return String(valor ?? "").replace(/[&<>"']/g, caracter => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': "&quot;",
        "'": "&#039;"
    })[caracter]);
}


function obtenerFecha(fecha) {
    if (!fecha) return "";

    // Evitamos problemas de zona horaria con fechas YYYY-MM-DD.
    return String(fecha).substring(0, 10);
}


function formatearFecha(fecha) {
    const fechaSimple = obtenerFecha(fecha);

    if (!fechaSimple) return "-";

    const partes = fechaSimple.split("-");

    if (partes.length !== 3) return fechaSimple;

    return `${partes[2]}/${partes[1]}/${partes[0]}`;
}


async function obtenerMensajeError(respuesta) {
    const texto = await respuesta.text();

    if (!texto) return `Error ${respuesta.status}`;

    try {
        const error = JSON.parse(texto);

        if (error.errors) {
            return Object.values(error.errors).flat().join("\n");
        }

        return error.mensaje || error.title || error.error || texto;

    } catch {
        return texto;
    }
}


// =========================================
// CARGAR PRODUCTOS
// =========================================

async function cargarProductos() {

    try {

        tabla.innerHTML = `
            <tr>
                <td colspan="8" class="text-center py-5">
                    Cargando productos...
                </td>
            </tr>
        `;

        const parametros = new URLSearchParams();

        const titulo = buscarTitulo.value.trim();

        if (titulo) {
            parametros.append("titulo", titulo);
        }

        if (filtroTipo.value) {
            parametros.append("idTipoProducto", filtroTipo.value);
        }

        if (filtroEstado.value) {
            parametros.append("idEstadoPublicacion", filtroEstado.value);
        }

        if (filtroClasificacion.value) {
            parametros.append("clasificacionEdad", filtroClasificacion.value);
        }

        const url = parametros.toString()
            ? `${API}?${parametros.toString()}`
            : API;

        const respuesta = await fetch(url, {
            method: "GET",
            headers: HEADERS
        });

        if (!respuesta.ok) {
            throw new Error(await obtenerMensajeError(respuesta));
        }

        productos = await respuesta.json();

        renderizarProductos();

    } catch (error) {

        console.error("Error al cargar productos:", error);

        tabla.innerHTML = `
            <tr>
                <td colspan="8" class="text-center text-danger py-5">
                    No se pudieron cargar los productos.
                    <br>
                    <small>${escaparHTML(error.message)}</small>
                </td>
            </tr>
        `;

        contadorProductos.textContent = "Error";

    }
}


// =========================================
// MOSTRAR PRODUCTOS EN LA TABLA
// =========================================

function renderizarProductos() {

    tabla.innerHTML = "";

    contadorProductos.textContent =
        `${productos.length} ${productos.length === 1 ? "producto" : "productos"}`;

    if (productos.length === 0) {

        tabla.innerHTML = `
            <tr>
                <td colspan="8" class="text-center py-5">
                    No se encontraron productos.
                </td>
            </tr>
        `;

        return;
    }

    for (const p of productos) {

        let claseEstado = "status-published";

        if (p.idEstadoPublicacion === 2) {
            claseEstado = "status-development";
        }

        if (p.idEstadoPublicacion === 3) {
            claseEstado = "status-retired";
        }

        const fila = document.createElement("tr");

        fila.innerHTML = `
            <td>${p.idProducto}</td>

            <td>
                <strong>${escaparHTML(p.titulo)}</strong>
                <div class="small text-secondary">
                    ${escaparHTML(p.descripcion || "")}
                </div>
            </td>

            <td>${escaparHTML(p.tipoProducto)}</td>

            <td>
                <span class="status-badge ${claseEstado}">
                    ${escaparHTML(p.estadoPublicacion)}
                </span>
            </td>

            <td>${escaparHTML(p.clasificacionEdad)}</td>

            <td>${formatearFecha(p.fechaLanzamiento)}</td>

            <td>${p.idProductoPadre ?? "-"}</td>

            <td class="text-end">
                <button
                    class="btn btn-sm btn-edit-product me-1"
                    data-accion="editar"
                    data-id="${p.idProducto}">
                    Editar
                </button>

                <button
                    class="btn btn-sm btn-delete-product"
                    data-accion="retirar"
                    data-id="${p.idProducto}">
                    Retirar
                </button>
            </td>
        `;

        tabla.appendChild(fila);
    }
}


// =========================================
// CREAR / ACTUALIZAR PRODUCTO
// =========================================

formulario.addEventListener("submit", async (e) => {

    e.preventDefault();

    const id = document.getElementById("idProducto").value;

    const producto = {

        titulo: document.getElementById("titulo").value.trim(),

        descripcion: document.getElementById("descripcion").value.trim(),

        idTipoProducto: Number(
            document.getElementById("idTipoProducto").value
        ),

        idEstadoPublicacion: Number(
            document.getElementById("idEstadoPublicacion").value
        ),

        clasificacionEdad:
            document.getElementById("clasificacionEdad").value,

        fechaLanzamiento:
            document.getElementById("fechaLanzamiento").value,

        idProductoPadre:
            document.getElementById("idProductoPadre").value
                ? Number(document.getElementById("idProductoPadre").value)
                : null
    };

    const esEdicion = Boolean(id);

    const url = esEdicion ? `${API}/${id}` : API;

    const metodo = esEdicion ? "PUT" : "POST";

    const botonGuardar = document.getElementById("btnGuardarProducto");

    try {

        botonGuardar.disabled = true;
        botonGuardar.textContent = "Guardando...";

        const respuesta = await fetch(url, {
            method: metodo,
            headers: HEADERS,
            body: JSON.stringify(producto)
        });

        if (!respuesta.ok) {
            throw new Error(await obtenerMensajeError(respuesta));
        }

        alert(
            esEdicion
                ? "Producto actualizado correctamente."
                : "Producto creado correctamente."
        );

        limpiarFormulario();

        modalProducto.hide();

        await cargarProductos();

    } catch (error) {

        console.error("Error al guardar producto:", error);

        alert(error.message);

    } finally {

        botonGuardar.disabled = false;
        botonGuardar.textContent = "Guardar producto";

    }

});


// =========================================
// EDITAR PRODUCTO
// =========================================

function editarProducto(id) {

    const p = productos.find(x => x.idProducto === id);

    if (!p) return;

    document.getElementById("idProducto").value = p.idProducto;

    document.getElementById("titulo").value = p.titulo;

    document.getElementById("descripcion").value = p.descripcion || "";

    document.getElementById("idTipoProducto").value = p.idTipoProducto;

    document.getElementById("idEstadoPublicacion").value =
        p.idEstadoPublicacion;

    document.getElementById("clasificacionEdad").value =
        p.clasificacionEdad;

    document.getElementById("fechaLanzamiento").value =
        obtenerFecha(p.fechaLanzamiento);

    document.getElementById("idProductoPadre").value =
        p.idProductoPadre ?? "";

    document.getElementById("tituloModalProducto").textContent =
        "Editar producto";

    document.getElementById("btnGuardarProducto").textContent =
        "Guardar cambios";

    modalProducto.show();
}


// =========================================
// RETIRAR PRODUCTO
// =========================================

async function retirarProducto(id) {

    const confirmado = confirm(
        "¿Estás seguro de que querés retirar este producto del catálogo?"
    );

    if (!confirmado) return;

    try {

        const respuesta = await fetch(`${API}/${id}`, {
            method: "DELETE",
            headers: HEADERS
        });

        if (!respuesta.ok) {
            throw new Error(await obtenerMensajeError(respuesta));
        }

        alert("Producto retirado correctamente.");

        await cargarProductos();

    } catch (error) {

        console.error("Error al retirar producto:", error);

        alert(error.message);

    }
}


// =========================================
// EVENTOS DE LA TABLA
// =========================================

tabla.addEventListener("click", (e) => {

    const boton = e.target.closest("button[data-accion]");

    if (!boton) return;

    const id = Number(boton.dataset.id);

    if (boton.dataset.accion === "editar") {
        editarProducto(id);
    }

    if (boton.dataset.accion === "retirar") {
        retirarProducto(id);
    }

});


// =========================================
// NUEVO PRODUCTO
// =========================================

document.getElementById("btnNuevoProducto").addEventListener("click", () => {

    limpiarFormulario();

    document.getElementById("tituloModalProducto").textContent =
        "Nuevo producto";

    document.getElementById("btnGuardarProducto").textContent =
        "Guardar producto";

});


// =========================================
// LIMPIAR FORMULARIO
// =========================================

function limpiarFormulario() {

    formulario.reset();

    document.getElementById("idProducto").value = "";

}


// =========================================
// FILTROS
// =========================================

let temporizadorBusqueda;

buscarTitulo.addEventListener("input", () => {

    clearTimeout(temporizadorBusqueda);

    temporizadorBusqueda = setTimeout(() => {
        cargarProductos();
    }, 350);

});


filtroTipo.addEventListener("change", cargarProductos);

filtroEstado.addEventListener("change", cargarProductos);

filtroClasificacion.addEventListener("change", cargarProductos);


document.getElementById("btnLimpiarFiltros").addEventListener("click", () => {

    buscarTitulo.value = "";

    filtroTipo.value = "";

    filtroEstado.value = "";

    filtroClasificacion.value = "";

    cargarProductos();

});


// =========================================
// INICIALIZACIÓN
// =========================================

if (!TOKEN) {

    alert("No se encontró una sesión activa. Iniciá sesión nuevamente.");

} else {

    // El backend excluye siempre los productos retirados.
    const opcionRetirados = filtroEstado.querySelector('option[value="3"]');

    if (opcionRetirados) {
        opcionRetirados.disabled = true;
        opcionRetirados.textContent = "Retirado (no disponible en listado)";
    }

    cargarProductos();

}

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

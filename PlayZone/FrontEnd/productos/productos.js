const API = "https://localhost:7239/api/Producto";

const TOKEN = localStorage.getItem('token');

const HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + TOKEN,
};

const tabla = document.getElementById("tabla");
const formulario = document.getElementById("formulario");
let productos = [];

async function cargarProductos() {
    const respuesta = await fetch(API, { headers: HEADERS });
    productos = await respuesta.json();

    tabla.innerHTML = "";
    for (const p of productos) {
        tabla.innerHTML += `
        <tr>
            <td>${p.idProducto}</td>
            <td>${p.titulo}</td>
            <td>${p.descripcion}</td>
            <td>${p.tipoProducto}</td>
            <td>${p.estadoPublicacion}</td>
            <td>${p.clasificacionEdad}</td>
            <td>${p.fechaLanzamiento}</td>
            <td>${p.idProductoPadre ?? ""}</td>
            <td>
            <button class="btn btn-sm btn-warning" onclick="editar(${p.idProducto})">Editar</button>
            <button class="btn btn-sm btn-danger" onclick="eliminar(${p.idProducto})">Eliminar</button>
            </td>
        </tr>`;
    }
}

formulario.addEventListener("submit", async (e) => {
    e.preventDefault();

    const id = document.getElementById("idProducto").value;

    const producto = {
        titulo: document.getElementById("titulo").value,
        descripcion: document.getElementById("descripcion").value,
        idTipoProducto: Number(document.getElementById("idTipoProducto").value),
        idEstadoPublicacion: Number(document.getElementById("idEstadoPublicacion").value),
        clasificacionEdad: document.getElementById("clasificacionEdad").value,
        fechaLanzamiento: document.getElementById("fechaLanzamiento").value,
        idProductoPadre: document.getElementById("idProductoPadre").value ? Number(document.getElementById("idProductoPadre").value) : null,
    };

    let respuesta;

    if (id) {
        // Si hay id, es una edición (PUT)
        respuesta = await fetch(`${API}/${id}`, {
        method: "PUT",
        headers: HEADERS,
        body: JSON.stringify(producto),
        });
    } else {
        // Si no hay id, es un producto nuevo (POST)
        respuesta = await fetch(API, {
        method: "POST",
        headers: HEADERS,
        body: JSON.stringify(producto),
        });
    }

    if (!respuesta.ok) {
        const error = await respuesta.json().catch(() => null);
        alert(error?.errors ? Object.values(error.errors).flat().join("\n") : "Error " + respuesta.status);
        return;
    }

    limpiarFormulario();
    cargarProductos();
});

function editar(id) {
    const p = productos.find((x) => x.idProducto === id);
    document.getElementById("idProducto").value = p.idProducto;
    document.getElementById("titulo").value = p.titulo;
    document.getElementById("descripcion").value = p.descripcion;
    document.getElementById("idTipoProducto").value = p.idTipoProducto;
    document.getElementById("idEstadoPublicacion").value = p.idEstadoPublicacion;
    document.getElementById("clasificacionEdad").value = p.clasificacionEdad;
    document.getElementById("fechaLanzamiento").value = p.fechaLanzamiento;
    document.getElementById("idProductoPadre").value = p.idProductoPadre ?? "";
}

async function eliminar(id) {
    console.log(id);
    
    if (!confirm("¿Eliminar este producto?")) return;
    await fetch(`${API}/${id}`, { method: "DELETE", headers: HEADERS });
    cargarProductos();
}

function limpiarFormulario() {
    formulario.reset();
    document.getElementById("idProducto").value = "";
}
document.getElementById("btnCancelar").addEventListener("click", limpiarFormulario);

cargarProductos();

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

    console.error("Error al validar la sesión:", error);

    localStorage.removeItem("token");

    window.location.replace("../login/login.html");

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

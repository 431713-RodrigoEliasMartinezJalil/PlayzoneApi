const token = localStorage.getItem("token");

const payload = JSON.parse(
    atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'))
);

const nombre = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

document.getElementById("titulo").innerText = `BIENVENIDO ${nombre}!!!`;
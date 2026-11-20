const API = "https://localhost:7239/api";
const form = document.getElementById("registerForm");
const countrySelect = document.getElementById("countryId");
const errorElement = document.getElementById("error");

async function cargarPaises() {
    try {
        const response = await fetch(`${API}/Pais`);
        if (!response.ok) {
            throw new Error("No se pudieron cargar los países.");
        }

        const paises = await response.json();
        for (const pais of paises) {
            const option = document.createElement("option");
            option.value = pais.idPais;
            option.textContent = pais.nombre;
            countrySelect.appendChild(option);
        }
    } catch (error) {
        errorElement.textContent = error.message;
    }
}

form.addEventListener("submit", async (event) => {
    event.preventDefault();
    errorElement.textContent = "";

    const response = await fetch(`${API}/Auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            username: document.getElementById("username").value.trim(),
            email: document.getElementById("email").value.trim(),
            name: document.getElementById("name").value.trim(),
            lastName: document.getElementById("lastName").value.trim(),
            password: document.getElementById("password").value,
            birthDate: document.getElementById("birthDate").value,
            countryId: Number(countrySelect.value)
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        errorElement.textContent = error?.message ?? "No se pudo completar el registro.";
        return;
    }

    window.location.href = "../login/login.html";
});

cargarPaises();

const form = document.getElementById('loginForm');
const errorEl = document.getElementById('error')

    form.addEventListener('submit', async (e) => {
      e.preventDefault(); // evita que la página se recargue
        errorEl.textContent = '';

        const email = document.getElementById('inputUser').value;
        const password = document.getElementById('inputPassword').value;

        try {
        const response = await fetch('https://localhost:7206/api/Auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ emailOrUsername: email, password })
        });

        if (!response.ok) {
            throw new Error('Credenciales incorrectas');
        }

        const data = await response.text();
        console.log(data);
        
        localStorage.setItem('token', data);
        window.location.href = '../paginaPrincipal/index.html';
        } catch (err) {
        errorEl.textContent = err.message;
        }
    });
const form = document.getElementById('loginForm');
const errorEl = document.getElementById('error');

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    errorEl.textContent = '';

    const email = document.getElementById('inputUser').value;
    const password = document.getElementById('inputPassword').value;

    try {
        const response = await fetch('http://localhost:5020/api/Auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        console.log(response);

        if (!response.ok) {
            throw new Error('Credenciales incorrectas');
        }

        const data = await response.json();
        console.log(data);

        
        localStorage.setItem('token', data.token ?? data);
        window.location.href = '../inicio/inicio.html';
    } catch (err) {
        errorEl.textContent = err.message;
    }
});
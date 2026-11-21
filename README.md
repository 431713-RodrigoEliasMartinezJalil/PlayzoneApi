# 🎮 PlayZone

**PlayZone** es una plataforma orientada al mundo de los videojuegos, diseñada para ofrecer una experiencia sencilla e intuitiva a los usuarios. Su objetivo es reunir en un solo lugar funcionalidades relacionadas con el universo gamer, mediante una interfaz moderna y atractiva.

## 🚀 Tecnologías utilizadas

### Frontend

- HTML5
- CSS3
- JavaScript

### Backend

- C#
- ASP.NET Core 8
- Entity Framework Core
- API REST
- JWT para autenticación

### Base de datos

- Microsoft SQL Server

## ✨ Características

- Interfaz moderna y adaptable.
- Sistema de autenticación de usuarios.
- Comunicación entre frontend y backend mediante una API REST.
- Gestión de datos utilizando Entity Framework Core.
- Diseño enfocado en la experiencia del usuario.

## ⚙️ Instalación y ejecución

### 1. Clonar el repositorio

```bash
git clone URL_DEL_REPOSITORIO
```

### 2. Crear la base de datos

- Abrir SQL Server Management Studio (SSMS).
- Conectarse a la instancia de SQL Server.
- Abrir el archivo `.sql` incluido en el proyecto.
- Ejecutar el script para crear y configurar la base de datos.

### 3. Configurar el backend

Ingresar a la carpeta del proyecto backend y restaurar las dependencias:

```bash
dotnet restore
```

Configurar la cadena de conexión a SQL Server en `appsettings.json`, utilizando los datos de la instancia local.

### 4. Ejecutar el backend

```bash
dotnet run
```

La API estará disponible en la dirección local indicada por ASP.NET Core.

### 5. Ejecutar el frontend

Abrir el archivo `index.html` utilizando una extensión como Live Server.

## 📁 Estructura del proyecto

La estructura general del proyecto se organiza en dos partes principales:

- **Frontend:** contiene la interfaz de usuario, estilos y funcionalidades JavaScript.
- **Backend:** contiene la API REST, lógica de negocio, acceso a datos y autenticación.

## 👨‍💻 Proyecto académico

PlayZone es un proyecto desarrollado con fines académicos, aplicando conocimientos de desarrollo web, programación orientada a objetos, bases de datos y arquitectura cliente-servidor.

---

**PlayZone — Tu universo gamer.** 🎮

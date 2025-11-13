# 🧱 Clean Architecture Base — ASP.NET Core 9

![C#](https://img.shields.io/badge/C%23-68217A?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-6C63FF?style=for-the-badge&logo=dotnet&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-F9C6D4?style=for-the-badge)
![Author](https://img.shields.io/badge/Author-Lorainne_Navarro-F4A7B9?style=for-the-badge)

**CleanArchitectureBase** es una plantilla desarrollada en **.NET 9** con ASP.NET Core Web API, siguiendo los principios de **Arquitectura Limpia (Clean Architecture).**
Está pensada para servir como base sólida y escalable para futuros proyectos backend, asegurando una separación clara de responsabilidades y fácil mantenibilidad.

> 💡 Ideal para construir APIs modulares, testeables y preparadas para producción, aplicando DDD, SOLID y buenas prácticas de desarrollo.

> 🧱 Esta plantilla sirve como punto de partida para crear **APIs empresariales**, **SaaS** o **sistemas internos** con una base sólida, modular y escalable.

## 🗂️ Estructura del Proyecto
La solución está organizada en capas independientes, promoviendo una arquitectura mantenible, escalable y fácil de extender.

```plaintext
📦 Backend (Solución)
├── 📁 MyApp.Application                  # Lógica de negocio y casos de uso
│   ├── 📁 Dependencias                   # Registro de servicios de aplicación
│   ├── 📁 DTOs                           # Objetos de transferencia de datos (Request/Response)
│   ├── 📁 Enums                          # Enumeraciones de la aplicación
│   ├── 📁 Interfaces                     # Contratos (servicios, repositorios)
│   ├── 📁 Mappers                        # Mapeos entre entidades y DTOs
│   ├── 📁 Services                       # Servicios internos de aplicación
│   ├── 📁 UseCases                       # Casos de uso de negocio
│   ├── 📁 Validators                     # Validaciones de entrada (FluentValidation)
│   └── 📄 ApplicationDependencyInjection.cs
│
├── 📁 MyApp.Domain                       # Núcleo del negocio (entidades y lógica pura)
│   ├── 📁 Entities                       # Entidades del dominio
│   ├── 📁 Interfaces                     # Contratos del dominio (repositorios)
│   └── 📁 Dependencias                   # Elementos específicos del dominio
│
├── 📁 MyApp.Infrastructure               # Implementación técnica (EF Core, JWT, correos, etc.)
│   ├── 📁 Context                        # DbContext y configuración de EF Core
│   ├── 📁 Repositories                   # Implementación de interfaces de datos
│   ├── 📁 Security                       # JWT, autenticación y cifrado
│   ├── 📁 Seeders                        # Datos iniciales (semillas)
│   ├── 📁 Services                       # Servicios externos (correo, archivos, etc.)
│   └── 📄 InfrastructureDependencyInjection.cs
│
├── 📁 MyApp.Presentation                 # Capa de presentación (API REST)
│   ├── 📁 Controllers                    # Endpoints HTTP
│   ├── 📁 MiddlewaresAndFilters          # Middlewares personalizados y filtros globales
│   ├── 📁 Dependencias                   # Inyección de dependencias de API
│   ├── 📁 Properties                     # Configuración del proyecto
│   ├── 📄 appsettings.json               # Configuración principal
│   └── 📄 Program.cs                     # Punto de entrada del proyecto
│
├── 📁 MyApp.Shared                       # Recursos comunes entre capas
│   ├── 📁 DTOs                           # DTOs compartidos
│   ├── 📁 Exceptions                     # Excepciones personalizadas
│   ├── 📁 Services                       # Servicios auxiliares
│   └── 📁 TemplateEmails                 # Plantillas HTML para emails
│
└── 📁 MyApp.Tests                        # Proyecto de pruebas unitarias
    ├── 📁 Application                    # Pruebas de casos de uso
    ├── 📁 Infrastructure                 # Pruebas de repositorios
    ├── 📁 Mocks                          # Mocks y fakes
    └── 📁 Dependencias                   # Configuración de pruebas

```

## 🧰 Tecnologías Utilizadas
* **.NET 9**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **AutoMapper**
* **FluentValidation**
* **xUnit**
* **Moq**
* **Swashbuckle (Swagger)**
* **PostgreSQL**

## 🧾 Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de tener instaladas las siguientes herramientas:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/es/)
- [PostgreSQL](https://www.postgresql.org/download/)

> ⚙️ Además, asegúrate de tener instalada la herramienta de línea de comandos de **Entity Framework Core (EF CLI)** para ejecutar migraciones desde la terminal:

```bash
dotnet tool install --global dotnet-ef
```

## 🛠️ Configuración del proyecto

1. **Clona el repositorio**:
```bash
git clone https://github.com/Lorainnenava/CleanArchitectureBase.Net.git
```
2. Abre la solución `MyApp.sln`
3. **Restaura los paquetes NuGet:**
```bash
dotnet restore
```
4. Configurar la Cadena de Conexión

Configura la base de datos según el entorno en el que vayas a trabajar (`Development`, `Staging` o `Production`).
Cada entorno utiliza su propio archivo `appsettings.{Environment}.json.`

> 📁 Ruta: `MyApp.Presentation/appsettings.{Environment}.json`

```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=tu_puerto;Database=tu_db;Username=tu_user;Password=tu_password;"
  }
```
> 💡 **Asegúrate de editar el archivo correspondiente al entorno que estés usando:**
>
> - `appsettings.Development.json` → entorno local  
> - `appsettings.Staging.json` → entorno de pruebas  
> - `appsettings.Production.json` → entorno productivo  

5. Configura las variables necesarias como JwtSettings y EmailSettings:

```json
  "JwtSettings": {
    "SecretKey": "su_clave_secreta",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "tucorreo@gmail.com",
    "SenderPassword": "tu-contraseña-o-app-password"
  }
```

## 🗃️ Migraciones (EF Core — Code First)

Desde la raíz del proyecto abre la terminal y crea la migración inicial:

```bash
dotnet ef migrations add InitialCreate -s Backend/MyApp.Presentation -p Backend/MyApp.Infrastructure
```

Aplicar la migración a la base de datos

```bash
dotnet ef database update -s Backend/MyApp.Presentation -p Backend/MyApp.Infrastructure
```

> ⚠️ Ejecuta estos pasos al iniciar el proyecto por primera vez o cuando cambies el modelo de datos.

## ▶️ Ejecución del Proyecto
> 💡 Por defecto, al ejecutar desde Visual Studio en modo **Debug**, el proyecto usará `appsettings.Development.json`.  
> En modo **Release**, se cargará automáticamente `appsettings.Production.json`.

## 🔧 Opción 1: Desde Visual Studio
    1. Establece MyApp.Presentation como proyecto de inicio
    2. Presiona F5 o haz clic en Iniciar

## 💻 Opción 2: Desde la terminal
    1. Abre la terminal en la raíz de la solución.
    2. dotnet run --project Backend/MyApp.Presentation

## 🚀 Swagger (Documentación de API)
El proyecto incluye Swagger para probar y visualizar los endpoints desde el navegador.

Accede desde el navegador a:

```bash
http://localhost:5229
```

## 🛡️ Autenticación JWT en Swagger

1. Usa `/api/User/create` para registrar un usuario
2. Luego `/api/UserSession/login` para obtener el token JWT
3. Pulsa el botón **Authorize** en Swagger y pega el token
4. Ya puedes probar endpoints protegidos

## 🧪 Pruebas Unitarias

🔹 **Opción 1: Desde Visual Studio**

    1. Abre el proyecto en Visual Studio.
    2. Ve al Explorador de soluciones..
    3. Haz clic derecho sobre el proyecto de pruebas (MyApp.Tests).
    4. Selecciona la opción "Ejecutar pruebas".

🔹 **Opción 2: Desde la Terminal**

    1. Abre una terminal en la raíz del proyecto 
    2. Ejecuta el siguiente comando:
```bash
  dotnet test
```
Las pruebas están organizadas por capa:
- `Application:` Casos de uso y lógica de negocio
- `Infrastructure:` Repositorios, servicios externos
- `Mocks:` Simulación de dependencias

## 🎯 Buenas Prácticas

- Principios **SOLID**
- Nombres de clases en **PascalCase**
- Interfaces con prefijo `I` (ej. IUserRepository`)
- DTOs separados por `Request` y `Response`.
- Casos de uso terminan en UseCase
- Evitar lógica en controladores → delegar a casos de uso
- Todo servicio debe inyectarse por Dependency Injection

## 🤝 Contribuciones
¡Gracias por tu interés en contribuir! ❤️

1. Haz fork del repositorio.
2. Crea tu rama de feature:
```bash
git checkout -b feature/nueva-funcionalidad
```
3. Realiza tus cambios y ejecuta las pruebas
4. Crea un Pull Request hacia `main`

## 🛡️ Recomendaciones
+ Usa nombres claros para las ramas: `feature/...`, `fix/...`, `test/....`
+ Agrega comentarios a código nuevo
+ Asegúrate de que **todas las pruebas pasen** antes de hacer merge
+ Si agregas un servicio o repositorio nuevo, actualiza la documentación de inyección
+ Si agregas funcionalidades nuevas, incluye pruebas si es posible.

## 📄 Licencia
Este proyecto está licenciado bajo **MIT License.**

## ✍️ Autor o Créditos
Creado con ❤️ por [Lorainne Navarro Carrillo](https://github.com/Lorainnenava)


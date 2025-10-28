```markdown
# AleStock

Sistema interno de comunicación y gestión de pedidos entre **Coordinación** y **Bodega**.  
Proyecto desarrollado en **.NET 8**, **Angular 19 (Vite + Tailwind CSS)** y **PostgreSQL**, totalmente orquestado mediante **Docker Compose**.

---

## Descripción general

AleStock centraliza la gestión de inventario, pedidos y movimientos entre la bodega y la coordinación, evitando dependencias con las sucursales.  
Incluye autenticación basada en JWT, validación por roles y migraciones automáticas con datos iniciales (seeders).

---

## Estructura del proyecto

```

ale-stock/
│
├── api/                        # Backend ASP.NET Core 8
│   └── AleStock.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       ├── Helpers/
│       ├── Program.cs
│       ├── AleStock.Api.csproj
│       └── ...
│
├── web/                        # Frontend Angular 19 (Vite + Tailwind)
│   ├── src/app/
│   │   ├── pages/              # Páginas principales (Inventario, Pedidos, etc.)
│   │   ├── services/           # Servicios HTTP con interceptor JWT
│   │   ├── guards/             # AuthGuard
│   │   ├── app.routes.ts       # Rutas standalone
│   │   └── ...
│   ├── tailwind.config.js
│   ├── vite.config.ts
│   └── package.json
│
├── db/                         # Datos persistentes de PostgreSQL
│   └── data/
│
└── docker-compose.yml           # Orquestación de API + DB + Frontend

````

---

## Stack tecnológico

| Capa | Tecnología |
|------|-------------|
| Backend | ASP.NET Core 8 (C#) |
| Base de datos | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Autenticación | JWT Bearer |
| Frontend | Angular 19 (Standalone Components + Vite + Tailwind CSS 4) |
| Infraestructura | Docker Compose |

---

## Ejecución con Docker

### 1. Construir e iniciar todo el entorno

```bash
docker compose build
docker compose up -d
````

Esto levanta tres contenedores:

* ale-postgres → Base de datos PostgreSQL (puerto 5432)
* ale-api → API .NET 8 (puerto 8080)
* ale-web → Frontend Angular (puerto 4200)

---

### 2. Verificar contenedores

```bash
docker compose ps
```

---

### 3. Acceder a la aplicación

| Servicio                | URL                                                            |
| ----------------------- | -------------------------------------------------------------- |
| Frontend Angular        | [http://localhost:4200](http://localhost:4200)                 |
| API REST                | [http://localhost:8080](http://localhost:8080)                 |
| Swagger (Documentación) | [http://localhost:8080/swagger](http://localhost:8080/swagger) |

---

## Autenticación JWT

```json
POST /api/Auth/login
{
  "email": "alejandro@alestock.local",
  "password": "admin123"
}
```

Respuesta:

```json
{
  "token": "<JWT>",
  "nombre": "Alejandro Neira",
  "rol": "Coordinador"
}
```

El token se guarda en localStorage y es inyectado automáticamente por el interceptor:

```
Authorization: Bearer <TOKEN>
```

---

## Datos iniciales (Seeders automáticos)

| Tipo        | Contenido                    |
| ----------- | ---------------------------- |
| Usuarios    | Coordinador y Bodega         |
| Productos   | RayBan, Oakley, Prada        |
| Inventario  | Stock inicial                |
| Pedidos     | Pedido de ejemplo            |
| Movimientos | Entradas y salidas simuladas |

Para reiniciar el entorno y los datos iniciales:

```bash
docker compose down -v
docker compose up -d
```

---

## Comandos útiles

```bash
# Ejecutar migraciones manualmente
dotnet ef migrations add <Nombre>
dotnet ef database update

# Ver logs en tiempo real
docker logs -f ale-api
docker logs -f ale-web

# Conectarse a PostgreSQL
docker exec -it ale-postgres psql -U postgres -d alestock
```

---

## Frontend Angular 19

### Dependencias principales

* Angular 19 (Standalone Components)
* Vite como build tool
* Tailwind CSS 4
* RxJS 7.8
* JWT Interceptor y AuthGuard
* HttpClient withFetch() optimizado para SSR

### Estructura del frontend

| Carpeta     | Descripción                                                     |
| ----------- | --------------------------------------------------------------- |
| pages/      | Vistas principales: Inventario, Movimientos, Pedidos, Productos |
| services/   | Manejo de APIs con headers JWT                                  |
| guards/     | Protección de rutas por rol                                     |
| components/ | Sidebar, Layouts, Formularios dinámicos                         |

---

## Configuración JWT (.NET)

```json
"Jwt": {
  "Key": "EstaClaveJWTDeAleStockEsMuySeguraYDeAlMenos32Caracteres",
  "Issuer": "AleStock",
  "Audience": "AleStockUsers"
}
```

---

## Próximos hitos

| Hito    | Descripción                                      |
| ------- | ------------------------------------------------ |
| Hito 8  | Normalización visual (Tailwind UI unificado)     |
| Hito 9  | Roles dinámicos en frontend (Coordinador/Bodega) |
| Hito 10 | Implementar despliegue QA/PRD automatizado       |
| Hito 11 | Dashboard de métricas (Inventario y Pedidos)     |

---

## Estado actual del desarrollo

* Backend funcional con controladores: Auth, Productos, Inventario, Movimientos, Pedidos
* Base de datos y seeders automáticos
* Frontend Angular 19 unificado con Tailwind
* Autenticación completa con JWT
* Sidebar dinámico por rol
* Próximo: Reportes, métricas y exportaciones

---

## Licencia

Proyecto interno — 2025
Uso restringido al equipo de desarrollo de AleStock.
Desarrollado por el Equipo de Ingeniería.

```
```

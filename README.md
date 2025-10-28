# AleStock

Sistema interno de comunicación y gestión de pedidos entre **Coordinación** y **Bodega**.  
Proyecto desarrollado en **.NET 8**, con base de datos **PostgreSQL**, y listo para ejecutar mediante **Docker Compose**.

---

## 📘 Descripción general

AleStock permite gestionar pedidos, inventario y movimientos internos entre la coordinación y la bodega, sin involucrar las sucursales.  
Incluye autenticación JWT, migraciones automáticas y seeders de datos iniciales.

---

## 🧩 Estructura del proyecto

```
ale-stock/
│
├── api/                     # Backend ASP.NET Core 8
│   └── AleStock.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       ├── Program.cs
│       ├── AleStock.Api.csproj
│       └── ...
│
├── web/                     # (Pendiente) Frontend Angular 18
│
├── db/                      # Datos persistentes de PostgreSQL
│   └── data/
│
└── docker-compose.yml        # Orquestación de contenedores
```

---

## ⚙️ Stack tecnológico

| Capa | Tecnología |
|------|-------------|
| **Backend** | ASP.NET Core 8 (C#) |
| **Base de datos** | PostgreSQL 16 |
| **ORM** | Entity Framework Core 8 |
| **Autenticación** | JWT Bearer |
| **Documentación API** | Swagger / OpenAPI |
| **Infraestructura** | Docker Compose |

---

## 🚀 Ejecución con Docker

### 1. Construir e iniciar el entorno

```bash
docker compose build
docker compose up -d
```

Esto levanta dos contenedores:
- **ale-postgres** → Base de datos PostgreSQL (puerto `5432`)
- **ale-api** → API .NET 8 (puerto `8080`)

La API se conectará automáticamente, aplicará **migraciones** y ejecutará los **seeders iniciales**.

---

### 2. Verificar contenedores activos

```bash
docker compose ps
```

---

### 3. Acceder a Swagger

```
http://localhost:8080/swagger
```

Desde ahí podrás probar:
- **POST /api/Auth/login**
- **GET /api/Pedidos**
- **POST /api/Pedidos**
- etc.

---

## 🔐 Autenticación JWT

Para probar autenticación:

```json
POST /api/Auth/login
{
  "email": "alejandro@alestock.local",
  "password": "admin123"
}
```

El token recibido debe enviarse en los headers:
```
Authorization: Bearer <TOKEN>
```

---

## 🧪 Datos iniciales (Seeder)

Los seeders cargan automáticamente al iniciar por primera vez:

| Tipo | Datos |
|------|-------|
| Usuarios | Coordinador y Bodega |
| Productos | RayBan, Oakley, Prada |
| Inventario | Stock inicial |
| Pedido | Pedido de ejemplo |
| Movimientos | Re-stock y ajuste |

Si deseas reiniciar la base y volver a ejecutar los seeders:

```bash
docker compose down -v
docker compose up -d
```

---

## 🧰 Comandos útiles

```bash
# Ejecutar migraciones manualmente (opcional)
dotnet ef migrations add <Nombre>
dotnet ef database update

# Ver logs de la API
docker logs -f ale-api

# Acceder a la base de datos
docker exec -it ale-postgres psql -U postgres -d alestock
```

---

## 📄 Entorno y configuración

### `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db;Port=5432;Database=alestock;Username=postgres;Password=postgres"
},
"Jwt": {
  "Key": "EstaClaveJWTDeAleStockEsMuySeguraYDeAlMenos32Caracteres",
  "Issuer": "AleStock",
  "Audience": "AleStockUsers"
}
```

---

## 🧱 Próximos hitos

| Hito | Descripción |
|------|--------------|
| Hito 7 | Controlador de inventario y movimientos |
| Hito 8 | Frontend Angular (panel de coordinación y bodega) |
| Hito 9 | Integración con despliegue en QA / PRD |
| Hito 10 | Monitoreo y métricas (Prometheus + Grafana opcional) |

---

## 📜 Licencia

Proyecto interno de desarrollo – 2025.  
Uso restringido al equipo de desarrollo de **AleStock**.
```
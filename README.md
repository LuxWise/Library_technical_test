# 📚 Library API + Log Worker – Prueba Técnica Backend .NET 8

Este repositorio contiene el desarrollo de la Prueba Técnica para Desarrollador Backend Junior de TuBoleta, implementada con .NET 8, Entity Framework Core, MySQL y totalmente dockerizada para su despliegue.

Se desarrollaron dos componentes principales:

1. **API REST – Biblioteca (Library)**
   - Autenticación con JWT y encriptación de contraseñas con BCrypt.
   - Endpoints protegidos y documentación en Swagger con soporte para Bearer Token.
   - Manejo de errores centralizado con middleware personalizado que devuelve siempre un JSON consistente.
   - Migraciones automáticas con EF Core para crear tablas y relaciones.
   - Algoritmo de recomendaciones de libros basado en:
     - Historial de préstamos del usuario.
     - Similitud de libros por autor, categoría y año.
     - Popularidad general.
   - CRUD de libros y categorías con filtros avanzados.
   - Base de datos y API orquestadas con Docker Compose.
   - Despliegue funcional en un servidor remoto.

2. **Log Worker (LogWorker)**
   - Proyecto Worker Service independiente que:
     - Monitorea carpeta `/logs` (modo FileSystemWatcher o polling configurable).
     - Lee y parsea líneas con formato:
       `YYYY-MM-DD HH:mm:ss [LEVEL] Message...`
     - Almacena en MySQL:
       - `log_files` → metadatos del archivo procesado.
       - `log_entries` → registros individuales.
     - Idempotencia por hash de archivo para evitar reprocesos.
   - Endpoints de métricas en la API:
     - `GET /api/metrics/levels` → conteo por nivel (INFO/WARN/ERROR).
     - `GET /api/metrics/timeseries` → series temporales por hora/día.
     - `GET /api/metrics/files` → listado de archivos procesados.

> ⚠️ El único punto no implementado fue el algoritmo de detección de anomalías en tiempo real con ML.

---

## 🚀 Tecnologías utilizadas

- .NET 8
- Entity Framework Core + Pomelo MySQL Provider
- BCrypt.Net para hashing de contraseñas
- JWT Bearer Authentication
- Swagger / Swashbuckle
- Docker & Docker Compose
- MySQL 8
- Serilog para logging de API en archivos
- Middleware personalizado para manejo de errores

---

## 📂 Estructura del proyecto

- `/Library` → API principal (Libros, Categorías, Auth, Métricas)
- `/LogWorker` → Worker Service que procesa logs
- `/Metrics` → Librería compartida con DbContext y entidades de métricas
- `docker-compose.yml`

---

## ⚙️ Configuración y ejecución local

### 1️⃣ Clonar el repositorio
```bash
git clone https://github.com/tuusuario/prueba-tuboleta.git
cd prueba-tuboleta
```

### 2️⃣ Configurar variables de entorno

Por defecto, `docker-compose.yml` ya define:
- DB_HOST=database
- DB_PORT=3306
- DB_NAME=library
- DB_USER=library_admin
- DB_PASSWORD=library_password
- Logs__WatchPath=/logs
- Logs__Pattern=*.log

### 3️⃣ Crear carpeta de logs
```bash
mkdir logs
```

### 4️⃣ Levantar stack con Docker
```bash
docker compose up --build -d
```

Esto levanta:
- database → MySQL
- library → API REST
- log-worker → Procesador de logs

---

## 🔍 Probar la API

Swagger disponible en:
[http://localhost:8080/swagger](http://localhost:8080/swagger)

Ejemplo para probar autenticación:
```http
POST /api/auth/login
{
  "username": "admin",
  "password": "admin123"
}
```

---

## 📜 Probar procesamiento de logs

1. Crear archivo en ./logs:
   ```bash
   cat > logs/test-01.log << 'EOF'
   2024-01-15 10:30:45 [INFO] User login: user123
   2024-01-15 10:31:02 [ERROR] Database connection failed
   2024-01-15 10:31:15 [WARN] High memory usage: 85%
   EOF
   ```
2. Revisar en la API:
   - `GET /api/metrics/levels`
   - `GET /api/metrics/files`

---

## 🌐 Despliegue en servidor

Se desplegó en un servidor Linux con Docker:

1. Clonar repositorio en el servidor.
2. Crear carpeta persistente para logs:
   ```bash
   mkdir -p /opt/prueba-tuboleta/logs
   ```
3. Levantar servicios:
   ```bash
   docker compose up -d --build
   ```
4. Exponer API con Nginx como proxy inverso + SSL.
5. Acceso público a Swagger:
   `https://<tu-dominio>/swagger`

---

## 🌍 Demo en línea

El proyecto está desplegado y disponible públicamente en:

[https://library-api.lux-wise.com/swagger](https://library-api.lux-wise.com/swagger)

Accede a la documentación interactiva de la API y prueba los endpoints directamente desde Swagger UI.

---

## 📌 Notas finales

- API y Worker comparten la misma base de datos y librería de métricas.
- El volumen ./logs está montado en ambos, permitiendo que los logs generados por la API sean procesados automáticamente.
- Falta por implementar el detector de anomalías del punto 16 de la prueba.

---

## 📈 Diagrama de arquitectura (sugerido)

```ascii
+----------------+         +----------------+         +----------------+
|                |  logs   |                |  DB     |                |
|    Library     +-------->+   LogWorker    +-------->+    MySQL DB    |
|     API        |         | (procesa logs) |         |                |
+----------------+         +----------------+         +----------------+
        |                        ^
        |                        |
        +------------------------+
           (comparten volumen logs)
```

---


# Etapa base
FROM node:22-alpine

WORKDIR /app

# Copiamos package.json y package-lock.json (si existe)
COPY package*.json ./

# Instalamos dependencias
RUN npm install -g @angular/cli@19 && npm install

# Copiamos el resto del código fuente
COPY . .

# Exponemos el puerto estándar de Angular
EXPOSE 4200

# Comando por defecto: ejecutar en modo desarrollo (live reload)
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "4200"]

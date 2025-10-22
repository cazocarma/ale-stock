# Etapa 1: Construcción de Angular
FROM node:22-alpine AS build

WORKDIR /app

# Copiar package.json y package-lock.json (o pnpm-lock.yaml)
COPY package*.json ./

# Instalar Angular CLI y dependencias
RUN npm install -g @angular/cli@19 && npm install

# Copiar el resto del código fuente
COPY . .

# Construir aplicación Angular
RUN ng build --configuration production

# Etapa 2: Servir con Nginx
FROM nginx:stable-alpine

# Copiar los artefactos compilados
COPY --from=build /app/dist/web/browser /usr/share/nginx/html

# Copiar configuración personalizada de Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

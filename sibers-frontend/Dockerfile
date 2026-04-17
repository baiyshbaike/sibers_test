# Build stage / Этап сборки
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files first for layer caching / Копируем package.json для кэширования слоёв
COPY package*.json ./
RUN npm ci

# Copy source and build / Копируем исходники и собираем
COPY . .
RUN npm run build

# Production stage with nginx / Продакшн с nginx
FROM nginx:alpine AS final

# Copy built files to nginx / Копируем собранные файлы в nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Custom nginx config for Vue Router / Nginx конфиг для Vue Router
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
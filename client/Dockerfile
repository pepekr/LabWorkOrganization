# Етап 1: Збірка
FROM node:20 AS build
WORKDIR /app

COPY client/package*.json ./

RUN npm install

COPY client/ .

RUN npm run build

# Етап 2: Виконання
FROM node:20-slim
WORKDIR /app
COPY --from=build /app/dist/client/ ./dist/client/
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/package.json .

# Ваш server.ts використовує PORT env або 4000
# Ми виставимо 4000
ENV PORT=4000
EXPOSE 4000

# Запускаємо SSR сервер
CMD [ "node", "dist/client/server/server.mjs" ]

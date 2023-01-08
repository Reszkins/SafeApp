FROM node:16.15.1 as node

COPY /ClientApp /app

WORKDIR /app
COPY . .
RUN npm install
RUN npm run build --dev
RUN export NODE_OPTIONS=--openssl-legacy-provider

FROM nginx:latest
COPY --from=node /app/dist/ClientApp /usr/share/nginx/html
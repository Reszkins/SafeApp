FROM node:16.15.1 as node

COPY /ClientApp /app

WORKDIR /app
COPY . .
RUN npm install
RUN npm run build --dev
RUN export NODE_OPTIONS=--openssl-legacy-provider

FROM nginx:latest
COPY /ClientApp/nginx.conf /etc/nginx/conf.d/default.conf
COPY /ClientApp/certs /etc/nginx/certs
COPY --from=node /app/dist/ClientApp /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]
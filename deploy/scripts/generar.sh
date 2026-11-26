#!/usr/bin/env bash
# Genera ocelot.json y el sitio de Nginx a partir de deploy/.env
set -euo pipefail
cd "$(dirname "$0")/.."
[ -f .env ] || { echo "Falta deploy/.env (copia .env.example)"; exit 1; }
set -a; . ./.env; set +a
[ ${#JWT_KEY} -ge 32 ] || { echo "JWT_KEY debe tener al menos 32 caracteres"; exit 1; }
[ ${#DB_PASSWORD} -ge 8 ] || { echo "DB_PASSWORD debe tener al menos 8 caracteres"; exit 1; }
mkdir -p generado
sed "s/__DOMINIO__/$DOMINIO/g" ocelot/ocelot.plantilla.json > generado/ocelot.json
sed -e "s/__DOMINIO__/$DOMINIO/g" \
    -e "s/__GATEWAY_PORT__/${GATEWAY_PORT:-5000}/g" \
    -e "s/__API_PORT__/${API_PORT:-5055}/g" \
    -e "s/__VACUNAS_PORT__/${VACUNAS_PORT:-5056}/g" \
    nginx/nginx.plantilla.conf > generado/veterinaria.conf
chmod 644 generado/ocelot.json
echo "Generado: deploy/generado/ocelot.json y deploy/generado/veterinaria.conf"

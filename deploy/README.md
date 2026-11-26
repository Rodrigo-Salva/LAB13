# Despliegue en VPS (Docker + Nginx del servidor)

Subdominios: `vet-gateway`, `vet-api` y `vet-vacunas` (registros DNS tipo A hacia la IP del VPS).
Sin dominio propio sirve `DOMINIO=<IP-con-guiones>.sslip.io`.

```bash
git clone https://github.com/Rodrigo-Salva/LAB13.git && cd LAB13/deploy
cp .env.example .env && chmod 600 .env && nano .env     # dominio, clave JWT, clave de la base
bash scripts/generar.sh
docker compose up -d --build
docker compose ps

# Nginx: se agrega un sitio nuevo, sin tocar los existentes
sudo cp generado/veterinaria.conf /etc/nginx/sites-available/veterinaria.conf
sudo ln -s /etc/nginx/sites-available/veterinaria.conf /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx

# HTTPS con Let's Encrypt
source .env
sudo certbot --nginx -d vet-gateway.$DOMINIO -d vet-api.$DOMINIO -d vet-vacunas.$DOMINIO
```

Actualizar: `git pull && bash scripts/generar.sh && docker compose up -d --build`.
Errores: `docker compose logs -f gateway` (o `veterinaria-api`, `servicio-vacunas`, `sqlserver`).
Prueba del 502: `docker compose stop servicio-vacunas` y luego `docker compose start servicio-vacunas`.

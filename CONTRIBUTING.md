# Contributing to MyCrypto

1. Fork el repositorio
2. Crea una rama (`feature/nueva-funcionalidad`)
3. Haz commit de tus cambios
4. Abre un Pull Request

## Releases

- La publicacion del paquete esta preparada mediante GitHub Actions en `.github/workflows/nuget-publish.yml`.
- Configura el secret `NUGET_API_KEY` en GitHub antes de publicar.
- Para disparar una publicacion, crea y sube un tag con formato `vX.Y.Z`.

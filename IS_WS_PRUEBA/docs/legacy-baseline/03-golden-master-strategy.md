# 03 Golden Master Strategy

## Salidas a congelar

Se congelan snapshots JSON de comportamiento observable para:

- Parser:
  - deteccion de nombres duplicados en `listaCampos`.
  - parseo de fecha con formato ISO UTC (`yyyy-MM-ddTHH:mm:ssZ`).
- Servicio CRUD:
  - flujo `Crear -> Consultar`.
  - `Actualizar` sin campos de update.
  - `Eliminar` ejecutado dos veces sobre el mismo id.

Ruta de snapshots:

- `tests/GoldenMaster/CampoParser/*.approved.json`
- `tests/GoldenMaster/PruebaCrudService/*.approved.json`

## Formato baseline

- JSON con sangria (`WriteIndented`) para facilitar diff en PR.
- Propiedades estabilizadas: `codigo`, `mensaje`, `listaCamposSalida`.
- Orden de campos conservado exactamente como lo produce el sistema actual.

## Normalizaciones aplicadas

Solo se normaliza:

- Campo `fecha_actualizacion` -> `"<UTC_TIMESTAMP>"`.

No se normalizan:

- `id` (se vuelve determinista reiniciando estado in-memory).
- `codigo`, `mensaje`, nombres/tipos de campo.
- Orden del payload.

## Riesgos de no determinismo

- `FechaActualizacion` se deriva de `DateTime.UtcNow`.
- Repositorio in-memory mantiene estado estatico global.

Mitigacion aplicada:

- Reinicio del estado del repositorio por reflection en setup de pruebas.
- Token fijo para timestamp volatil.

## Criterio de aprobacion

Un escenario queda en `PASS` si el snapshot actual coincide 1:1 con su archivo `.approved.json` luego de la normalizacion minima indicada.


# 01 Contract Discovery

## Servicio/clase objetivo
- Servicio ASMX: `IS_WS_PRUEBA`
- Archivo: `IS_WS_PRUEBA.asmx.cs`
- Clase de negocio asociada: `PruebaCrudService`

## Firma de metodos similares (evidencia)
- Todos los WebMethods existentes usan:
  - entrada: `SoapRequestDto request`
  - salida: `SoapResponseDto`
- Operaciones existentes: `Crear`, `Consultar`, `Actualizar`, `Eliminar`.
- El servicio mantiene `ExecuteSafely(...)` para devolver codigo tecnico `900` ante excepciones no controladas.

## Patron real de request
- Request wrapper unico: `SoapRequestDto`.
- Datos de entrada por `listaCampos` (`List<SoapCampoDto>`).
- Cada campo usa forma `{ name, value, type }`.
- Convencion de nombres observada: campos en `snake_case` para negocio (`fecha_fundacion`, etc.).

## Patron real de response
- Response wrapper unico: `SoapResponseDto`.
- Estructura:
  - `codigo` (`000`, `001`, `900`)
  - `mensaje`
  - `listaCamposSalida` (lista opcional de `SoapCampoDto`)

## Patron real de errores
- Error funcional esperado: `codigo=001`.
- Error tecnico esperado: `codigo=900`.
- Excepciones se capturan y transforman a respuesta tecnica.

## Decision final de adaptacion
- Se implementa el feature como:
  - `public SoapResponseDto CalcularOfertaCrediticiaLegacy(SoapRequestDto request)`
- Se expone en ASMX con `[WebMethod]` y mismo pipeline `ExecuteSafely`.
- Se conserva contrato SOAP actual (sin request/response nuevos, sin cambio de namespace, sin cambio de serializacion).
- Se aceptan nombres de campo en estilo del repo y alias camelCase para compatibilidad con el ejemplo funcional entregado.

# 04 Test Cases

## `CampoParser.TryBuildFieldMap`

- Caso nominal:
  - Entrada: `listaCampos` con nombres unicos.
  - Expectativa observable: `ok=true`, mapa generado.
- Caso null/vacio:
  - Entrada: request null o `listaCampos` vacia.
  - Expectativa observable: `ok=false`, mensaje de error funcional.
- Caso borde:
  - Entrada: nombres con espacios laterales.
  - Expectativa observable: nombre normalizado por `Trim()`.
- Caso invalido:
  - Entrada: nombre duplicado.
  - Expectativa observable: `ok=false`, `"Duplicate campo name: <name>."`.
- Caso bug/comportamiento legacy:
  - Duplicado se detecta con comparacion case-insensitive.

## `CampoParser.TryGetRequiredDate`

- Caso nominal:
  - Entrada: `yyyy-MM-dd`.
  - Expectativa observable: parseo correcto.
- Caso borde:
  - Entrada: `yyyy-MM-ddTHH:mm:ssZ`.
  - Expectativa observable: parseo correcto (comportamiento actual).
- Caso invalido:
  - Entrada: fecha no parseable.
  - Expectativa observable: mensaje `"Campo <name> must use format yyyy-MM-dd."`.
- Caso bug/comportamiento legacy:
  - El mensaje de error sugiere solo `yyyy-MM-dd`, pero el parser acepta formatos con hora.

## `PruebaCrudService.Crear`

- Caso nominal:
  - Entrada: `nombre`, `descripcion`, `fecha_fundacion` validos.
  - Expectativa observable: `Codigo=000`, mensaje `"Registro creado."`, salida con `id`.
- Caso invalido:
  - Falta de campo requerido.
  - Expectativa observable: `Codigo=001` + mensaje de validacion de parser.
- Caso bug/comportamiento legacy:
  - Errores inesperados se transforman a `Codigo=900` por catch global.

## `PruebaCrudService.Consultar`

- Caso nominal:
  - Entrada: id existente y activo.
  - Expectativa observable: `Codigo=000`, payload completo con `fecha_actualizacion`.
- Caso invalido:
  - id inexistente.
  - Expectativa observable: `Codigo=001`, `"Registro no encontrado."`.
- Caso borde:
  - id existente pero inactivo.
  - Expectativa observable: `Codigo=001`, `"Registro no encontrado."`.

## `PruebaCrudService.Actualizar`

- Caso nominal:
  - Entrada: id existente + al menos un campo opcional valido.
  - Expectativa observable: `Codigo=000`, mensaje `"Registro actualizado."`.
- Caso invalido:
  - id inexistente.
  - Expectativa observable: `Codigo=001`, `"Registro no encontrado."`.
- Caso borde:
  - solo id (sin campos a actualizar).
  - Expectativa observable: `Codigo=001`, `"No update fields provided."`.

## `PruebaCrudService.Eliminar`

- Caso nominal:
  - Entrada: id existente activo.
  - Expectativa observable: `Codigo=000`, `"Registro eliminado."`.
- Caso invalido:
  - id inexistente.
  - Expectativa observable: `Codigo=001`, `"Registro no encontrado."`.
- Caso bug/comportamiento legacy:
  - doble eliminacion del mismo id devuelve `"Registro ya eliminado."`.

## Cobertura implementada en esta ejecucion

- `CampoParser`:
  - duplicados de nombre (`GoldenMaster`).
  - parseo de fecha ISO UTC (`GoldenMaster`).
- `PruebaCrudService`:
  - `Crear -> Consultar` (`GoldenMaster`).
  - `Actualizar` sin campos (`GoldenMaster`).
  - `Eliminar` doble (`GoldenMaster`).


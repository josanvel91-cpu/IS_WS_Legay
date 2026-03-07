# 02 Characterization Strategy

## Alcance

Se construye baseline de comportamiento observable sobre:

- `PruebaCrudService` (CRUD funcional).
- `CampoParser` (normalizacion/validaciones de `listaCampos`).
- `InMemoryPruebaRepository` (persistencia in-memory y efectos de estado).
- Contratos DTO involucrados en la forma de salida (`SoapResponseDto`, `SoapCampoDto`).

No se modifica codigo productivo.

## Enfoque

1. Probar logica de negocio y parseo directamente en pruebas automatizadas.
2. Congelar salidas representativas mediante snapshots aprobados (`.approved.json`).
3. Aplicar normalizacion minima unicamente al campo volatil `fecha_actualizacion`.
4. Reiniciar estado global del repositorio in-memory entre pruebas para mantener reproducibilidad.

## Criterios de seleccion

- Prioridad alta a puntos publicos (`[WebMethod]`) y metodos de negocio invocados por ellos.
- Prioridad alta a parseo de campos por su impacto transversal.
- Cobertura de casos nominales, invalidos y de comportamiento legacy observable.

## Limites actuales

- No se levanta host ASMX en IIS/IIS Express dentro de esta ejecucion.
- Los WebMethods se cubren de forma indirecta a traves de `PruebaCrudService`.
- No se valida serializacion XML runtime (`?WSDL` y envelopes) en esta fase.

## Preservacion de comportamiento legacy

Se preservan explicitamente:

- Codigos de respuesta funcional/tecnica (`000`, `001`, `900`).
- Mensajes actuales (`Registro creado.`, `Registro no encontrado.`, `Registro ya eliminado.`, etc.).
- Forma y orden del payload de `ListaCamposSalida`.
- Comportamientos discutibles pero observados (ejemplo: `TryGetRequiredDate` acepta formatos con hora, aunque el mensaje de error menciona solo `yyyy-MM-dd`).

## Lo que no se intenta resolver en esta fase

- Refactor de arquitectura o mejoras de diseno.
- Correccion de bugs funcionales potenciales.
- Cambio de contratos SOAP/XML.
- Endurecimiento de validaciones fuera del comportamiento actual.


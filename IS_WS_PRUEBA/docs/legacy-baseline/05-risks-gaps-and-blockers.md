# 05 Risks, Gaps And Blockers

## Riesgos tecnicos

- Estado global compartido:
  - `InMemoryPruebaRepository` usa campos estaticos (`Records`, `_nextId`).
  - Riesgo de interferencia entre pruebas/ejecuciones concurrentes.
- Campo volatil:
  - `FechaActualizacion` usa `DateTime.UtcNow`.
  - Riesgo de snapshots inestables sin normalizacion.
- Manejo de errores amplio:
  - Captura generica de excepciones y retorno tecnico `900`.
  - Puede ocultar causa raiz de fallas.
- Contrato legado sensible:
  - Mensajes y codigos son parte del comportamiento observable consumido.

## Brechas actuales

- No se ejecutaron pruebas E2E contra host ASMX (`IS_WS_PRUEBA.asmx`) en IIS/IIS Express.
- No se validaron envelopes SOAP runtime ni `?WSDL` en esta corrida.
- No existe validacion de regresion XML canonica (XSD/schema contract test).

## Bloqueadores potenciales para siguientes fases

- Si el siguiente agente cambia mensajes o codigos, puede romper consumidores legacy.
- Si no se respeta el orden de `ListaCamposSalida`, habra regresiones de contrato.
- Cambios en parseo de fecha pueden alterar aceptacion actual de payloads.
- Migraciones a repositorio no estatico sin baseline pueden introducir falsos negativos/positivos.

## Necesidad de seam/aislamiento

- Para pruebas confiables se requiere reset controlado del estado in-memory.
- Para pruebas E2E ASMX se requiere entorno IIS/IIS Express habilitado.
- Para auditoria de contrato SOAP se recomienda agregar pipeline de snapshot XML de respuestas reales.


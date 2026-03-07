# 06 Execution Summary

## Que se encontro

- Servicio legacy ASMX .NET Framework 4.0 con 4 WebMethods publicos:
  - `Crear`, `Consultar`, `Actualizar`, `Eliminar`.
- Logica de negocio concentrada en `PruebaCrudService`.
- Parseo transversal de entrada en `CampoParser`.
- Persistencia in-memory con estado estatico global y timestamps UTC.

## Que se genero

- Baseline documental en `docs/legacy-baseline/01-06`.
- Proyecto de pruebas de caracterizacion:
  - `tests/IS_WS_PRUEBA.CharacterizationTests/`
- Snapshots aprobados (`GoldenMaster`):
  - `tests/GoldenMaster/CampoParser/*.approved.json`
  - `tests/GoldenMaster/PruebaCrudService/*.approved.json`

## Cobertura conseguida

- Casos parser:
  - nombre duplicado.
  - parseo de fecha ISO UTC.
- Casos CRUD:
  - flujo `Crear -> Consultar`.
  - `Actualizar` sin campos.
  - `Eliminar` doble.

## Pendientes

- Pruebas runtime ASMX (IIS/IIS Express) para cubrir host `IS_WS_PRUEBA.asmx`.
- Validacion de contrato SOAP/WSDL en ejecucion real.
- Ampliar baseline a escenarios tecnicos `Codigo=900` inducidos.

## Siguiente paso recomendado antes de implementar feature

1. Ejecutar baseline (`dotnet test`) y fijar resultado como referencia de PR.
2. Implementar cambios funcionales en rama separada.
3. Re-ejecutar baseline; cualquier diff en snapshots debe justificarse explicitamente.


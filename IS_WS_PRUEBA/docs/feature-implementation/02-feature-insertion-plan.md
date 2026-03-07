# 02 Feature Insertion Plan

## Metodo nuevo a crear
- `CalcularOfertaCrediticiaLegacy`

## Archivo destino
- Exposicion SOAP: `IS_WS_PRUEBA.asmx.cs`
- Implementacion de negocio: `Domain/Services/PruebaCrudService.cs`

## Firma final
- WebMethod:
  - `public SoapResponseDto CalcularOfertaCrediticiaLegacy(SoapRequestDto request)`
- Servicio:
  - `public SoapResponseDto CalcularOfertaCrediticiaLegacy(SoapRequestDto request)`

## Retorno final
- `SoapResponseDto` con contratos vigentes:
  - `000` para aprobado/aprobado manual.
  - `001` para rechazo funcional.
  - `900` para error tecnico.
- Salida por `listaCamposSalida` (cuota, capacidad, detalle segun rama).

## Dependencias a reutilizar
- `CampoParser.TryBuildFieldMap(...)` para validar envelope `listaCampos`.
- Helpers internos ya existentes de `PruebaCrudService`:
  - `Success(...)`
  - `FunctionalError(...)`
  - `Field(...)`
- Wrapper de seguridad del ASMX:
  - `ExecuteSafely(...)`

## Impacto previsto
- Sin cambios en metodos CRUD existentes.
- Sin cambios de namespace SOAP ni DTOs.
- Sin refactor estructural global.
- Nuevas pruebas de characterization/golden master para el feature.

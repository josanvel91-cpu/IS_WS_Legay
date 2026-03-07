# 03 Implementation Summary

## Archivos modificados
- `IS_WS_PRUEBA/IS_WS_PRUEBA.asmx.cs`
- `IS_WS_PRUEBA/Domain/Services/PruebaCrudService.cs`
- `IS_WS_PRUEBA/tests/IS_WS_PRUEBA.CharacterizationTests/CharacterizationAndGoldenMasterTests.cs`
- `IS_WS_PRUEBA/tests/GoldenMaster/PruebaCrudService/calcular-oferta-crediticia-aprobado.approved.json`
- `IS_WS_PRUEBA/tests/GoldenMaster/PruebaCrudService/calcular-oferta-crediticia-aprobado-manual.approved.json`

## Metodo implementado
- `CalcularOfertaCrediticiaLegacy(SoapRequestDto request)` en `PruebaCrudService`.
- WebMethod expuesto con mismo nombre en `IS_WS_PRUEBA.asmx.cs`.

## Comportamiento agregado
- Recibe campos legacy de oferta crediticia por `listaCampos`.
- Calcula `cuota` y `capacidad` con reglas deliberadamente legacy.
- Devuelve:
  - `000|APROBADO` cuando cuota <= capacidad.
  - `001|RECHAZADO` cuando cuota > capacidad.
  - `000|APROBADO_MANUAL` si ingreso/deuda negativos.
  - `900|ERROR` con detalle tecnico al fallar conversiones/calculo.

## Que se respeto del repo
- Contrato de entrada/salida existente (`SoapRequestDto`/`SoapResponseDto`).
- Convencion de codigos funcionales/tecnicos (`000/001/900`).
- Estructura SOAP ASMX sin DTOs nuevos ni cambios de namespace.
- Insercion local sin afectar CRUD existente.

## Problemas intencionales conservados para fases posteriores
- Parseo numerico dependiente de cultura (`Convert.ToDouble`/`CurrentCulture`).
- Duplicacion de regla de capacidad y recalculo de cuota.
- Regla arbitraria por ultimo digito de identificacion.
- Exposicion de detalle interno (usuario/id/server/fecha).
- Mensaje tecnico con `exception.ToString()`.

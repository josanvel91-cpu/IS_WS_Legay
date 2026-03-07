# 04 Baseline Impact

## Pruebas existentes no afectadas
- Se mantuvieron en PASS los 5 escenarios baseline previos:
  - `CampoParser_DuplicateCampoName_MatchesGoldenMaster`
  - `CampoParser_RequiredDate_AcceptsZuluFormat_MatchesGoldenMaster`
  - `PruebaCrudService_CrearConsultar_MatchesGoldenMaster`
  - `PruebaCrudService_ActualizarSinCampos_MatchesGoldenMaster`
  - `PruebaCrudService_EliminarDosVeces_MatchesGoldenMaster`

## Golden masters sin cambio
- `tests/GoldenMaster/CampoParser/*.approved.json`
- `tests/GoldenMaster/PruebaCrudService/crear-consultar.approved.json`
- `tests/GoldenMaster/PruebaCrudService/actualizar-sin-campos.approved.json`
- `tests/GoldenMaster/PruebaCrudService/eliminar-doble.approved.json`

## Nuevas pruebas agregadas
- `PruebaCrudService_CalcularOfertaCrediticiaLegacy_Aprobado_MatchesGoldenMaster`
- `PruebaCrudService_CalcularOfertaCrediticiaLegacy_AprobadoManual_MatchesGoldenMaster`

## Nuevos snapshots agregados
- `tests/GoldenMaster/PruebaCrudService/calcular-oferta-crediticia-aprobado.approved.json`
- `tests/GoldenMaster/PruebaCrudService/calcular-oferta-crediticia-aprobado-manual.approved.json`

## Cambios intencionales de snapshot
- No hubo modificaciones en snapshots existentes.
- Solo se agregaron snapshots nuevos para el feature.

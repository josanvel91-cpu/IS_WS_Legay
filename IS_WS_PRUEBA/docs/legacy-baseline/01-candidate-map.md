# 01 Candidate Map

Proyecto analizado: `IS_WS_PRUEBA`  
Ruta: `NET/GIT/IS_WS_Legay/IS_WS_PRUEBA`

## Candidatos priorizados

| Metodo/Clase | Archivo | Tipo | Prioridad | Motivo de seleccion | Baseline sugerida | Riesgo principal |
| --- | --- | --- | --- | --- | --- | --- |
| `IS_WS_PRUEBA.Crear` | `IS_WS_PRUEBA.asmx.cs` | WebMethod publico | Alta | Punto de entrada SOAP expuesto | Manual baseline + cobertura indirecta via `PruebaCrudService.Crear` | Regresion de contrato SOAP |
| `IS_WS_PRUEBA.Consultar` | `IS_WS_PRUEBA.asmx.cs` | WebMethod publico | Alta | Punto de entrada SOAP expuesto | Manual baseline + cobertura indirecta via `PruebaCrudService.Consultar` | Regresion en payload de salida |
| `IS_WS_PRUEBA.Actualizar` | `IS_WS_PRUEBA.asmx.cs` | WebMethod publico | Alta | Punto de entrada SOAP expuesto | Manual baseline + cobertura indirecta via `PruebaCrudService.Actualizar` | Regresion en validaciones de update |
| `IS_WS_PRUEBA.Eliminar` | `IS_WS_PRUEBA.asmx.cs` | WebMethod publico | Alta | Punto de entrada SOAP expuesto | Manual baseline + cobertura indirecta via `PruebaCrudService.Eliminar` | Regresion en semantica de borrado logico |
| `PruebaCrudService.Crear` | `Domain/Services/PruebaCrudService.cs` | Logica de negocio | Alta | Parseo + persistencia + respuesta funcional | Characterization + Golden Master | Contrato de salida y codigos funcionales |
| `PruebaCrudService.Consultar` | `Domain/Services/PruebaCrudService.cs` | Logica de negocio | Alta | Construye payload de salida completo | Characterization + Golden Master | Cambios no detectados en campos serializados |
| `PruebaCrudService.Actualizar` | `Domain/Services/PruebaCrudService.cs` | Logica de negocio | Alta | Multiples ramas de validacion | Characterization + Golden Master | Regresion en reglas de update parcial |
| `PruebaCrudService.Eliminar` | `Domain/Services/PruebaCrudService.cs` | Logica de negocio | Alta | Borrado logico y mensajes funcionales | Characterization + Golden Master | Cambio de semantica en reintentos de eliminacion |
| `CampoParser.TryBuildFieldMap` | `Domain/Services/CampoParser.cs` | Validador parser | Alta | Puerta de entrada para todos los campos | Characterization + Golden Master | Romper mensajes de error legacy |
| `CampoParser.TryGetRequiredDate` | `Domain/Services/CampoParser.cs` | Validador parser | Alta | Parseo sensible de fechas | Characterization + Golden Master | Divergencia entre mensaje y formatos aceptados |
| `InMemoryPruebaRepository.Create/Update/Delete` | `Infrastructure/InMemoryPruebaRepository.cs` | Persistencia in-memory | Media | Estado global + timestamps no deterministas | Characterization con normalizacion minima | Falsos positivos por campos volatiles |
| `SoapRequestDto`, `SoapResponseDto`, `SoapCampoDto` | `Contracts/*.cs` | Contratos serializados | Media | Contrato XML observable | Golden Master de payloads | Cambios de forma XML/salida |
| `ServiceContractMetadata.ServiceNamespace` | `Contracts/ServiceContractMetadata.cs` | Constante de contrato | Baja | Namespace SOAP estable | Manual baseline only | Cambio accidental de namespace |

## Dependencias y condiciones de riesgo detectadas

- Estado global mutable en `InMemoryPruebaRepository` (`Records`, `_nextId`).
- Campo volatil `FechaActualizacion` generado con `DateTime.UtcNow`.
- Captura global de excepciones en `PruebaCrudService` y `IS_WS_PRUEBA` que convierte errores a codigo tecnico `900`.
- Proyecto ASMX en .NET Framework 4.0; pruebas de extremo a extremo del host requieren IIS/IIS Express.


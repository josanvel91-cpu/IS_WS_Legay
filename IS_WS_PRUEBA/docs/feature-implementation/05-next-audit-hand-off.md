# 05 Next Audit Hand-off

## Vulnerabilidades esperables a detectar
- Exposicion de informacion sensible en campo `detalle` (`usuario`, `id`, `server`, `fecha`).
- Filtracion tecnica en error (`mensaje = exception.ToString()`).
- Comportamiento dependiente de cultura para conversion numerica.

## Issues de calidad esperables
- Duplicacion de reglas/calculos (`capacidad` y rama `plazo_meses > 60`).
- Nombres y reglas legacy no uniformes (alias snake_case/camelCase).
- Regla funcional arbitraria por sufijo de identificacion.
- Regla de aprobacion manual para datos negativos.

## Duplicacion esperable
- Doble calculo de capacidad de pago.
- Recalculo repetido de cuota con seguro en ramas diferentes.
- Estructuras de salida similares para aprobado/rechazado.

## Puntos recomendados para blueprint/remediacion
1. Centralizar parseo numerico/cultura y validar tipos de forma uniforme.
2. Extraer reglas de negocio a funciones puras y eliminar duplicaciones.
3. Sustituir detalle interno por trazabilidad segura (id de correlacion).
4. Reemplazar `exception.ToString()` por mensaje tecnico controlado + logging interno.
5. Definir reglas funcionales explicitas (eliminar reglas arbitrarias y bug de aprobacion manual).

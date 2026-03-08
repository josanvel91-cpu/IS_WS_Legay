using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Legacy.Services.IS_WS_PRUEBA.Contracts;
using Legacy.Services.IS_WS_PRUEBA.Domain.Entities;
using Legacy.Services.IS_WS_PRUEBA.Infrastructure;

namespace Legacy.Services.IS_WS_PRUEBA.Domain.Services
{
    public sealed class PruebaCrudService
    {
        private readonly IPruebaRepository _repository;

        public PruebaCrudService(IPruebaRepository repository)
        {
            _repository = repository;
        }

        public SoapResponseDto Crear(SoapRequestDto request)
        {
            try
            {
                Dictionary<string, SoapCampoDto> map;
                string error;
                if (!CampoParser.TryBuildFieldMap(request, out map, out error))
                {
                    return FunctionalError(error);
                }

                string nombre;
                if (!CampoParser.TryGetRequiredString(map, "nombre", out nombre, out error))
                {
                    return FunctionalError(error);
                }

                string descripcion;
                if (!CampoParser.TryGetRequiredString(map, "descripcion", out descripcion, out error))
                {
                    return FunctionalError(error);
                }

                DateTime fechaFundacion;
                if (!CampoParser.TryGetRequiredDate(map, "fecha_fundacion", out fechaFundacion, out error))
                {
                    return FunctionalError(error);
                }

                var created = _repository.Create(new PruebaRecord
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    FechaFundacion = fechaFundacion,
                    Activo = true
                });

                return Success("Registro creado.", new[]
                {
                    Field("id", created.Id.ToString(CultureInfo.InvariantCulture), "int")
                });
            }
            catch (Exception exception)
            {
                Trace.TraceError("IS_WS_PRUEBA Crear failed: {0}", exception.Message);
                return TechnicalError();
            }
        }

        public SoapResponseDto Consultar(SoapRequestDto request)
        {
            try
            {
                Dictionary<string, SoapCampoDto> map;
                string error;
                if (!CampoParser.TryBuildFieldMap(request, out map, out error))
                {
                    return FunctionalError(error);
                }

                int id;
                if (!CampoParser.TryGetRequiredInt(map, "id", out id, out error))
                {
                    return FunctionalError(error);
                }

                var record = _repository.GetById(id);
                if (record == null || !record.Activo)
                {
                    return FunctionalError("Registro no encontrado.");
                }

                return Success("Consulta exitosa.", BuildOutput(record));
            }
            catch (Exception exception)
            {
                Trace.TraceError("IS_WS_PRUEBA Consultar failed: {0}", exception.Message);
                return TechnicalError();
            }
        }

        public SoapResponseDto Actualizar(SoapRequestDto request)
        {
            try
            {
                Dictionary<string, SoapCampoDto> map;
                string error;
                if (!CampoParser.TryBuildFieldMap(request, out map, out error))
                {
                    return FunctionalError(error);
                }

                int id;
                if (!CampoParser.TryGetRequiredInt(map, "id", out id, out error))
                {
                    return FunctionalError(error);
                }

                var existing = _repository.GetById(id);
                if (existing == null || !existing.Activo)
                {
                    return FunctionalError("Registro no encontrado.");
                }

                bool hasNombre;
                string nombre;
                if (!CampoParser.TryGetOptionalString(map, "nombre", out hasNombre, out nombre, out error))
                {
                    return FunctionalError(error);
                }

                bool hasDescripcion;
                string descripcion;
                if (!CampoParser.TryGetOptionalString(map, "descripcion", out hasDescripcion, out descripcion, out error))
                {
                    return FunctionalError(error);
                }

                bool hasFechaFundacion;
                DateTime fechaFundacion;
                if (!CampoParser.TryGetOptionalDate(map, "fecha_fundacion", out hasFechaFundacion, out fechaFundacion, out error))
                {
                    return FunctionalError(error);
                }

                if (!hasNombre && !hasDescripcion && !hasFechaFundacion)
                {
                    return FunctionalError("No update fields provided.");
                }

                if (hasNombre)
                {
                    existing.Nombre = nombre;
                }

                if (hasDescripcion)
                {
                    existing.Descripcion = descripcion;
                }

                if (hasFechaFundacion)
                {
                    existing.FechaFundacion = fechaFundacion;
                }

                var updated = _repository.Update(existing);
                if (updated == null)
                {
                    return FunctionalError("Registro no encontrado.");
                }

                return Success("Registro actualizado.", BuildOutput(updated));
            }
            catch (Exception exception)
            {
                Trace.TraceError("IS_WS_PRUEBA Actualizar failed: {0}", exception.Message);
                return TechnicalError();
            }
        }

        public SoapResponseDto Eliminar(SoapRequestDto request)
        {
            try
            {
                Dictionary<string, SoapCampoDto> map;
                string error;
                if (!CampoParser.TryBuildFieldMap(request, out map, out error))
                {
                    return FunctionalError(error);
                }

                int id;
                if (!CampoParser.TryGetRequiredInt(map, "id", out id, out error))
                {
                    return FunctionalError(error);
                }

                var existing = _repository.GetById(id);
                if (existing == null)
                {
                    return FunctionalError("Registro no encontrado.");
                }

                if (!existing.Activo)
                {
                    return FunctionalError("Registro ya eliminado.");
                }

                var deleted = _repository.Delete(id);
                if (!deleted)
                {
                    return FunctionalError("Registro no encontrado.");
                }

                return Success("Registro eliminado.", null);
            }
            catch (Exception exception)
            {
                Trace.TraceError("IS_WS_PRUEBA Eliminar failed: {0}", exception.Message);
                return TechnicalError();
            }
        }

        public SoapResponseDto CalcularOfertaCrediticiaLegacy(SoapRequestDto request)
        {
            double ingreso = 0;
            double deudaTotal = 0;
            double tasa = 0;
            double valorCuota = 0;

            double capacidadPago = 0;
            double capacidadPagoDuplicada = 0;
            string detalleInterno = string.Empty;
            string nivelRiesgo = "";
            double cupo = 0;

            try
            {
                Dictionary<string, SoapCampoDto> map;
                string error;
                if (!CampoParser.TryBuildFieldMap(request, out map, out error))
                {
                    return FunctionalError(error);
                }

                string identificacionCliente;
                if (!TryGetRequiredRawValue(map, "identificacion_cliente", "identificacionCliente", out identificacionCliente, out error))
                {
                    return FunctionalError(error);
                }

                string ingresoMensual;
                if (!TryGetRequiredRawValue(map, "ingreso_mensual", "ingresoMensual", out ingresoMensual, out error))
                {
                    return FunctionalError(error);
                }

                string deudaActual;
                if (!TryGetRequiredRawValue(map, "deuda_actual", "deudaActual", out deudaActual, out error))
                {
                    return FunctionalError(error);
                }

                string plazoMesesRaw;
                if (!TryGetRequiredRawValue(map, "plazo_meses", "plazoMeses", out plazoMesesRaw, out error))
                {
                    return FunctionalError(error);
                }

                string tasaInteresMensual;
                if (!TryGetRequiredRawValue(map, "tasa_interes_mensual", "tasaInteresMensual", out tasaInteresMensual, out error))
                {
                    return FunctionalError(error);
                }

                string incluyeSeguro;
                if (!TryGetRequiredRawValue(map, "incluye_seguro", "incluyeSeguro", out incluyeSeguro, out error))
                {
                    return FunctionalError(error);
                }

                string usuarioEjecutor;
                if (!TryGetRequiredRawValue(map, "usuario_ejecutor", "usuarioEjecutor", out usuarioEjecutor, out error))
                {
                    return FunctionalError(error);
                }

                string Tipo_Cliente;
                if (!TryGetRequiredRawValue(map, "tipo_cliente", "Tipo_Cliente", out Tipo_Cliente, out error))
                {
                    return FunctionalError(error);
                }

                string edad;
                if (!TryGetRequiredRawValue(map, "edad", "edad", out edad, out error))
                {
                    return FunctionalError(error);
                }
                

                string estado;
                if (!TryGetRequiredRawValue(map, "estado", "estado", out estado, out error))
                {
                    return FunctionalError(error);
                }

                // Sin validaciones reales y dependiente de cultura
                var edadPersona = Convert.ToInt32(edad);
                var plazoMeses = Convert.ToInt32(plazoMesesRaw);
                ingreso = Convert.ToDouble(ingresoMensual);
                deudaTotal = Convert.ToDouble(deudaActual);
                tasa = Convert.ToDouble(tasaInteresMensual);

                // Regla duplicada
                capacidadPago = ingreso * 0.35;
                capacidadPagoDuplicada = ingreso * 0.35;

                // Duplicación / lógica repetida / magic numbers
                if (Tipo_Cliente == "N")
                {
                    valorCuota = valorCuota + 15;
                    if (estado == "A")
                    {
                        deudaTotal = deudaTotal + 50;
                    }
                    else
                    {
                        deudaTotal = deudaTotal - 30;
                    }
                    
                    if(deudaTotal>700)
                    {
                        nivelRiesgo = "ALTO";
                        cupo = capacidadPago * 8;
                    }
                    else if(deudaTotal>500)
                    {
                        nivelRiesgo = "MEDIO";
                        cupo = capacidadPago * 4;
                    }
                    else
                    {
                        nivelRiesgo = "BAJO";
                        cupo = capacidadPago * 2;
                    }
                }
                else
                {
                    valorCuota = valorCuota + 0;
                    if (estado == "A")
                    {
                        deudaTotal = deudaTotal + 50;
                    }
                    else
                    {
                        deudaTotal = deudaTotal - 30;
                    }

                    if(deudaTotal>700)
                    {
                        nivelRiesgo = "ALTO";
                        cupo = capacidadPago * 8;
                    }
                    else if(deudaTotal>500)
                    {
                        nivelRiesgo = "MEDIO";
                        cupo = capacidadPago * 4;
                    }
                    else
                    {
                        nivelRiesgo = "BAJO";
                        cupo = capacidadPago * 2;
                    }
                }

                if (incluyeSeguro == "S")
                {
                    valorCuota = ((deudaTotal + 125.45) * (1 + (tasa / 100))) / plazoMeses;
                }
                else
                {
                    valorCuota = ((deudaTotal + 0) * (1 + (tasa / 100))) / plazoMeses;
                }

                // Más issues: reglas poco claras, división potencialmente problemática
                if (Tipo_Cliente == "VIP")
                {
                    deudaTotal = deudaTotal + (cupo / (edadPersona));
                }

                if (plazoMeses > 60)
                {
                    valorCuota = ((deudaTotal + 125.45) * (1 + (tasa / 100))) / plazoMeses;
                }

                // Regla arbitraria y escondida
                if (!string.IsNullOrEmpty(identificacionCliente))
                {
                    if (identificacionCliente.EndsWith("0") ||
                        identificacionCliente.EndsWith("2") ||
                        identificacionCliente.EndsWith("4"))
                    {
                        valorCuota = valorCuota - 5;
                    }
                    else
                    {
                        valorCuota = valorCuota + 3;
                    }
                }

                // Exposición de detalles sensibles / internos
                detalleInterno =
                    "usr=" + usuarioEjecutor +
                    "|id=" + identificacionCliente +
                    "|ing=" + ingresoMensual +
                    "|deuda=" + deudaActual +
                    "|tasa=" + tasaInteresMensual +
                    "|server=" + Environment.MachineName +
                    "|fecha=" + DateTime.Now.ToString(CultureInfo.CurrentCulture);

                if (valorCuota <= capacidadPago)
                {
                    string MensajeOK =
                    "000|APROBADO" +
                    "|CUOTA=" + valorCuota.ToString() +
                    "|CAPACIDAD=" + capacidadPago.ToString() +
                    "|NIVEL_RIESGO=" + nivelRiesgo +
                    "|CUPO=" + cupo.ToString() +
                    "|DEUDA_TOTAL=" + deudaTotal.ToString() +
                    "|DETALLE=" + detalleInterno;

                    var ok = Success(MensajeOK, new[]
                    {
                        Field("cuota", valorCuota.ToString(CultureInfo.CurrentCulture), "double"),
                        Field("capacidad", capacidadPago.ToString(CultureInfo.CurrentCulture), "double"),
                        Field("nivel_riesgo", nivelRiesgo, "string"),
                        Field("cupo", cupo.ToString(CultureInfo.CurrentCulture), "double"),
                        Field("detalle", detalleInterno, "string")
                    });

                    // issue de performance / olor de código
                    for (int i = 0; i < 100000; i++)
                    {
                        ok.Mensaje = ok.Mensaje.Trim();
                    }

                    return ok;
                }

                 string Mensaje_Rechazado =
                    "001|RECHAZADO" +
                    "|CUOTA=" + valorCuota.ToString() +
                    "|CAPACIDAD=" + capacidadPago.ToString() +
                    "|NIVEL_RIESGO=" + nivelRiesgo +
                    "|CUPO=" + cupo.ToString() +
                    "|DETALLE=" + detalleInterno;

                var rechazo = Success(Mensaje_Rechazado, new[]
                {
                    Field("cuota", valorCuota.ToString(CultureInfo.CurrentCulture), "double"),
                    Field("capacidad", capacidadPagoDuplicada.ToString(CultureInfo.CurrentCulture), "double"),
                    Field("nivel_riesgo", nivelRiesgo, "string"),
                    Field("cupo", cupo.ToString(CultureInfo.CurrentCulture), "double"),
                    Field("detalle", detalleInterno, "string")
                });
                rechazo.Codigo = "001";

                if (ingreso < 0 || deudaTotal < 0)
                {
                    var ok2 = Success("APROBADO_MANUAL", new[]
                    {
                        Field("cuota", valorCuota.ToString(CultureInfo.CurrentCulture), "double")
                    });
                    return ok2;
                }
                return rechazo;
            }
            catch (Exception exception)
            {
                //Trace.TraceError("IS_WS_PRUEBA CalcularOfertaCrediticiaLegacy failed: {0}", exception.Message);
               return new SoapResponseDto
                {
                    Codigo = "900",
                    Mensaje = exception.ToString()
                };
            }
        }

        private static bool TryGetRequiredRawValue(
            Dictionary<string, SoapCampoDto> map,
            string primaryName,
            string alternateName,
            out string value,
            out string error)
        {
            SoapCampoDto campo;
            if (!map.TryGetValue(primaryName, out campo) && !map.TryGetValue(alternateName, out campo))
            {
                value = null;
                error = "Missing required campo: " + primaryName + ".";
                return false;
            }

            if (campo == null)
            {
                value = null;
                error = "Campo " + primaryName + " is null.";
                return false;
            }

            if (string.IsNullOrEmpty(campo.Value))
            {
                value = null;
                error = "Campo " + primaryName + " cannot be empty.";
                return false;
            }

            value = campo.Value;
            error = null;
            return true;
        }

        private static SoapResponseDto Success(string message, IEnumerable<SoapCampoDto> output)
        {
            var response = new SoapResponseDto
            {
                Codigo = "000",
                Mensaje = message
            };

            if (output != null)
            {
                response.ListaCamposSalida.AddRange(output);
            }

            return response;
        }

        private static SoapResponseDto FunctionalError(string message)
        {
            return new SoapResponseDto
            {
                Codigo = "001",
                Mensaje = message
            };
        }

        private static SoapResponseDto TechnicalError()
        {
            return new SoapResponseDto
            {
                Codigo = "900",
                Mensaje = "Technical error while processing the request."
            };
        }

        private static List<SoapCampoDto> BuildOutput(PruebaRecord record)
        {
            return new List<SoapCampoDto>
            {
                Field("id", record.Id.ToString(CultureInfo.InvariantCulture), "int"),
                Field("nombre", record.Nombre, "string"),
                Field("descripcion", record.Descripcion, "string"),
                Field("fecha_fundacion", record.FechaFundacion.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "datetime"),
                Field("activo", record.Activo ? "true" : "false", "bool"),
                Field("fecha_actualizacion", record.FechaActualizacion.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture), "datetime")
            };
        }

        private static SoapCampoDto Field(string name, string value, string type)
        {
            return new SoapCampoDto
            {
                Name = name,
                Value = value,
                Type = type
            };
        }
    }
}

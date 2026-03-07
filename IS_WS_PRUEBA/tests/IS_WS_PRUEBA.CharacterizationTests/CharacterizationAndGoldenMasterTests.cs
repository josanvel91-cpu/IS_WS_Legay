using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Legacy.Services.IS_WS_PRUEBA.Contracts;
using Legacy.Services.IS_WS_PRUEBA.Domain.Entities;
using Legacy.Services.IS_WS_PRUEBA.Domain.Services;
using Legacy.Services.IS_WS_PRUEBA.Infrastructure;
using Xunit;

namespace IS_WS_PRUEBA.CharacterizationTests;

public sealed class CharacterizationAndGoldenMasterTests
{
    [Fact]
    public void CampoParser_DuplicateCampoName_MatchesGoldenMaster()
    {
        var request = BuildRequest(
            ("nombre", "Empresa A", "string"),
            ("nombre", "Empresa B", "string"));

        var ok = CampoParser.TryBuildFieldMap(request, out _, out var error);
        var snapshot = ToJson(new
        {
            ok,
            error
        });

        AssertApproved("CampoParser/duplicate-campo-name.approved.json", snapshot);
    }

    [Fact]
    public void CampoParser_RequiredDate_AcceptsZuluFormat_MatchesGoldenMaster()
    {
        var request = BuildRequest(("fecha_fundacion", "2020-05-01T10:20:30Z", "datetime"));
        Assert.True(CampoParser.TryBuildFieldMap(request, out var map, out var mapError), mapError);

        var ok = CampoParser.TryGetRequiredDate(map!, "fecha_fundacion", out var parsed, out var error);
        var snapshot = ToJson(new
        {
            ok,
            error,
            parsed = parsed.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
        });

        AssertApproved("CampoParser/required-date-accepts-zulu.approved.json", snapshot);
    }

    [Fact]
    public void PruebaCrudService_CrearConsultar_MatchesGoldenMaster()
    {
        ResetRepositoryState();
        var service = NewService();

        var crear = service.Crear(BuildRequest(
            ("nombre", "Empresa Legacy", "string"),
            ("descripcion", "Descripcion inicial", "string"),
            ("fecha_fundacion", "2020-05-01", "datetime")));

        var consultar = service.Consultar(BuildRequest(("id", "1", "int")));

        var snapshot = ToJson(new
        {
            create = ToStableResponse(crear),
            consultar = ToStableResponse(consultar)
        });

        AssertApproved("PruebaCrudService/crear-consultar.approved.json", snapshot);
    }

    [Fact]
    public void PruebaCrudService_ActualizarSinCampos_MatchesGoldenMaster()
    {
        ResetRepositoryState();
        var service = NewService();
        service.Crear(BuildRequest(
            ("nombre", "Empresa Legacy", "string"),
            ("descripcion", "Descripcion inicial", "string"),
            ("fecha_fundacion", "2020-05-01", "datetime")));

        var actualizarSinCampos = service.Actualizar(BuildRequest(("id", "1", "int")));
        var snapshot = ToJson(new
        {
            actualizarSinCampos = ToStableResponse(actualizarSinCampos)
        });

        AssertApproved("PruebaCrudService/actualizar-sin-campos.approved.json", snapshot);
    }

    [Fact]
    public void PruebaCrudService_EliminarDosVeces_MatchesGoldenMaster()
    {
        ResetRepositoryState();
        var service = NewService();
        service.Crear(BuildRequest(
            ("nombre", "Empresa Legacy", "string"),
            ("descripcion", "Descripcion inicial", "string"),
            ("fecha_fundacion", "2020-05-01", "datetime")));

        var firstDelete = service.Eliminar(BuildRequest(("id", "1", "int")));
        var secondDelete = service.Eliminar(BuildRequest(("id", "1", "int")));

        var snapshot = ToJson(new
        {
            firstDelete = ToStableResponse(firstDelete),
            secondDelete = ToStableResponse(secondDelete)
        });

        AssertApproved("PruebaCrudService/eliminar-doble.approved.json", snapshot);
    }

    [Fact]
    public void PruebaCrudService_CalcularOfertaCrediticiaLegacy_Aprobado_MatchesGoldenMaster()
    {
        ResetRepositoryState();
        var service = NewService();

        var response = service.CalcularOfertaCrediticiaLegacy(BuildRequest(
            ("identificacion_cliente", "12345678", "string"),
            ("ingreso_mensual", "1000", "string"),
            ("deuda_actual", "6000", "string"),
            ("plazo_meses", "24", "int"),
            ("tasa_interes_mensual", "1.5", "string"),
            ("incluye_seguro", "S", "string"),
            ("usuario_ejecutor", "analista01", "string"),
            ("tipo_cliente", "N", "string"),
            ("estado", "A", "string"),
            ("edad", "35", "int")));

        var snapshot = ToJson(new
        {
            response = ToStableResponse(response)
        });

        AssertApproved("PruebaCrudService/calcular-oferta-crediticia-aprobado.approved.json", snapshot);
    }

    [Fact]
    public void PruebaCrudService_CalcularOfertaCrediticiaLegacy_AprobadoManual_MatchesGoldenMaster()
    {
        ResetRepositoryState();
        var service = NewService();

        var response = service.CalcularOfertaCrediticiaLegacy(BuildRequest(
            ("identificacion_cliente", "9988772", "string"),
            ("ingreso_mensual", "-1000", "string"),
            ("deuda_actual", "6000", "string"),
            ("plazo_meses", "24", "int"),
            ("tasa_interes_mensual", "1.5", "string"),
            ("incluye_seguro", "N", "string"),
            ("usuario_ejecutor", "analista02", "string"),
            ("tipo_cliente", "N", "string"),
            ("estado", "A", "string"),
            ("edad", "35", "int")));

        var snapshot = ToJson(new
        {
            response = ToStableResponse(response)
        });

        AssertApproved("PruebaCrudService/calcular-oferta-crediticia-aprobado-manual.approved.json", snapshot);
    }

    private static PruebaCrudService NewService()
    {
        return new PruebaCrudService(InMemoryPruebaRepository.Instance);
    }

    private static SoapRequestDto BuildRequest(params (string Name, string Value, string Type)[] fields)
    {
        var request = new SoapRequestDto();
        foreach (var field in fields)
        {
            request.ListaCampos.Add(new SoapCampoDto
            {
                Name = field.Name,
                Value = field.Value,
                Type = field.Type
            });
        }

        return request;
    }

    private static object ToStableResponse(SoapResponseDto response)
    {
        return new
        {
            codigo = response.Codigo,
            mensaje = response.Mensaje,
            listaCamposSalida = response.ListaCamposSalida.Select(
                campo => new
                {
                    name = campo.Name,
                    value = NormalizeValue(campo.Name, campo.Value),
                    type = campo.Type
                }).ToArray()
        };
    }

    private static string NormalizeValue(string? fieldName, string? fieldValue)
    {
        if (string.Equals(fieldName, "fecha_actualizacion", StringComparison.OrdinalIgnoreCase))
        {
            return "<UTC_TIMESTAMP>";
        }

        if (string.Equals(fieldName, "detalle", StringComparison.OrdinalIgnoreCase))
        {
            return NormalizeDetalle(fieldValue);
        }

        if (string.Equals(fieldName, "cuota", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "capacidad", StringComparison.OrdinalIgnoreCase))
        {
            return NormalizeNumeric(fieldValue);
        }

        return fieldValue ?? string.Empty;
    }

    private static string NormalizeDetalle(string? fieldValue)
    {
        if (string.IsNullOrEmpty(fieldValue))
        {
            return string.Empty;
        }

        var parts = fieldValue.Split('|');
        for (var index = 0; index < parts.Length; index++)
        {
            if (parts[index].StartsWith("server=", StringComparison.OrdinalIgnoreCase))
            {
                parts[index] = "server=<MACHINE_NAME>";
                continue;
            }

            if (parts[index].StartsWith("fecha=", StringComparison.OrdinalIgnoreCase))
            {
                parts[index] = "fecha=<LOCAL_TIMESTAMP>";
            }
        }

        return string.Join("|", parts);
    }

    private static string NormalizeNumeric(string? fieldValue)
    {
        if (string.IsNullOrWhiteSpace(fieldValue))
        {
            return string.Empty;
        }

        double parsed;
        if (double.TryParse(fieldValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out parsed) ||
            double.TryParse(fieldValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out parsed))
        {
            return parsed.ToString("G17", CultureInfo.InvariantCulture);
        }

        return fieldValue;
    }

    private static string ToJson(object value)
    {
        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    private static void AssertApproved(string relativePath, string actual)
    {
        var approvedPath = Path.GetFullPath(Path.Combine(ProjectRoot(), "tests", "GoldenMaster", relativePath));
        Assert.True(File.Exists(approvedPath), "Missing approved snapshot: " + approvedPath);

        var approved = File.ReadAllText(approvedPath);
        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    private static string ProjectRoot()
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }

    private static string NormalizeNewLines(string text)
    {
        return text.Replace("\r\n", "\n").TrimEnd('\r', '\n');
    }

    private static void ResetRepositoryState()
    {
        var repositoryType = typeof(InMemoryPruebaRepository);
        const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        var syncField = repositoryType.GetField("Sync", Flags)
            ?? throw new InvalidOperationException("Sync field not found.");
        var recordsField = repositoryType.GetField("Records", Flags)
            ?? throw new InvalidOperationException("Records field not found.");
        var nextIdField = repositoryType.GetField("_nextId", Flags)
            ?? throw new InvalidOperationException("_nextId field not found.");

        var syncLock = syncField.GetValue(null)
            ?? throw new InvalidOperationException("Sync value not found.");

        lock (syncLock)
        {
            var records = recordsField.GetValue(null) as IDictionary<int, PruebaRecord>
                ?? throw new InvalidOperationException("Records dictionary not found.");

            records.Clear();
            nextIdField.SetValue(null, 1);
        }
    }
}

using ClosedXML.Excel;
using CoreContable.Entities.FunctionResult;
using CoreContable.Entities.FuntionResult;
using CoreContable.Models.Report;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NPOI.SS.Formula.Functions;
using Rotativa.AspNetCore;
using System.Globalization;

namespace CoreContable.Controllers;

[Authorize]
public class ReportsController(
    ILogger<ReportsController> logger,
    IReportsRepository reportsRepository,
    IDmgPolizaRepository dmgPolizaRepository,
    ISecurityRepository securityRepository,
    AccountUtils accountUtils
) : Controller
{
    private object reportType;

    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_REPORTS)]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    //--------------------------------------------------REPORTES------------------------------------------------------------

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_PRINT}," + $"{CC.THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_PRINT}")]
    [HttpGet]
    public async Task<IActionResult> PolizaDiarioOMayor([FromQuery] string codCia, [FromQuery] string tipoDocto,
        [FromQuery] int numPoliza, [FromQuery] int periodo, [FromQuery] string reportType)
    {
        try
        {
            List<ReportePolizaDiarioFromFuncResultSet> reportData = [];
            var nombreReporte = "";

            switch (reportType)
            {
                case CC.REPORT_TYPE_DIARIO:
                    reportData = await reportsRepository.GetDataForPolizaDiario(codCia, tipoDocto, numPoliza, periodo);
                    nombreReporte = "Comprobante de Diario";

                    // Validar que hay datos
                    if (reportData == null || reportData.Count == 0)
                    {
                        return NotFound("No se encontraron datos para este reporte");
                    }
                    break;
                case CC.REPORT_TYPE_MAYOR:
                    reportData = await reportsRepository.GetDataForPolizaMayor(codCia, tipoDocto, numPoliza, periodo);
                    nombreReporte = "Asiento Contable Mayorizado";

                    // Validar que hay datos
                    if (reportData == null || reportData.Count == 0)
                    {
                        return NotFound("No se encontraron datos para este reporte");
                    }
                    break;
            }

            if (reportData.Count > 0)
            {
                var polizaDiarioData = MakePolizaDiarioData(reportData, nombreReporte);
                if (reportType == CC.REPORT_TYPE_MAYOR)
                    await dmgPolizaRepository.SetPrinted(codCia, periodo, tipoDocto, numPoliza);

                return new ViewAsPdf(CC.REPORT_NAME_COMP_DIARIO, polizaDiarioData)
                {
                    PageSize = CC.DEFAULT_REPORT_SIZE,
                    PageOrientation = CC.DEFAULT_REPORT_ORIENTATION,
                    PageMargins = CC.DEFAULT_MARGINS,
                    CustomSwitches = CC.GetDefaultReportSwitches(securityRepository.GetSessionFullName())
                };
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ReportsController), nameof(PolizaDiarioOMayor));
        }

        return NotFound();
    }


    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_REPORTS)]
    [HttpGet]
    public async Task<IActionResult> HistoricoDecuenta([FromQuery] string codCia, [FromQuery] string cuentaContable, [FromQuery] string startDate,
    [FromQuery] string finishDate, [FromQuery] int cta1, [FromQuery] int cta2, [FromQuery] int cta3, [FromQuery] int cta4, [FromQuery] int cta5,
    [FromQuery] int cta6
)
    {
        try
        {
            // Obtener los datos
            List<ReporteHistoricoCuentaFromFuncResultSet>? reportData = await reportsRepository.GetDataForHistoricoCuenta(
                codCia, cuentaContable, startDate, finishDate, cta1, cta2, cta3, cta4, cta5, cta6
            );

            // Verificar si hay datos
            if (reportData != null && reportData.Count > 0)
            {
                // Ordenar los datos por la fecha
                reportData = reportData.OrderBy(r => r.FECHA).ToList();

                // Capturar el saldo anterior del primer registro
                var saldoAnterior = reportData.FirstOrDefault()?.SaldoAnterior ?? 0;

                var convertedData = reportData.Select(d => new ReporteHistoricoCuentaFromFunc
                {
                    Nombre_Cia = d.NombreCia,
                    NUM_POLIZA = d.NUM_POLIZA,
                    CORRELAT = d.CORRELAT,
                    centro_costo = d.centro_costo,
                    TipoDocto = d.TipoDocto,
                    CTA_1 = d.CTA_1,
                    CTA_2 = d.CTA_2,
                    CTA_3 = d.CTA_3,
                    CTA_4 = d.CTA_4,
                    CTA_5 = d.CTA_5,
                    CTA_6 = d.CTA_6,
                    DESCRIP_ESP = d.DESCRIP_ESP,
                    FECHA = d.FECHA,
                    MES = d.MES,
                    CuentaNivel = d.NumeroCuenta,
                    ANIO = d.ANIO,
                    CONCEPTO = d.CONCEPTO,
                    CARGO = d.CARGO,
                    ABONO = d.ABONO,
                    SaldoAnterior = d.SaldoAnterior,
                    SaldoMesActual = d.SaldoMesActual
                }).ToList();


                // Crear el modelo para la vista/pdf
                var polizaDiarioData = MakeHistoricoCuentaData(convertedData, startDate, finishDate);

                // Retornar el PDF usando Rotativa
                return new ViewAsPdf(CC.REPORT_NAME_HISTORICO_CUENTA, polizaDiarioData)
                {
                    PageSize = CC.DEFAULT_REPORT_SIZE,
                    PageOrientation = CC.DEFAULT_REPORT_ORIENTATION_H,
                    PageMargins = CC.DEFAULT_MARGINS,
                    CustomSwitches = CC.GetDefaultReportSwitches(securityRepository.GetSessionFullName())
                };
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ReportsController), nameof(HistoricoDecuenta));
        }

        return NotFound();
    }



    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_REPORTS)]
    [HttpGet]
    public async Task<IActionResult> BalanceComprobacion([FromQuery] string codCia, [FromQuery] string startDate, [FromQuery] string finishDate, [FromQuery] string level)
    {
        try
        {
            var reportData = await reportsRepository.GetDataForBalanceComprobacion(codCia, startDate, finishDate, level);

            if (reportData is { Count: > 0 })
            {
                // Aquí no se separan activos y pasivos, simplemente se trabaja con todos los registros
                var balanceComprobacionData = new ReporteBalanceComprobacion
                {
                    CiaNombre = reportData[0].NombreCia,
                    Subtitulo = "Balance de Comprobación",
                    Dia = DateTimeUtils.GetDayFromString(finishDate),
                    Mes = DateTimeUtils.GetMonthFromString(finishDate),
                    Anio = DateTimeUtils.GetYearFromString(finishDate),
                    // No es necesario separar activos y pasivos, se combinan en una sola lista
                    CuentasUnificadas = reportData
                        .OrderBy(cuenta => string.Join("", cuenta.CuentaContable.Split(' '))) // Ordenar como cadena completa
                        .ThenBy(cuenta => cuenta.CTA_NIVEL) // Si es necesario, ordenar por el nivel también
                        .Where(cuenta => cuenta.SaldoAnterior != 0 || cuenta.Cargos != 0 || cuenta.Abonos != 0 || cuenta.SaldoDelMes != 0 || cuenta.SaldoActual != 0)
                        .Select(cuenta => CreateReporteBalanceComprobacionLista(cuenta)) // Mapear los resultados a la estructura adecuada
                        .ToList()
                };

                // Generación del archivo PDF
                var fileName = $"{codCia} - BALANCE_COMPROBACION - {DateTimeUtils.GetCurrentTimeSpanAsStringForFileName()}.pdf";

                return new ViewAsPdf(CC.REPORT_NAME_BALANCE_COMP, balanceComprobacionData)
                {
                    PageSize = CC.DEFAULT_REPORT_SIZE,
                    PageOrientation = CC.DEFAULT_REPORT_ORIENTATION,
                    PageMargins = CC.DEFAULT_MARGINS,
                    CustomSwitches = CC.GetDefaultReportSwitches(securityRepository.GetSessionFullName())
                };
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(ReportsController), nameof(BalanceComprobacion));
        }

        return NotFound();
    }



    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_REPORTS)]
    [HttpGet]
    public async Task<IActionResult> BalanceGral(
            [FromQuery] string codCia,
            [FromQuery] string fecha)
    {
        try
        {


            // Llamar al repositorio para obtener los datos
            var reportData = await reportsRepository.GetDataForBalanceGral(fecha, codCia);

            // Validar que hay datos
            if (reportData == null || reportData.Count == 0)
            {
                return NotFound("No se encontraron datos para los criterios proporcionados.");
            }

            // Crear el modelo del reporte
            var balanceGralData = MakeBalanceGralData(reportData, fecha);

            // Generación del PDF usando Rotativa
            return new ViewAsPdf("BalanceGral", balanceGralData)
            {
                PageSize = CC.DEFAULT_REPORT_SIZE,
                PageOrientation = CC.DEFAULT_REPORT_ORIENTATION_H,
                PageMargins = CC.DEFAULT_MARGINS,
                CustomSwitches = CC.DEFAULT_REPORT_SWITCHES
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(ReportsController), nameof(BalanceGral));
            return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
        }
    }

    private ReporteBalanceGral MakeBalanceGralData(List<ReporteBalanceGralFromFunc> data, string fecha)
    {
        return new ReporteBalanceGral
        {
            FechaReporte = fecha,
            Compania = data[0].Nombre_Cia,
            Detalles = data
        };
    }


    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_REPORTS)]
    [HttpGet]
    public async Task<IActionResult> DiarioMayor([FromQuery] string codCia, [FromQuery] string cuentaContable,
        [FromQuery] string startDate, [FromQuery] string finishDate, [FromQuery] int cta1, [FromQuery] int cta2,
        [FromQuery] int cta3, [FromQuery] int cta4, [FromQuery] int cta5, [FromQuery] int cta6)
    {
        try
        {
            var reportData = await reportsRepository.GetDataForDiarioMayor(codCia, cuentaContable, startDate,
                finishDate, cta1, cta2, cta3, cta4, cta5, cta6);

            if (reportData != null && reportData.Count > 0)
            {
                var diarioMayorData = MakeDiarioMayorData(reportData, startDate,
                finishDate);

                return new ViewAsPdf("DiarioMayor", diarioMayorData)
                {
                    PageSize = CC.DEFAULT_REPORT_SIZE,
                    PageOrientation = CC.DEFAULT_REPORT_ORIENTATION_H,
                    PageMargins = CC.DEFAULT_MARGINS,
                    CustomSwitches = CC.GetDefaultReportSwitches(securityRepository.GetSessionFullName())
                };
            }
            else
            {
                logger.LogWarning("No se encontraron datos para el reporte Diario Mayor.");
                return NotFound("No se encontraron datos para el reporte Diario Mayor.");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(ReportsController), nameof(DiarioMayor));
            return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
        }
    }

    private ReporteDiarioMayor MakeDiarioMayorData(List<ReporteDiarioMayorFromFunc> data, string fechaInicio,
    string fechaFin)
    {
        return new ReporteDiarioMayor
        {
            NombreCompania = data[0].NombreCompania,
            DescripcionTipoDocumento = data[0].DescripcionTipoDocumento,
            NUM_POLIZA = data[0].NUM_POLIZA,
            Fecha_Poliza = data[0].Fecha_Poliza,
            ConceptoEncabezado = data[0].ConceptoEncabezado,
            TipoDocto = data[0].TIPO_DOCTO,
            CentroCosto = data[0].CENTRO_COSTO,
            NumeroDeCuenta = data[0].FullAccountNumber,
            NombreCuenta = data[0].NombreCuenta,
            Concepto = data[0].ConceptoDetalle,
            Cargo = (double)data[0].CARGO,
            Abono = (double)data[0].ABONO,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Detalles = data
        };
    }

    [IsAuthorized(alias: CC.FIST_LEVEL_PERMISSION_REPORTS)]
    [HttpGet]
    public async Task<IActionResult> EstadosResultados(
     [FromQuery] string codCia,
     [FromQuery] string fechaInicio,
     [FromQuery] string fechaFin)
    {
        try
        {

            // Obtener datos del repositorio
            var reportData = await reportsRepository.GetDataForEstadoResultados(codCia, fechaInicio, fechaFin);

            // Validar que hay datos
            if (reportData == null || reportData.Count == 0)
            {
                return NotFound("No se encontraron datos para los criterios proporcionados.");
            }

            // Crear el modelo del reporte
            var estadoResultadosData = MakeEstadoResultadosData(reportData, fechaInicio, fechaFin);

            // Generación del PDF usando Rotativa
            return new ViewAsPdf("EstadosResultados", estadoResultadosData)
            {
                PageSize = CC.DEFAULT_REPORT_SIZE,
                PageOrientation = CC.DEFAULT_REPORT_ORIENTATION,
                PageMargins = CC.DEFAULT_MARGINS,
                CustomSwitches = CC.DEFAULT_REPORT_SWITCHES
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(ReportsController), nameof(EstadosResultados));
            return StatusCode(500, "Ocurrió un error interno al procesar la solicitud.");
        }
    }


    private ReporteEstadoResultados MakeEstadoResultadosData(
    List<ReporteEstadoResultadosDetalle> data,
    string fechaInicio,
    string fechaFin)
    {
        // Declaración de variables para la cabecera con valores predeterminados en caso de errores en fechas
        string nombreCia = data.FirstOrDefault()?.Nombre_Cia ?? "Nombre de Compañía Desconocido";
        string dia = "00";
        string mes = "00";
        string anio = "0000";
        string subtitulo = "Estado de Resultados";

        // Intentar parsear fechaFin y asignar día, mes y año si el formato es correcto
        if (DateTime.TryParseExact(fechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFechaFin))
        {
            dia = parsedFechaFin.Day.ToString("00");
            mes = parsedFechaFin.Month.ToString("00");
            anio = parsedFechaFin.Year.ToString();
        }
        else
        {
            // Log o advertencia en caso de que la fecha no esté en el formato esperado
            logger.LogWarning("El formato de fechaFin '{fechaFin}' no es válido. Se esperaba 'yyyy-MM-dd'.", fechaFin);
        }

        // Calcular el total de ventas para los porcentajes
        decimal totalVentas = data
            .Where(d => d.Grupo_Cta == "VENTAS")
            .Sum(d => d.Saldo);

        // Calcular los porcentajes para cada detalle
        foreach (var detalle in data)
        {
            detalle.PorcentajeMes = totalVentas != 0 ? ((detalle.Saldo / totalVentas) * 100) : 0;
            detalle.PorcentajeAcumulado = totalVentas != 0 ? (detalle.Saldo / totalVentas) * 100 : 0;
        }

        // Crear el modelo del reporte con los datos recibidos y las fechas formateadas
        return new ReporteEstadoResultados
        {
            NombreCia = nombreCia,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Dia = dia,
            Mes = mes,
            Anio = anio,
            Subtitulo = subtitulo,
            Detalles = data
        };
    }


    //-----------------------------------------------------------------------------------------------------------------

    private ReportePolizaDiario MakePolizaDiarioData(
        List<ReportePolizaDiarioFromFuncResultSet> data, string reportType)
    {
        ReportePolizaDiarioCabecera? cabecera = null;
        List<ReportePolizaDiarioCuenta> cuentas = [];

        try
        {
            // Paso 1: Obteniendo la cabecera.
            cabecera = new ReportePolizaDiarioCabecera
            {
                NombreReporte = reportType,
                NombreCompania = data[0].NombreCompania,
                DescripcionTipoDocumento = data[0].DescripcionTipoDocumento,
                NUM_POLIZA = data[0].NUM_POLIZA,
                Fecha_Poliza = data[0].Fecha_Poliza,
                ConceptoEncabezado = data[0].ConceptoEncabezado
            };

            // Paso 2: Agrupando las cuentas.
            cuentas.AddRange(data.Select(diario => new ReportePolizaDiarioCuenta
            {
                CentroCosto = diario.CENTRO_COSTO,
                // NumeroDeCuenta = diario.FullAccountNumber,
                NumeroDeCuenta = diario.FullAccountNumber,
                NombreCuenta = diario.NombreCuenta,
                Concepto = diario.ConceptoDetalle,
                Cargo = diario.CARGO,
                Abono = diario.ABONO
            }));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ReportsController), nameof(MakePolizaDiarioData));
        }

        return new ReportePolizaDiario
        {
            Cabecera = cabecera,
            Cuentas = cuentas
        };
    }

    private double SumCargosFilterByMonth(List<ReporteHistoricoCuentaFromFunc> data, int mes)
    {
        return data.Where(d => d.MES == mes).Sum(d => d.CARGO ?? 0);
    }

    private double SumAbonosFilterByMonth(List<ReporteHistoricoCuentaFromFunc> data, int mes)
    {
        return data.Where(d => d.MES == mes).Sum(d => d.ABONO ?? 0);
    }

    private decimal GetFinalBalance(List<ReporteHistoricoCuentaFromFunc> data, int mes, decimal? saldoAnterior)
    {
        var totalCargos = data.Where(d => d.MES == mes).Sum(d => d.CARGO ?? 0);
        var totalAbonos = data.Where(d => d.MES == mes).Sum(d => d.ABONO ?? 0);
        return saldoAnterior.GetValueOrDefault() + (decimal)(totalCargos - totalAbonos);
    }

    private ReporteHistoricoCuentas MakeHistoricoCuentaData(
    List<ReporteHistoricoCuentaFromFunc> data,
    string fechaInicio,
    string fechaFin)
    {
        // Paso 1: Cabecera
        var cabecera = new ReporteHistoricoCuentasCabecera
        {
            NombreCia = data.FirstOrDefault()?.Nombre_Cia ?? string.Empty,
            NumeroCuenta = data.FirstOrDefault()?.CuentaNivel ?? string.Empty,
            NombreCuenta = data.FirstOrDefault()?.DESCRIP_ESP ?? string.Empty,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin
        };

        // Paso 2: Agrupar y calcular datos de títulos y detalles
        var titulos = new List<ReporteHistoricoCuentasListaTitulo>();
        var detalles = new List<ReporteHistoricoCuentasListaDetalle>();

        foreach (var cuenta in data)
        {
            // Solo agregar un título por mes
            if (!titulos.Any(t => t.Mes == cuenta.MES && t.Anio == cuenta.ANIO))
            {
                titulos.Add(new ReporteHistoricoCuentasListaTitulo
                {
                    Mes = cuenta.MES,
                    Anio = cuenta.ANIO,
                    SaldoAnterior = cuenta.SaldoAnterior ?? 0,
                    Cargo = SumCargosFilterByMonth(data, cuenta.MES),
                    Abono = SumAbonosFilterByMonth(data, cuenta.MES),
                    SaldoMesActual = (double?)GetFinalBalance(data, cuenta.MES, cuenta.SaldoAnterior)
                });
            }

            // Agregar detalles
            detalles.Add(new ReporteHistoricoCuentasListaDetalle
            {
                Mes = cuenta.MES,
                Anio = cuenta.ANIO,
                Asiento = cuenta.NUM_POLIZA,
                Fecha = DateTimeUtils.FormatToString(cuenta.FECHA),
                Concepto = cuenta.CONCEPTO,
                TipoDocto = cuenta.TipoDocto ?? string.Empty,
                Centro = cuenta.centro_costo ?? string.Empty,
                Cargo = cuenta.CARGO,
                Abono = cuenta.ABONO,
                SaldoAnterior = cuenta.SaldoAnterior
            });
        }

        return new ReporteHistoricoCuentas
        {
            Cabecera = cabecera,
            Titulos = titulos,
            Detalles = detalles
        };
    }

    static decimal getSaldoActualForBalanceComprobacion(ReporteBalanceComprobacionResultSet cuenta)
    {
        return cuenta.Clase_saldo switch
        {
            //Las cuentas deudoras que tambien son de activo se hace: SA + CARGO - ABONO
            // Y las acreedoras se hacen: SA + ABO - CARG
            CC.SALDO_TIPO_DEUDOR => cuenta.SaldoAnterior + cuenta.Cargos - cuenta.Abonos,
            CC.SALDO_TIPO_ACREEDOR => cuenta.SaldoAnterior + cuenta.Abonos - cuenta.Cargos,
            _ => 0
        };
    }

    static decimal getSaldoDelMesForBalanceComprobacion(ReporteBalanceComprobacionResultSet cuenta)
    {
        return cuenta.Clase_saldo switch
        {
            CC.SALDO_TIPO_DEUDOR => cuenta.Cargos - cuenta.Abonos,
            CC.SALDO_TIPO_ACREEDOR => cuenta.Abonos - cuenta.Cargos,
            _ => 0
        };
    }

    private ReporteBalanceComprobacion MakeBalanceComprobacionData(List<ReporteBalanceComprobacionResultSet> data,
    string fechaInicio, string fechaFin)
    {
        List<ReporteBalanceComprobacionLista> cuentasUnificadas = new List<ReporteBalanceComprobacionLista>();


        const string subtitulo = "Balance de Comprobación";
        var day = "";
        var month = "";
        var year = "";
        var ciaName = "";

        try
        {
            // Paso 1: Obteniendo la cabecera.
            day = DateTimeUtils.GetDayFromString(fechaFin);
            month = DateTimeUtils.GetMonthFromString(fechaFin);
            year = DateTimeUtils.GetYearFromString(fechaFin);
            ciaName = data[0].NombreCia;

            // Paso 2: Agregando las cuentas.
            foreach (var cuenta in data
                         .Where(cuenta => cuenta.SaldoAnterior != 0
                                          || cuenta.Cargos != 0 || cuenta.Abonos != 0))
            {
                var cuentaLista = new ReporteBalanceComprobacionLista
                {
                    CuentaContable = FormatAccountByLevel(
                        cuenta.Cta1, cuenta.Cta2, cuenta.Cta3, cuenta.Cta4, cuenta.Cta5, cuenta.Cta6),
                    DescripEsp = cuenta.DescripEsp,
                    SaldoAnterior = cuenta.SaldoAnterior,
                    Cargos = cuenta.Cargos,
                    Abonos = cuenta.Abonos,
                    SaldoActual = getSaldoActualForBalanceComprobacion(cuenta),
                    Nivel = cuenta.Nivel,
                    GRUPO_CTA = cuenta.GRUPO_CTA,
                    Sub_Grupo = cuenta.Sub_Grupo,
                    Clase_saldo = cuenta.Clase_saldo,
                    NombreCia = cuenta.NombreCia,
                    SaldoDelMes = getSaldoDelMesForBalanceComprobacion(cuenta),
                    CTA_NIVEL = cuenta.CTA_NIVEL,
                    Cta_Catalana = cuenta.Cta_Catalana,
                    // Determina el tipo de cuenta
                    TipoCuenta = cuenta.Clase_saldo == CC.SALDO_TIPO_DEUDOR ? "Activo" : "Pasivo"
                };

                cuentasUnificadas.Add(cuentaLista);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ReportsController), nameof(MakeBalanceComprobacionData));
        }

        return new ReporteBalanceComprobacion
        {
            CiaNombre = ciaName,
            Subtitulo = subtitulo,
            Dia = day,
            Mes = month,
            Anio = year,
            CuentasUnificadas = cuentasUnificadas,

        };
    }

    private ReporteBalanceComprobacionLista CreateReporteBalanceComprobacionLista(ReporteBalanceComprobacionResultSet cuenta)
    {
        return new ReporteBalanceComprobacionLista
        {
            CuentaContable = FormatAccountByLevel(cuenta.Cta1, cuenta.Cta2, cuenta.Cta3, cuenta.Cta4, cuenta.Cta5, cuenta.Cta6),
            DescripEsp = cuenta.DescripEsp,
            SaldoAnterior = cuenta.SaldoAnterior,
            Cargos = cuenta.Cargos,
            Abonos = cuenta.Abonos,
            SaldoActual = getSaldoActualForBalanceComprobacion(cuenta),
            Nivel = cuenta.Nivel,
            GRUPO_CTA = cuenta.GRUPO_CTA,
            Sub_Grupo = cuenta.Sub_Grupo,
            Clase_saldo = cuenta.Clase_saldo,
            NombreCia = cuenta.NombreCia,
            SaldoDelMes = getSaldoDelMesForBalanceComprobacion(cuenta),
            CTA_NIVEL = cuenta.CTA_NIVEL,
            Cta_Catalana = cuenta.Cta_Catalana,
            TipoCuenta = cuenta.Clase_saldo == CC.SALDO_TIPO_DEUDOR ? "Activo" : "Pasivo"
        };
    }

    private string FormatAccountByLevel(
        int cta1, int cta2, int cta3, int cta4, int cta5, int cta6)
    {
        var separator = accountUtils.GetSeparator();
        var account = $"{accountUtils.GetFormattedByLevel(1, cta1)}" +
                      $"{accountUtils.GetFormattedByLevel(2, cta2)}" +
                      $"{accountUtils.GetFormattedByLevel(3, cta3)}" +
                      $"{accountUtils.GetFormattedByLevel(4, cta4)}" +
                      $"{accountUtils.GetFormattedByLevel(5, cta5)}" +
                      $"{accountUtils.GetFormattedByLevel(6, cta6)}";

        return account;
    }



    //--------------------------------------------------Excel------------------------------------------------------------

    public async Task<IActionResult> ExportarBalanceComprobacionExcel(string codCia, string startDate, string finishDate, string level)
    {
        // Obtener datos del repositorio
        var reportData = await reportsRepository.GetDataForBalanceComprobacion(codCia, startDate, finishDate, level);

        if (reportData == null || reportData.Count == 0)
        {
            return NotFound("No se encontraron datos para el reporte de Balance de Comprobación.");
        }

        // Filtrar registros donde SaldoAnterior, Abonos y Cargos no sean todos 0
        reportData = reportData
            .Where(cuenta => !(cuenta.SaldoAnterior == 0 && cuenta.Abonos == 0 && cuenta.Cargos == 0))
            .ToList();

        if (reportData.Count == 0)
        {
            return NotFound("No se encontraron datos válidos para exportar.");
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Balance de Comprobación");

            // Título y subtítulo
            worksheet.Cell(6, 2).Value = reportData.First().NombreCia; // Nombre de la compañía
            worksheet.Cell(6, 2).Style.Font.Bold = true;
            worksheet.Cell(6, 2).Style.Font.FontSize = 16; // Tamaño de fuente mayor para el nombre de la compañía
            worksheet.Cell(7, 2).Value = $"BALANCE DE COMPROBACIÓN AL {finishDate}";
            worksheet.Cell(7, 2).Style.Font.Bold = true;
            worksheet.Cell(8, 2).Value = "(Expresado en dólares US$)";

            worksheet.Range("B6:H6").Merge();
            worksheet.Range("B7:H7").Merge();
            worksheet.Range("B8:H8").Merge();
            worksheet.Range("B6:H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Configuración de encabezados
            worksheet.Cell(10, 2).Value = "CUENTA";
            worksheet.Cell(10, 3).Value = "DESCRIPCIÓN";
            worksheet.Cell(10, 4).Value = "SALDO ANTERIOR";
            worksheet.Cell(10, 5).Value = "CARGOS";
            worksheet.Cell(10, 6).Value = "ABONOS";
            worksheet.Cell(10, 7).Value = "SALDO DEL MES";
            worksheet.Cell(10, 8).Value = "SALDO ACTUAL";

            worksheet.Range("B10:H10").Style.Font.Bold = true;
            worksheet.Range("B10:H10").Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Range("B10:H10").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range("B10:H10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int currentRow = 11;

            // Llenado de datos
            foreach (var cuenta in reportData)
            {
                worksheet.Cell(currentRow, 2).Value = cuenta.Cta_Catalana;
                worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(currentRow, 3).Value = cuenta.DescripEsp;
                worksheet.Cell(currentRow, 4).Value = cuenta.SaldoAnterior == 0 ? "0.00" : cuenta.SaldoAnterior.ToString("#,##0.00");
                worksheet.Cell(currentRow, 5).Value = cuenta.Cargos == 0 ? "0.00" : cuenta.Cargos.ToString("#,##0.00");
                worksheet.Cell(currentRow, 6).Value = cuenta.Abonos == 0 ? "0.00" : cuenta.Abonos.ToString("#,##0.00");
                worksheet.Cell(currentRow, 7).Value = getSaldoDelMesForBalanceComprobacion(cuenta);
                worksheet.Cell(currentRow, 8).Value = cuenta.SaldoActual == 0 ? "0.00" : cuenta.SaldoActual.ToString("#,##0.00");

                worksheet.Range($"B{currentRow}:H{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range($"B{currentRow}:H{currentRow}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range($"D{currentRow}:H{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                currentRow++;
            }

            // Fila de total
            worksheet.Cell(currentRow, 2).Value = "TOTAL";
            worksheet.Range($"B{currentRow}:D{currentRow}").Merge();
            worksheet.Cell(currentRow, 5).Value = reportData.Sum(c => c.Cargos).ToString("#,##0.00");
            worksheet.Cell(currentRow, 6).Value = reportData.Sum(c => c.Abonos).ToString("#,##0.00");

            worksheet.Range($"B{currentRow}:H{currentRow}").Style.Font.Bold = true;
            worksheet.Range($"B{currentRow}:H{currentRow}").Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Range($"B{currentRow}:H{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"B{currentRow}:H{currentRow}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Autoajustar ancho de columnas
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceComprobacion.xlsx");
            }
        }
    }

    public async Task<IActionResult> ExportarEstadoResultadosExcel(string codCia, string fechaInicio, string fechaFin)
{
    // Obtener los datos del repositorio
    var reportData = await reportsRepository.GetDataForEstadoResultados(codCia, fechaInicio, fechaFin);

    // Parsear las fechas al formato deseado
    var fechaInicioDt = DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    var fechaFinDt = DateTime.ParseExact(fechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    string fechaInicioFormateada = fechaInicioDt.ToString("dd/MM/yyyy", new CultureInfo("es-ES"));
    string fechaFinFormateada = fechaFinDt.ToString("dd/MM/yyyy", new CultureInfo("es-ES"));
    string mesInicio = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fechaInicioDt.ToString("MMMM", new CultureInfo("es-ES")));

    // Calcular totales
    decimal totalIngresosVentas = reportData.Where(d => d.Cta_Catalana.StartsWith("51")).Sum(d => d.Saldo);
    decimal totalGastos = reportData.Where(d => d.Cta_Catalana.StartsWith("4")).Sum(d => d.Saldo);
    decimal resultadoBruto = totalIngresosVentas - reportData.Where(d => d.Cta_Catalana.StartsWith("42")).Sum(d => d.Saldo);
    decimal resultadoOperacion = resultadoBruto - reportData.Where(d => d.Cta_Catalana.StartsWith("44")).Sum(d => d.Saldo);
    decimal resultadoIntegralTotal = reportData.Where (d => d.Cta_Catalana.StartsWith ("5")).Sum (d => d.Saldo) - totalGastos;

        // Crear archivo Excel
        using (var workbook = new XLWorkbook ( )) {
            var worksheet = workbook.Worksheets.Add ("Estado de Resultados");

            // Encabezado
            worksheet.Cell (2, 1).Value = reportData.First ( ).Nombre_Cia;
            worksheet.Cell (2, 1).Style.Font.Bold = true;
            worksheet.Cell (2, 1).Style.Font.FontSize = 16;
            worksheet.Cell (3, 1).Value = $"Estado de Resultados del {fechaInicioFormateada} al {fechaFinFormateada}";
            worksheet.Cell (3, 1).Style.Font.Bold = true;
            worksheet.Cell (4, 1).Value = "(Expresado en dólares US$)";
            worksheet.Range ("A1:H4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range ("A2:H2").Merge ( );
            worksheet.Range ("A3:H3").Merge ( );
            worksheet.Range ("A4:H4").Merge ( );

            // Columnas principales
            worksheet.Range ("B6:D6").Merge ( ).Value = mesInicio;
            worksheet.Cell (6, 5).Value = "%";
            worksheet.Range ("F6:G6").Merge ( ).Value = "ACUMULADO";
            worksheet.Cell (6, 8).Value = "%";
            worksheet.Range ("B6:I8").Style.Font.Bold = true;
            worksheet.Range ("D6:I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range ("B6:C8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            int currentRow = 7;

            // Diccionario para grupos de cuentas
            Dictionary<string, string> titulos = new Dictionary<string, string>
            {
            { "51", "INGRESOS POR VENTAS" },
            { "42", "COSTO DE VENTA" },
            { "44", "GASTOS DE OPERACIÓN" },
            { "52", "INGRESOS POR OTRAS VENTAS" },
            { "45", "GASTOS FINANCIEROS" },
            { "46", "OTROS GASTOS" }
        };

            foreach (var titulo in titulos) {
                decimal total = reportData.Where (d => d.Cta_Catalana.StartsWith (titulo.Key)).Sum (d => d.Saldo);

                // Fila de espacio
                worksheet.Cell (currentRow, 1).Value = "";  // Fila en blanco entre meses y más/menos
                currentRow++;

                if (titulo.Key.Equals ("52")) {
                    // Agregar "más" o "menos"
                    worksheet.Cell (currentRow, 2).Value = "INGRESOS, COSTOS Y GASTOS DE NO OPERACIÓN";
                    currentRow++;
                }

                // Agregar "más" o "menos"
                worksheet.Cell (currentRow, 2).Value = titulo.Key.StartsWith ("4") ? "MENOS:" : "MÁS:";
                worksheet.Cell (currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                currentRow++;

                // Fila principal de cada grupo
                worksheet.Cell (currentRow, 2).Value = titulo.Value;
                worksheet.Cell (currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell (currentRow, 4).Value = total.ToString ("#,##0.00");
                worksheet.Cell (currentRow, 5).Value = totalIngresosVentas != 0 ? (total / totalIngresosVentas * 100).ToString ("0.00") + "%" : "0.00%";
                worksheet.Cell (currentRow, 7).Value = total.ToString ("#,##0.00");
                worksheet.Cell (currentRow, 8).Value = totalIngresosVentas != 0 ? (total / totalIngresosVentas * 100).ToString ("0.00") : "0.00";
                worksheet.Range (currentRow, 4, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range (currentRow, 4, currentRow, 8).Style.Font.Bold = true;
                currentRow++;

                // Detalles por cuenta
                foreach (var cuenta in reportData.Where (d => d.Cta_Catalana.StartsWith (titulo.Key)).ToList ( )) {
                    worksheet.Cell (currentRow, 2).Value = cuenta.Descrip_Esp;
                    worksheet.Cell (currentRow, 3).Value = cuenta.Saldo.ToString ("#,##0.00");
                    worksheet.Cell (currentRow, 6).Value = cuenta.Saldo.ToString ("#,##0.00");

                    worksheet.Range (currentRow, 3, currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    currentRow++;
                }

                // Agregar totales específicos como Resultado Bruto y Operación
                if (titulo.Key == "42") {
                    worksheet.Cell (currentRow, 2).Value = "RESULTADO BRUTO";
                    worksheet.Cell (currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell (currentRow, 4).Value = resultadoBruto.ToString ("#,##0.00");
                    worksheet.Cell (currentRow, 5).Value = totalIngresosVentas != 0 ? (resultadoBruto / totalIngresosVentas * 100).ToString ("0.00") + "%" : "0.00%";
                    worksheet.Cell (currentRow, 7).Value = resultadoBruto.ToString ("#,##0.00");
                    worksheet.Cell (currentRow, 8).Value = totalIngresosVentas != 0 ? (resultadoBruto / totalIngresosVentas * 100).ToString ("0.00") : "0.00";
                    worksheet.Range (currentRow, 3, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Range (currentRow, 2, currentRow, 8).Style.Font.Bold = true;
                    currentRow++;
                }

                if (titulo.Key == "44") {
                    worksheet.Cell (currentRow, 2).Value = "RESULTADO DE OPERACIÓN";
                    worksheet.Cell (currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell (currentRow, 4).Value = resultadoOperacion.ToString ("#,##0.00");
                    worksheet.Cell (currentRow, 5).Value = totalIngresosVentas != 0 ? (resultadoOperacion / totalIngresosVentas * 100).ToString ("0.00") + "%" : "0.00%";
                    worksheet.Cell (currentRow, 7).Value = resultadoOperacion.ToString ("#,##0.00");
                    worksheet.Cell (currentRow, 8).Value = totalIngresosVentas != 0 ? (resultadoOperacion / totalIngresosVentas * 100).ToString ("0.00") : "0.00";
                    worksheet.Range (currentRow, 3, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Range (currentRow, 2, currentRow, 8).Style.Font.Bold = true;
                    currentRow++;
                }
            }

            currentRow++;

            // Resultado Integral Total
            worksheet.Cell (currentRow, 2).Value = "RESULTADO ANTES DE IMPUESTOS";
            worksheet.Range (currentRow, 2, currentRow, 8).Style.Font.Bold = true;
            worksheet.Cell (currentRow, 4).Value = resultadoIntegralTotal.ToString ("#,##0.00");
            worksheet.Cell (currentRow, 5).Value = totalIngresosVentas != 0 ? (resultadoIntegralTotal / totalIngresosVentas * 100).ToString ("0.00") + "%" : "0.00%";
            worksheet.Cell (currentRow, 7).Value = resultadoIntegralTotal.ToString ("#,##0.00");
            worksheet.Cell (currentRow, 8).Value = totalIngresosVentas != 0 ? (resultadoIntegralTotal / totalIngresosVentas * 100).ToString ("0.00") : "0.00";
            worksheet.Range (currentRow, 3, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            currentRow++;

            // Resultado Integral Total
            worksheet.Cell (currentRow, 2).Value = "RESULTADO DEL EJERCICIO";
            worksheet.Range (currentRow, 2, currentRow, 8).Style.Font.Bold = true;
            worksheet.Cell (currentRow, 4).Value = resultadoIntegralTotal.ToString ("#,##0.00");
            worksheet.Cell (currentRow, 5).Value = totalIngresosVentas != 0 ? (resultadoIntegralTotal / totalIngresosVentas * 100).ToString ("0.00") + "%" : "0.00%";
            worksheet.Cell (currentRow, 7).Value = resultadoIntegralTotal.ToString ("#,##0.00");
            worksheet.Cell (currentRow, 8).Value = totalIngresosVentas != 0 ? (resultadoIntegralTotal / totalIngresosVentas * 100).ToString ("0.00") : "0.00";
            worksheet.Range (currentRow, 3, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            currentRow++;

            // Resultado Integral Total
            worksheet.Cell (currentRow, 2).Value = "OTROS RESULTADOS INTEGRALES";
            currentRow++;

            // Resultado Integral Total
            worksheet.Cell (currentRow, 2).Value = "RESULTADO INTEGRAL TOTAL DEL EJERCICIO";
            worksheet.Range (currentRow, 2, currentRow, 8).Style.Font.Bold = true;
            worksheet.Cell (currentRow, 4).Value = resultadoIntegralTotal.ToString ("#,##0.00");
            worksheet.Cell (currentRow, 5).Value = totalIngresosVentas != 0 ? (resultadoIntegralTotal / totalIngresosVentas * 100).ToString ("0.00") + "%" : "0.00%";
            worksheet.Cell (currentRow, 7).Value = resultadoIntegralTotal.ToString ("#,##0.00");
            worksheet.Cell (currentRow, 8).Value = totalIngresosVentas != 0 ? (resultadoIntegralTotal / totalIngresosVentas * 100).ToString ("0.00") : "0.00";
            worksheet.Range (currentRow, 3, currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            currentRow++;

            worksheet.Columns ( ).AdjustToContents ( );

            // Exportar archivo
            using (var stream = new MemoryStream ( )) {
                workbook.SaveAs (stream);
                return File (stream.ToArray ( ), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EstadoResultados.xlsx");
            }
        }
}

    public async Task<IActionResult> ExportarHistoricoCuentaExcel(
    string codCia, string cuentaContable, string startDate, string finishDate,
    int cta1, int cta2, int cta3, int cta4, int cta5, int cta6)
    {
        var reportData = await reportsRepository.GetDataForHistoricoCuenta(
            codCia, cuentaContable, startDate, finishDate, cta1, cta2, cta3, cta4, cta5, cta6);

        if (reportData == null || reportData.Count == 0)
        {
            return NotFound("No se encontraron datos para el reporte.");
        }

        var saldoInicial = reportData.FirstOrDefault()?.SaldoAnterior ?? 0;
        double saldoAcumulado = (double)saldoInicial;

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Histórico de Cuentas");

            var cabecera = reportData.FirstOrDefault();
            if (cabecera != null)
            {
                // Encabezado
                worksheet.Range("B2:I2").Merge().Value = reportData.First().NombreCia;
                worksheet.Range("B2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("B2:I2").Style.Font.Bold = true;
                worksheet.Range("B2:I2").Style.Font.FontSize = 16;

                worksheet.Range("B3:I3").Merge().Value = "(Expresado en dólares US$)";
                worksheet.Range("B3:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Dejar fila 4 vacía
                worksheet.Cell(5, 2).Value = $"Movimiento de la cuenta: {cabecera.NumeroCuenta} - {cabecera.DESCRIP_ESP}";
                worksheet.Range("B5:H5").Merge();
                worksheet.Cell(5, 2).Style.Font.SetBold();

                worksheet.Cell(6, 2).Value = $"Movimientos entre: " +
                    (DateTime.TryParse(startDate, out var fechaInicio) ? fechaInicio.ToString("dd/MM/yyyy") : "Fecha no válida") +
                    " y " +
                    (DateTime.TryParse(finishDate, out var fechaFin) ? fechaFin.ToString("dd/MM/yyyy") : "Fecha no válida");
                worksheet.Range("B6:H6").Merge();
            }

            // Cabecera de la tabla
            worksheet.Cell(7, 2).Value = "FECHA";
            worksheet.Cell(7, 3).Value = "POLIZA";
            worksheet.Cell(7, 4).Value = "TIPO TRANSACCIÓN";
            worksheet.Cell(7, 5).Value = "CENTRO DE COSTO";
            worksheet.Cell(7, 6).Value = "DETALLE";
            worksheet.Cell(7, 7).Value = "CARGOS";
            worksheet.Cell(7, 8).Value = "ABONOS";
            worksheet.Cell(7, 9).Value = "SALDO";

            worksheet.Range("B7:I7").Style.Font.Bold = true;
            worksheet.Range("B7:I7").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range("B7:I7").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range("B7:I7").Style.Fill.BackgroundColor = XLColor.LightGray;

            int currentRow = 8;

            // Saldo inicial
            worksheet.Range($"B{currentRow}:D{currentRow}").Merge().Value = cabecera.NumeroCuenta;
            worksheet.Range($"B{currentRow}:D{currentRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range($"E{currentRow}:F{currentRow}").Merge().Value = cabecera.DESCRIP_ESP;
            worksheet.Range($"E{currentRow}:F{currentRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            worksheet.Cell(currentRow, 7).Value = "Saldo Inicial:";
            worksheet.Cell(currentRow, 9).Value = saldoInicial.ToString("#,##0.00");
            worksheet.Range($"B{currentRow}:I{currentRow}").Style.Font.SetBold();

            worksheet.Range(currentRow, 2, currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(currentRow, 2, currentRow, 9).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            currentRow++;

            // Detalle de movimientos
            foreach (var detalle in reportData)
            {
                saldoAcumulado += (detalle.CARGO.GetValueOrDefault() - detalle.ABONO.GetValueOrDefault());

                worksheet.Cell(currentRow, 2).Value = detalle.FECHA.ToString("dd/MM/yyyy");
                worksheet.Cell(currentRow, 3).Value = detalle.NUM_POLIZA;
                worksheet.Cell(currentRow, 4).Value = detalle.TipoDocto;
                worksheet.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Cell(currentRow, 5).Value = detalle.centro_costo;
                worksheet.Cell(currentRow, 6).Value = detalle.CONCEPTO;
                worksheet.Cell(currentRow, 7).Value = detalle.CARGO?.ToString("#,##0.00") ?? "0.00";
                worksheet.Cell(currentRow, 8).Value = detalle.ABONO?.ToString("#,##0.00") ?? "0.00";
                worksheet.Cell(currentRow, 9).Value = saldoAcumulado.ToString("#,##0.00");

                worksheet.Range(currentRow, 2, currentRow, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(currentRow, 2, currentRow, 9).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;
            }

            // Totales
            worksheet.Cell(currentRow, 6).Value = "Totales:";
            worksheet.Cell(currentRow, 7).Value = reportData.Sum(d => d.CARGO.GetValueOrDefault()).ToString("#,##0.00");
            worksheet.Cell(currentRow, 8).Value = reportData.Sum(d => d.ABONO.GetValueOrDefault()).ToString("#,##0.00");

            worksheet.Range($"B{currentRow}:I{currentRow}").Style.Font.SetBold().Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Range($"B{currentRow}:I{currentRow}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range($"B{currentRow}:I{currentRow}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Ajustar ancho de columnas
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HistoricoCuentas.xlsx");
            }
        }
    }


    public async Task<IActionResult> ExportarBalanceGralExcel(string fecha, string codCia)
    {
        // Obtener datos del repositorio
        var reportData = await reportsRepository.GetDataForBalanceGral(fecha, codCia);

        // Convertir la fecha y formatearla
        DateTime parsedDate = DateTime.Parse(fecha);
        string formattedDate = parsedDate.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES"));

        if (reportData == null || reportData.Count == 0)
        {
            return NotFound("No se encontraron datos para el reporte de Balance General.");
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Balance General");

            // Encabezados
            worksheet.Cell(2, 2).Value = reportData.First().Nombre_Cia;
            worksheet.Cell(2, 2).Style.Font.Bold = true;
            worksheet.Cell(2, 2).Style.Font.FontSize = 16;

            // Establecer el valor en la celda
            worksheet.Cell(3, 2).Value = $"Balance General al {formattedDate}";

            worksheet.Cell(3, 2).Style.Font.Bold = true;

            worksheet.Cell(4, 2).Value = "(Expresado en dólares US$)";

            worksheet.Range("B2:F2").Merge();
            worksheet.Range("B3:F3").Merge();
            worksheet.Range("B4:F4").Merge();
            worksheet.Range("B2:F4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int currentRow = 6;
            int filaActivosFinal = 6;
            int filaPasivosFinal = 6;

            // Crear dos columnas: Izquierda (Activos) y Derecha (Pasivos y Patrimonio)
            int columnaIzquierda = 2; // Columna inicial para Activos
            int columnaDerecha = 5;   // Columna inicial para Pasivos y Patrimonio

            // Activos
            currentRow++;
            foreach (var subGrupo in new[] { "CORRIENTE", "NO CORRIENTE" })
            {
                var items = reportData.Where(x => x.grupo_cta == "ACTIVO" && x.sub_grupo == subGrupo).ToList();
                if (items.Any())
                {
                    worksheet.Cell(currentRow, columnaIzquierda).Value = $"ACTIVO - {subGrupo}";
                    worksheet.Cell(currentRow, columnaIzquierda).Style.Font.Bold = true;
                    currentRow++;

                    foreach (var item in items)
                    {
                        worksheet.Cell(currentRow, columnaIzquierda).Value = item.DESCRIP_ESP;
                        worksheet.Cell(currentRow, columnaIzquierda + 1).Value = "$ " + item.saldo?.ToString("#,##0.00") ?? "0.00";
                        worksheet.Cell(currentRow, columnaIzquierda + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        currentRow++;
                    }

                    // Subtotal
                    worksheet.Cell(currentRow, columnaIzquierda).Value = $"Subtotal: ";
                    worksheet.Cell(currentRow, columnaIzquierda + 1).Value = "$" + items.Sum(x => x.saldo)?.ToString("#,##0.00") ?? "0.00";
                    worksheet.Cell(currentRow, columnaIzquierda).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, columnaIzquierda).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(currentRow, columnaIzquierda + 1).Style.Font.Bold = true;
                    currentRow++;

                    // Agregar fila en blanco
                    currentRow++;
                }
            }
            filaActivosFinal = currentRow;

            // Pasivos y Patrimonio
            currentRow = 6;
            currentRow++;
            foreach (var grupo in new[] { "PASIVO", "PATRIMONIO" })
            {
                foreach (var subGrupo in new[] { "CORRIENTE", "NO CORRIENTE" })
                {
                    var items = reportData.Where(x => x.grupo_cta == grupo && x.sub_grupo == subGrupo).ToList();
                    if (items.Any())
                    {
                        if (grupo.Equals("PASIVO"))
                        {
                            worksheet.Cell(currentRow, columnaDerecha).Value = $"{grupo} - {subGrupo}";
                        }
                        else
                        {
                            worksheet.Cell(currentRow, columnaDerecha).Value = $"{grupo}";
                        }
                        worksheet.Cell(currentRow, columnaDerecha).Style.Font.Bold = true;
                        currentRow++;

                        foreach (var item in items)
                        {
                            worksheet.Cell(currentRow, columnaDerecha).Value = item.DESCRIP_ESP;
                            worksheet.Cell(currentRow, columnaDerecha + 1).Value = "$ " + item.saldo?.ToString("#,##0.00") ?? "0.00";
                            worksheet.Cell(currentRow, columnaDerecha + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            currentRow++;
                        }

                        // Subtotal
                        worksheet.Cell(currentRow, columnaDerecha).Value = $"Subtotal: ";
                        worksheet.Cell(currentRow, columnaDerecha + 1).Value = "$" + items.Sum(x => x.saldo)?.ToString("#,##0.00") ?? "0.00";
                        worksheet.Cell(currentRow, columnaDerecha).Style.Font.Bold = true;
                        worksheet.Cell (currentRow, columnaDerecha).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell(currentRow, columnaDerecha + 1).Style.Font.Bold = true;
                        currentRow++;

                        // Agregar fila en blanco
                        currentRow++;
                    }
                }
            }
            filaPasivosFinal = currentRow;

            // Totales generales
            int filaTotales = Math.Max(filaActivosFinal, filaPasivosFinal) + 1; // Siempre dejar una fila libre entre los datos y los totales
            worksheet.Cell(filaTotales, columnaIzquierda).Value = "Total Activos:";
            worksheet.Cell(filaTotales, columnaIzquierda + 1).Value = "$" + reportData.Where(x => x.grupo_cta == "ACTIVO").Sum(x => x.saldo)?.ToString("#,##0.00") ?? "0.00";
            worksheet.Cell(filaTotales, columnaIzquierda).Style.Font.Bold = true;
            worksheet.Cell(filaTotales, columnaIzquierda).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Cell(filaTotales, columnaIzquierda + 1).Style.Font.Bold = true;

            worksheet.Cell(filaTotales, columnaDerecha).Value = "Total Pasivos y Patrimonio:";
            worksheet.Cell(filaTotales, columnaDerecha + 1).Value = "$" + reportData.Where(x => x.grupo_cta == "PASIVO" || x.grupo_cta == "PATRIMONIO").Sum(x => x.saldo)?.ToString("#,##0.00") ?? "0.00";
            worksheet.Cell(filaTotales, columnaDerecha).Style.Font.Bold = true;
            worksheet.Cell(filaTotales, columnaDerecha).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Cell(filaTotales, columnaDerecha + 1).Style.Font.Bold = true;

            // Ajustar columnas y estilo
            worksheet.Columns().AdjustToContents();
            worksheet.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BalanceGeneral.xlsx");
            }
        }
    }

    public async Task<IActionResult> ExportarPolizaDiarioOMayorExcel(string codCia, string tipoDocto, int numPoliza, int periodo)
    {
        var reportData = await reportsRepository.GetDataForPolizaDiario(codCia, tipoDocto, numPoliza, periodo);

        if (reportData == null || !reportData.Any())
        {
            return NotFound("No se encontraron datos para este reporte");
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Comprobante Diario");

            // Encabezado de la compañía
            worksheet.Range("B3:E3").Merge().Value = reportData.First().NombreCompania;
            worksheet.Range("B3:E3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("B3:E3").Style.Font.Bold = true;
            worksheet.Range("B3:E3").Style.Font.FontSize = 16;


            // Información detallada del reporte
            worksheet.Cell(6, 2).Value = numPoliza;
            worksheet.Cell(6, 2).Style.Font.Bold = true;
            worksheet.Cell(6, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Range("C6:D6").Merge().Value = reportData.First().DescripcionTipoDocumento;
            worksheet.Range("C6:D6").Merge().Style.Font.Bold = true;
            worksheet.Range("C6:D6").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(6, 5).Value = reportData.First().Fecha_Poliza?.ToString("dd/MM/yyyy");

            worksheet.Range("B8:C8").Merge().Value = reportData.First().ConceptoEncabezado;
            worksheet.Range("D8:F8").Merge().Value = "(Expresado en dólares US$)";
            worksheet.Range("D8:F8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // Cabeceras de la tabla
            worksheet.Range("B10:E10").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range("B10:E10").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(10, 2).Value = "Cuenta";
            worksheet.Cell(10, 3).Value = "Concepto";
            worksheet.Cell(10, 4).Value = "Cargos";
            worksheet.Cell(10, 5).Value = "Abonos";
            worksheet.Row(10).Style.Font.Bold = true;
            worksheet.Range("B10:E10").Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Row(10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int currentRow = 11;
            string cuentaActual = null;
            decimal subtotalCargo = 0;
            decimal subtotalAbono = 0;

            foreach (var cuenta in reportData)
            {
                if (cuenta.NombreCuenta != cuentaActual)
                {

                    worksheet.Cell(currentRow, 2).Value = cuenta.FullAccountNumber;
                    worksheet.Cell(currentRow, 3).Value = cuenta.NombreCuenta;
                    worksheet.Row(currentRow).Style.Font.Bold = true;

                    worksheet.Range(currentRow, 2, currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(currentRow, 2, currentRow, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    currentRow++;
                    cuentaActual = cuenta.NombreCuenta;
                    subtotalCargo = 0;
                    subtotalAbono = 0;
                }

                worksheet.Cell(currentRow, 2).Value = cuenta.CENTRO_COSTO;
                worksheet.Cell(currentRow, 3).Value = cuenta.ConceptoDetalle;
                worksheet.Cell(currentRow, 4).Value = (cuenta.CARGO ?? 0).ToString("#,##0.00");
                worksheet.Cell(currentRow, 5).Value = (cuenta.ABONO ?? 0).ToString("#,##0.00");

                subtotalCargo += (decimal)(cuenta.CARGO ?? 0d);
                subtotalAbono += (decimal)(cuenta.ABONO ?? 0d);

                // Aplicar bordes a cada fila de datos
                worksheet.Range(currentRow, 2, currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(currentRow, 2, currentRow, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;
            }

            worksheet.Cell(currentRow, 3).Value = "TOTAL DOCUMENTO:";
            worksheet.Cell(currentRow, 4).Value = reportData.Sum(c => c.CARGO ?? 0).ToString("#,##0.00");
            worksheet.Cell(currentRow, 5).Value = reportData.Sum(c => c.ABONO ?? 0).ToString("#,##0.00");
            worksheet.Range(currentRow, 2, currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.Row(currentRow).Style.Font.Bold = true;

            // Aplicar bordes a la fila de totales
            worksheet.Range(currentRow, 2, currentRow, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(currentRow, 2, currentRow, 5).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ComprobanteDiario.xlsx");
            }
        }
    }

}
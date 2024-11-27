using System.Diagnostics;
using System.Text.Json;
using CoreContable.Entities;
using CoreContable.Enums;
using CoreContable.Models.Dto;
using CoreContable.Models.Iterable;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CoreContable.Controllers;

public class RepositoryController(
    ILogger<RepositoryController> logger,
    ISecurityRepository securityRepository,
    IDmgNumeraRepository dmgNumeraRepository,
    IContaRepoRepository contaRepoRepository,
    IDetRepoRepository detRepoRepository,
    IDmgDoctosRepository dmgDoctosRepository,
    IDmgCieCierreRepository dmgCieCierreRepository,
    IRepositoryImportLogRepository repositoryImportLogRepository,
    IDmgCuentasRepository dmgCuentasRepository
) : CrudController
{
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_REPOSITORIO)]
    public IActionResult Index()
    {
        return View();
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_REPOSITORIO)]
    [HttpPost]
    public async Task<JsonResult> GetForDt(string? doctoType)
    {
        DataTableResultSet<List<RepositorioResultSet>>? dataTableResultSet;

        try
        {
            var currentCia = securityRepository.GetSessionCiaCode();
            // var dtParams = GetDtParamsFromQuery(Request);
            var dtParams = GetDtParams(Request);
            dataTableResultSet = await contaRepoRepository
                .GetAllBy(dataTabletDto: dtParams, codCia: currentCia, tipoDocto: doctoType);
        }
        catch (Exception e)
        {
            dataTableResultSet = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(GetForDt));
        }

        return Json(dataTableResultSet, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOneBy([FromQuery] string codCia, [FromQuery] int period,
        [FromQuery] string doctoType, [FromQuery] int numPoliza)
    {
        RepositorioResultSet? data;

        try
        {
            data = await contaRepoRepository.GetOneBy(
                codCia,
                period,
                doctoType,
                numPoliza
            );

            if (data != null)
            {
                // Getting dmgdocto to select2
                var currentCia = securityRepository.GetSessionCiaCode();
                var dmgDocto = await dmgDoctosRepository.GetOneDmgDoctoByCia(currentCia, doctoType);
                data.selTIPO_DOCTO = new Select2ResultSet
                {
                    id = dmgDocto?.TIPO_DOCTO ?? "",
                    text = dmgDocto?.DESCRIP_TIPO ?? ""
                };

                // Getting STAT_POLIZA to select2.
                data.selSTAT_POLIZA = new Select2ResultSet()
                {
                    id = data.STAT_POLIZA,
                    text = data.STAT_POLIZA == "G" ? "Grabada" : "Revisada"
                };
            }
        }
        catch (Exception e)
        {
            data = null;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(GetOneBy));
        }

        return Json(new
        {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_UPDATE}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_DELETE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdateOrDeleteOne([FromForm] RepositorioDto data)
    {
        bool result;

        try
        {
            var operation = data.OPERACION ?? "";
            var sessionUser = securityRepository.GetSessionUserName();
            var codCia = securityRepository.GetSessionCiaCode();

            // Si se esta agregando una cabecera o editando una existente se debe validar si el periodo esta abierto.
            if (operation != "ELIMINAR" && !await IsPeriodOpen(codCia, int.Parse(data.MES), int.Parse(data.PERIODO)))
            {
                return Json(new
                {
                    success = false,
                    message = "El período seleccionado no está habilitado."
                });
            }

            if (operation is "MODIFICAR" or "ELIMINAR")
            {
                // data.FECHA_CAMBIO = DateTime.Now;
                data.MODIFICACION_FECHA = DateTime.Now;
                data.MODIFICACION_USUARIO = sessionUser;

                result = await contaRepoRepository.ModifyOrDeleteOneBy(data);
            }
            else
            {
                data.GRABACION_FECHA = DateTime.Now;
                data.GRABACION_USUARIO = sessionUser;
                result = await contaRepoRepository.SaveOne(data);
            }
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(SaveOrUpdateOrDeleteOne));
        }

        var message = data.OPERACION switch
        {
            "MODIFICAR" => "Encabezado actualizado correctamente",
            "ELIMINAR" => "Encabezado eliminado correctamente",
            _ => "Encabezado guardado correctamente"
        };

        var errorMessage = data.OPERACION switch
        {
            "MODIFICAR" => "Ocurrió un error al actualizar el encabezado",
            "ELIMINAR" => "Ocurrió un error al eliminar el encabezado",
            _ => "Ocurrió un error al guardar el registro"
        };

        return Json(new
        {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_DET_REPOSITORIO_CAN_ADD}")]
    [HttpPost]
    public async Task<JsonResult> SaveList([FromBody] DetRepositorioListDto data)
    {
        var result = false;
        var saveRepositoryHeaderResult = SaveRepositoryHeaderResult.Error;
        var resultBody = false;
        var updateTotalResult = false;

        try
        {
            if (data.detRepoList.Count == 0)
            {
                throw new Exception("No se han enviado detalles para guardar.");
            }

            // saveRepositoryHeaderResult = await SaveRepositoryHeader(data.header);
            (saveRepositoryHeaderResult, var numPoliza) = await SaveRepositoryHeader(data.header);

            resultBody = saveRepositoryHeaderResult == SaveRepositoryHeaderResult.Success
                ? await SaveRepositoryDetails(data, int.Parse(data.header.NUM_POLIZA))
                : false;

            if (saveRepositoryHeaderResult == SaveRepositoryHeaderResult.Success && resultBody && numPoliza != null)
            {
                updateTotalResult = await UpdatePolizaTotalJob(new RepositoryIterable
                {
                    CodCia = data.header.COD_CIA,
                    Periodo = data.header.PERIODO,
                    TipoDocto = data.header.TIPO_DOCTO,
                    NumPoliza = numPoliza.Value
                });
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(SaveList));
        }

        result = saveRepositoryHeaderResult == SaveRepositoryHeaderResult.Success && resultBody;
        var message = saveRepositoryHeaderResult switch
        {
            SaveRepositoryHeaderResult.PeriodClosed => "El período seleccionado no está habilitado.",
            SaveRepositoryHeaderResult.Error => "Ocurrió un error al guardar el encabezado y detalles",
            SaveRepositoryHeaderResult.Success => resultBody
                ? "Encabezado y detalles guardados correctamente"
                : "Ocurrió un error al guardar los detalles",
            _ => "Ocurrió un error al guardar el encabezado y detalles"
        };

        return Json(new
        {
            success = result,
            message,
            data = new
            {
                codCia = data.header.COD_CIA,
                periodo = data.header.PERIODO,
                tipoDocto = data.header.TIPO_DOCTO,
                numPoliza = data.header.NUM_POLIZA
            }
        });
    }

    private Task<bool> UpdatePolizaTotalJob(RepositoryIterable data)
    {
        try
        {
            return contaRepoRepository.UpdateTotalPoliza(
                data.CodCia, int.Parse(data.Periodo),
                data.TipoDocto, data.NumPoliza);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(UpdatePolizaTotalJob));
            return Task.FromResult(false);
        }
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_CAPITALIZE}")]
    [HttpPost]
    public async Task<JsonResult> CapitalizeAccounts([FromForm] CapitalizeAccountDto data)
    {
        bool result;

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            result = await contaRepoRepository.CapitalizeAccounts(data);
            stopwatch.Stop();

            // Si el proceso dura menos de 1 segundo el dialog de loading no se oculta en el fronetend.
            var elapsed = stopwatch.ElapsedMilliseconds;
            if (elapsed < 1000)
            {
                var milliseconds = 1000 - (int)elapsed;
                Thread.Sleep(milliseconds);
            }
        }
        catch (Exception e)
        {
            result = false;
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(CapitalizeAccounts));
        }

        var message = result
            ? "Asientos mayorizados correctamente"
            : "Ocurrió un error al mayorizar los asientos";

        return Json(new
        {
            success = result, message
        });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_IMPORT_FROM_EXCEL}")]
    [HttpGet]
    public async Task<IActionResult> DownloadExcelFormat()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "static", "PLANTILLA_REPOSITORIOS.xlsx");

        var memory = new MemoryStream();
        await using (var stream = new FileStream(path, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }

        memory.Position = 0;
        return File(
            memory,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Path.GetFileName(path)
        );
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_IMPORT_FROM_EXCEL}")]
    [HttpPost]
    public async Task<JsonResult> ImportFromExcel(IFormFile file)
    {
        try
        {
            if (!IsValidExcelFormat(file.FileName))
            {
                return Json(new
                {
                    success = false,
                    message = "Formato de archivo no permitido."
                });
            }

            var rows = await GetRowsFromExcel(file);
            var groupRepos = GroupListByOrderNumber(rows);
            var currentCia = securityRepository.GetSessionCiaCode();
            var currentDate = DateTime.Now;
            var currentUser = securityRepository.GetSessionUserName();

            var successHeadersCount = 0;
            var successDetailsCount = 0;

            var totalsToBeUpdated = new List<RepositoryIterable>();

            foreach (var header in groupRepos)
            {
                var rowsByAccountingAccount = GetRowsByOrderNumber(rows, header);
                var firstRow = rowsByAccountingAccount.First();

                var headerData = new RepositorioDto
                {
                    COD_CIA = currentCia,
                    PERIODO = $"{DateTimeUtils.getYearFromStringDate(firstRow.FECHA)}",
                    TIPO_DOCTO = firstRow.TIPO_DOCTO,
                    NUM_REFERENCIA = firstRow.FECHA,
                    FECHA = firstRow.FECHA,
                    ANIO = $"{DateTimeUtils.getYearFromStringDate(firstRow.FECHA)}",
                    MES = $"{DateTimeUtils.getMonthFromStringDate(firstRow.FECHA)}",
                    CONCEPTO = firstRow.CONCEPTO,
                    STAT_POLIZA = "G",
                    GRABACION_FECHA = currentDate,
                };

                for (var i = 0; i < rowsByAccountingAccount.Count; i++)
                {
                    var currentRow = rowsByAccountingAccount[i];
                    if (i == 0)
                    {
                        var (result, numPoliza) = await SaveRepositoryHeader(headerData, true);
                        successHeadersCount += result == SaveRepositoryHeaderResult.Success ? 1 : 0;
                        if (numPoliza != null && result == SaveRepositoryHeaderResult.Success)
                        {
                            totalsToBeUpdated.Add(new RepositoryIterable
                            {
                                CodCia = currentCia,
                                Periodo = headerData.PERIODO,
                                TipoDocto = headerData.TIPO_DOCTO,
                                NumPoliza = numPoliza.Value
                            });
                        }
                    }

                    var coreAccountNumber = await dmgCuentasRepository
                        .GetCoreContableAccountFromCatalanaAccount(currentCia, currentRow.CUENTA_CONTABLE);

                    var detailData = new DetRepositorioDto
                    {
                        det_COD_CIA = currentCia,
                        det_PERIODO = int.Parse(headerData.PERIODO),
                        det_TIPO_DOCTO = headerData.TIPO_DOCTO,
                        det_NUM_POLIZA = int.Parse(headerData.NUM_POLIZA),
                        det_CORRELAT = i + 1,
                        CTA_1 = GetNumberFromCuentaContable(coreAccountNumber, 0),
                        CTA_2 = GetNumberFromCuentaContable(coreAccountNumber, 1),
                        CTA_3 = GetNumberFromCuentaContable(coreAccountNumber, 2),
                        CTA_4 = GetNumberFromCuentaContable(coreAccountNumber, 3),
                        CTA_5 = GetNumberFromCuentaContable(coreAccountNumber, 4),
                        CTA_6 = GetNumberFromCuentaContable(coreAccountNumber, 5),
                        det_CONCEPTO = currentRow.CONCEPTO_DETALLE,
                        CARGO = currentRow.CARGO == "" ? 0 : double.Parse(currentRow.CARGO),
                        ABONO = currentRow.ABONO == "" ? 0 : double.Parse(currentRow.ABONO),
                        CENTRO_COSTO = currentRow.CENTRO_COSTO,
                        GRABACION_FECHA = currentDate,
                        GRABACION_USUARIO = currentUser
                    };

                    successDetailsCount += await detRepoRepository.SaveOne(detailData) ? 1 : 0;
                }
            }

            foreach (var total in totalsToBeUpdated)
            {
                await UpdatePolizaTotalJob(total);
            }

            return Json(new
            {
                success = true,
                message = "Registros procesados correctamente.",
                data = new
                {
                    totalHeaders = groupRepos.Count,
                    headersSaved = successHeadersCount,
                    totalDetails = rows.Count,
                    detailsSaved = successDetailsCount
                }
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(ImportFromExcel));

            return Json(new
            {
                success = false,
                message = "Ocurrió un error al procesar el archivo."
            });
        }
    }

    private async Task<bool> IsPeriodOpen(string codCia, int month, int year)
    {
        var dmgCieCierre = await dmgCieCierreRepository
            .GetOneBy(codCia, year, month);

        if (dmgCieCierre == null)
        {
            return false;
        }

        return dmgCieCierre.CIE_ESTADO == "A";
    }

    private async Task<(SaveRepositoryHeaderResult, int?)> SaveRepositoryHeader(RepositorioDto data, bool isImport = false)
    {
        var result = SaveRepositoryHeaderResult.Error;
        var sessionUser = securityRepository.GetSessionUserName();
        var codCia = securityRepository.GetSessionCiaCode();
        var numPoliza = 0;

        try
        {
            // PASO 0: Validar si el periodo esta abierto.
            if (!await IsPeriodOpen(codCia, int.Parse(data.MES), int.Parse(data.PERIODO)))
            {
                return (SaveRepositoryHeaderResult.PeriodClosed, null);
            }

            // PASO 1: Generar el numero de la nueva poliza:
            numPoliza = await dmgNumeraRepository.GenerateNumPoliza(
                data.COD_CIA,
                data.TIPO_DOCTO,
                int.Parse(data.PERIODO),
                int.Parse(data.MES)
            );

            if (numPoliza != 0)
            {
                data.NUM_POLIZA = $"{numPoliza}";
                // PASO 2: Guardar el encabezado de repositorio con el numero de poliza generado en el paso anterior.
                data.GRABACION_FECHA = DateTime.Now;
                data.GRABACION_USUARIO = sessionUser;
                data.FECHA_CAMBIO = data.FECHA;
                result = await contaRepoRepository.SaveOne(data)
                    ? SaveRepositoryHeaderResult.Success
                    : SaveRepositoryHeaderResult.Error;
            }

            if (result == SaveRepositoryHeaderResult.Success && isImport)
            {
                await LogImportResult(int.Parse(data.NUM_POLIZA), data.TIPO_DOCTO, data.CONCEPTO);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(SaveRepositoryHeader));
            return (SaveRepositoryHeaderResult.Error, null);
        }

        return (result, numPoliza);
    }

    private async Task<bool> SaveRepositoryDetails(DetRepositorioListDto data, int numPoliza)
    {
        var result = false;
        var detailsSaved = 0;
        var currentDate = DateTime.Now;
        var currentUser = securityRepository.GetSessionUserName();
        var index = 1;

        foreach (var detalle in data.detRepoList)
        {
            detalle.det_CORRELAT = index++;
            detalle.det_COD_CIA = data.header.COD_CIA;
            detalle.det_PERIODO = int.Parse(data.header.PERIODO);
            detalle.det_TIPO_DOCTO = data.header.TIPO_DOCTO;
            detalle.det_NUM_POLIZA = numPoliza;
            detalle.GRABACION_FECHA = currentDate;
            detalle.GRABACION_USUARIO = currentUser;
            var resultBodyRow = await detRepoRepository.SaveOne(detalle);
            detailsSaved += resultBodyRow ? 1 : 0;
        }

        result = detailsSaved == data.detRepoList.Count;
        return result;
    }

    ///
    /// .xlsx: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
    /// .xls: application/vnd.ms-excel
    /// .xlsm: application/vnd.ms-excel.sheet.macroEnabled.12
    /// .xlsb: application/vnd.ms-excel.sheet.binary.macroEnabled.12
    /// .csv: text/csv
    ///
    public bool IsValidExcelFormat(string fileName)
    {
        // var validFormats = new List<string> { ".xlsx", ".xls", ".xlsm", ".xlsb", ".csv" };
        var validFormats = new List<string> { ".xlsx", ".xls" };
        var fileExtension = Path.GetExtension(fileName);
        return validFormats.Contains(fileExtension);
    }

    private List<string> GroupListByOrderNumber(List<RepositoryFromCsvDto> rows)
    {
        try
        {
            return rows
                .GroupBy(x => x.NUMERO)
                .Select(x => x.Key)
                .Where(x => x != "")
                .ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(GroupListByOrderNumber));
            return [];
        }
    }

    private List<RepositoryFromCsvDto> GetRowsByOrderNumber(List<RepositoryFromCsvDto> rows, string orderNumber)
    {
        try
        {
            return rows
                .Where(x => x.NUMERO == orderNumber)
                .ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(GetRowsByOrderNumber));
            return [];
        }
    }

    private string? SafeGetCellStringValue(IRow? row, int index, CellType cellType = CellType.String)
    {
        var cell = row.GetCell(index);
        if (cell == null) return null;
        try
        {
            return cell.CellType switch
            {
                CellType.String => cell.StringCellValue,
                CellType.Numeric => cell.NumericCellValue.ToString(),
                _ => cell.StringCellValue
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(SafeGetCellStringValue));
            return "";
        }
    }

    private async Task<List<RepositoryFromCsvDto>> GetRowsFromExcel(IFormFile file)
    {
        try
        {
            IWorkbook workbook;
            var records = new List<RepositoryFromCsvDto>();

            await using var stream = file.OpenReadStream();
            workbook = new XSSFWorkbook(stream);
            var sheet = workbook.GetSheetAt(0);
            var rows = sheet.GetRowEnumerator();

            while (rows.MoveNext())
            {
                var row = (IRow?) rows.Current;
                if (row?.GetCell(0) == null) break;
                if (row.RowNum <= 1) continue;

                var record = new RepositoryFromCsvDto
                {
                    NUMERO = SafeGetCellStringValue(row, 0) ?? string.Empty,
                    TIPO_DOCTO = SafeGetCellStringValue(row, 1) ?? string.Empty,
                    FECHA = SafeGetCellStringValue(row, 2) ?? string.Empty,
                    CONCEPTO = SafeGetCellStringValue(row, 3) ?? string.Empty,
                    CENTRO_COSTO = SafeGetCellStringValue(row, 4) ?? string.Empty,
                    CUENTA_CONTABLE = SafeGetCellStringValue(row, 5) ?? string.Empty,
                    CONCEPTO_DETALLE = SafeGetCellStringValue(row, 6) ?? string.Empty,
                    CARGO = SafeGetCellStringValue(row, 7) ?? string.Empty,
                    ABONO = SafeGetCellStringValue(row, 8) ?? string.Empty
                };

                records.Add(record);
            }

            return records;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(GetRowsFromExcel));
            return [];
        }
    }

    private static int GetNumberFromCuentaContable(string cuentaContable, int index)
        => index is < 0 or > 5 ? 0 : int.Parse(cuentaContable.Substring(index, 1));

    private async Task LogImportResult(int numPoliza, string tipoDocto, string description)
    {
        var data = new RepositoryImportLog
        {
            CodCia = securityRepository.GetSessionCiaCode(),
            NumPoliza = numPoliza,
            TipoDocto = tipoDocto,
            Description = description,
            UploadUser = securityRepository.GetSessionUserName(),
            UploadAt = DateTime.Now,
        };
    
        try
        {
            await repositoryImportLogRepository.SaveOne(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryController), nameof(LogImportResult));
        }
    }
}
using CoreContable.Entities.FunctionResult;
using CoreContable.Models.Report;
using CoreContable.Models.ResultSet;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IReportsRepository {
    Task<List<ReportePolizaDiarioFromFuncResultSet>> GetDataForPolizaDiario (string codCia, string tipoDocto, int numPoliza,
        int periodo);

    Task<List<ReportePolizaDiarioFromFuncResultSet>> GetDataForPolizaMayor (string codCia, string tipoDocto, int numPoliza,
        int periodo);

    Task<List<ReporteHistoricoCuentaFromFuncResultSet>> GetDataForHistoricoCuenta (string codCia, string centroCosto,
        string fechaInicio, string fechaFin, int cta1, int cta2, int cta3, int cta4, int cta5, int cta6);

    Task<List<ReporteBalanceComprobacionResultSet>> GetDataForBalanceComprobacion (string codCia, string fechaInicio,
        string fechaFin, string level);
    Task<List<ReporteBalanceGralFromFunc>> GetDataForBalanceGral (string fecha, string codCia);

    Task<List<ReporteDiarioMayorFromFunc>> GetDataForDiarioMayor (
    string codCia, string centroCosto, string fechaInicio, string fechaFin, int cta1, int cta2, int cta3, int cta4, int cta5, int cta6);

    Task<List<ReporteEstadoResultadosDetalle>> GetDataForEstadoResultados(string codCia, string startDate, string endDate);


}


public class ReportsRepository(
    DbContext dbContext,
    ILogger<ReportsRepository> logger
) : IReportsRepository {
    private object tipoDocto;
    private object numPoliza;
    private object periodo;

    //--------------------------------------------------REPORTES------------------------------------------------------------

    public Task<List<ReportePolizaDiarioFromFuncResultSet>> GetDataForPolizaDiario (string codCia, string tipoDocto, int numPoliza, int periodo) {
        try {
            var result = dbContext.ReportePolizaDiarioFromFunc
                .FromSqlRaw ("SELECT * FROM CATALANA.Rpt_PolizaDiario({0}, {1}, {2}, {3})",
                    codCia, tipoDocto, numPoliza, periodo)
                .Select (entity => new ReportePolizaDiarioFromFuncResultSet {
                    COD_CIA = entity.COD_CIA,
                    NombreCompania = entity.NombreCompania,
                    DescripcionTipoDocumento = entity.DescripcionTipoDocumento,
                    CTA_1 = entity.CTA_1,
                    CTA_2 = entity.CTA_2,
                    CTA_3 = entity.CTA_3,
                    CTA_4 = entity.CTA_4,
                    CTA_5 = entity.CTA_5,
                    CTA_6 = entity.CTA_6,
                    FullAccountNumber = entity.CuentaContable,
                    NombreCuenta = entity.NombreCuenta,
                    CENTRO_COSTO = entity.CENTRO_COSTO,
                    DescripcionCentroCosto = entity.DescripcionCentroCosto,
                    NUM_POLIZA = entity.NUM_POLIZA,
                    Fecha_Poliza = entity.Fecha_Poliza,
                    ConceptoEncabezado = entity.ConceptoEncabezado,
                    ConceptoDetalle = entity.ConceptoDetalle,
                    TOTAL_POLIZA = entity.TOTAL_POLIZA,
                    STAT_POLIZA = entity.STAT_POLIZA,
                    CARGO = entity.CARGO,
                    ABONO = entity.ABONO,
                    CategoriaOrden = entity.CategoriaOrden
                })
                .ToListAsync ( );

            return Task.FromResult (result.Result
                .OrderBy (item => item.ABONO == 0 ? 0 : 1)  // Primero ABONO = 0
                .ThenBy (item => item.CARGO == 0 ? 0 : 1)   // Luego CARGO = 0
                .ThenBy (item => item.FullAccountNumber)   // Finalmente orden por número de cuenta
                .ToList ( ));
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}", nameof (ReportsRepository), nameof (GetDataForPolizaDiario));
            return Task.FromResult (new List<ReportePolizaDiarioFromFuncResultSet> ( ));
        }
    }


    public Task<List<ReportePolizaDiarioFromFuncResultSet>> GetDataForPolizaMayor ( string codCia, string tipoDocto,
    int numPoliza, int periodo ) {
        try {
            var result = dbContext.ReportePolizaMayorFromFunc
                .FromSqlRaw ("SELECT * FROM CATALANA.Rpt_PolizaMayor({0}, {1}, {2}, {3})", codCia, tipoDocto, numPoliza, periodo)
                .Select (entity => new ReportePolizaDiarioFromFuncResultSet {
                    COD_CIA = entity.COD_CIA,
                    NombreCompania = entity.NombreCompania,
                    DescripcionTipoDocumento = entity.DescripcionTipoDocumento,
                    CTA_1 = entity.CTA_1,
                    CTA_2 = entity.CTA_2,
                    CTA_3 = entity.CTA_3,
                    CTA_4 = entity.CTA_4,
                    CTA_5 = entity.CTA_5,
                    CTA_6 = entity.CTA_6,

                    FullAccountNumber = entity.CuentaContable,
                    NombreCuenta = entity.NombreCuenta,
                    CENTRO_COSTO = entity.CENTRO_COSTO,
                    DescripcionCentroCosto = entity.DescripcionCentroCosto,
                    NUM_POLIZA = entity.NUM_POLIZA,
                    Fecha_Poliza = entity.Fecha_Poliza,  
                    ConceptoEncabezado = entity.ConceptoEncabezado,
                    ConceptoDetalle = entity.ConceptoDetalle,
                    TOTAL_POLIZA = entity.TOTAL_POLIZA,
                    STAT_POLIZA = entity.STAT_POLIZA,
                    CARGO = entity.CARGO,
                    ABONO = entity.ABONO,
                    CategoriaOrden = entity.CategoriaOrden 
                    })
                .ToListAsync ( );

            return Task.FromResult (result.Result
                .OrderBy (item => item.ABONO == 0 ? 0 : 1)  // Primero ABONO = 0
                .ThenBy (item => item.CARGO == 0 ? 0 : 1)   // Luego CARGO = 0
                .ThenBy (item => item.FullAccountNumber)   // Finalmente orden por número de cuenta
                .ToList ( ));
            }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (ReportsRepository), nameof (GetDataForPolizaMayor));
            return Task.FromResult (new List<ReportePolizaDiarioFromFuncResultSet> ( ));
            }
        }

    public Task<List<ReporteHistoricoCuentaFromFuncResultSet>> GetDataForHistoricoCuenta(string codCia,
        string centroCosto, string fechaInicio, string fechaFin, int cta1, int cta2, int cta3, int cta4, int cta5, int cta6) {
        try {
            return dbContext.ReporteHistoricoCuentaFromFunc
                .FromSqlRaw(
                    "SELECT * FROM CATALANA.Rpt_HistoricoCuenta({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                    codCia, centroCosto, fechaInicio, fechaFin, cta1, cta2, cta3, cta4, cta5, cta6
                )
                .Select(entity => new ReporteHistoricoCuentaFromFuncResultSet {
                    NombreCia = entity.Nombre_Cia,
                    NUM_POLIZA = entity.NUM_POLIZA,
                    CORRELAT = entity.CORRELAT,
                    centro_costo = entity.centro_costo,
                    CTA_1 = entity.CTA_1,
                    CTA_2 = entity.CTA_2,
                    CTA_3 = entity.CTA_3,
                    CTA_4 = entity.CTA_4,
                    CTA_5 = entity.CTA_5,
                    CTA_6 = entity.CTA_6,
                    NumeroCuenta = entity.CuentaNivel,
                    DESCRIP_ESP = entity.DESCRIP_ESP,
                    FECHA = entity.FECHA,
                    TipoDocto = entity.TipoDocto,
                    MES = entity.MES,
                    ANIO = entity.ANIO,
                    CONCEPTO = entity.CONCEPTO,
                    CARGO = entity.CARGO,
                    ABONO = entity.ABONO,
                    SaldoAnterior = entity.SaldoAnterior,
                    SaldoMesActual = entity.SaldoMesActual
                })
                .OrderBy (entity => entity.FECHA)
                .ToListAsync();

        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ReportsRepository), nameof(GetDataForHistoricoCuenta));
            return Task.FromResult(new List<ReporteHistoricoCuentaFromFuncResultSet>());
        }
    }

    public Task<List<ReporteBalanceComprobacionResultSet>> GetDataForBalanceComprobacion(string codCia, string fechaInicio, string fechaFin, string level) {
        try {
            // Ejecución de la función Rpt_BalanceComprobacion
            return dbContext.ReporteBalanceComprobacionFromFunc
                .FromSqlRaw(
                    "SELECT * FROM CATALANA.Rpt_BalanceComprobacion({0}, {1}, {2}, {3})",
                    codCia, fechaInicio, fechaFin , level
                )
                .Select(entity => new ReporteBalanceComprobacionResultSet {
                    // Mapeo de los campos devueltos por la función SQL
                    CuentaContable = entity.CUENTACONTABLE,
                    CTA_NIVEL = entity.CTA_NIVEL,
                    DescripEsp = entity.DESCRIP_ESP,
                    Nivel = entity.Nivel ?? 1, // Nivel contable
                    GRUPO_CTA = entity.GRUPO_CTA,
                    Cta_Catalana = entity.Cta_Catalana,
                    Sub_Grupo = entity.Sub_Grupo,
                    Clase_saldo = entity.Clase_saldo,
                    SaldoAnterior = entity.SALDO_ANT ?? 0,
                    Cargos = entity.Cargos,
                    Abonos = entity.Abonos,
                    NombreCia = entity.Nombre_Cia
                })

                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Error en {Class}.{Method}", nameof(ReportsRepository), nameof(GetDataForBalanceComprobacion));
            return Task.FromResult(new List<ReporteBalanceComprobacionResultSet>());
        }
    }

    public async Task<List<ReporteBalanceGralFromFunc>> GetDataForBalanceGral(string fecha, string codCia) {
        try {
            var result = await dbContext.Set<ReporteBalanceGralFromFunc>()
                .FromSqlRaw(
                    "SELECT * FROM CATALANA.Rpt_Balance_Gral(@p0, @p1)",

                    fecha, codCia
                )
                .ToListAsync();

            return result;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}", nameof(ReportsRepository), nameof(GetDataForBalanceGral));
            return new List<ReporteBalanceGralFromFunc>();
        }
    }

    public async Task<List<ReporteDiarioMayorFromFunc>> GetDataForDiarioMayor (
    string codCia, string centroCosto, string fechaInicio, string fechaFin, int cta1, int cta2, int cta3, int cta4, int cta5, int cta6) {
        try {
            var result = await dbContext.ReportePolizaMayorFromFunc
                .FromSqlRaw ("SELECT * FROM CATALANA.Rpt_PolizaMayor_Aux({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                    codCia, centroCosto, fechaInicio, fechaFin, cta1, cta2, cta3, cta4, cta5, cta6)
                .Select (entity => new ReporteDiarioMayorFromFunc {
                    COD_CIA = entity.COD_CIA,
                    NombreCompania = entity.NombreCompania,
                    DescripcionTipoDocumento = entity.DescripcionTipoDocumento,
                    CTA_1 = entity.CTA_1,
                    CTA_2 = entity.CTA_2,
                    CTA_3 = entity.CTA_3,

                    CTA_4 = entity.CTA_4,
                    CTA_5 = entity.CTA_5,
                    CTA_6 = entity.CTA_6,
                    FullAccountNumber = entity.CuentaContable,
                    NombreCuenta = entity.NombreCuenta,
                    CENTRO_COSTO = entity.CENTRO_COSTO,
                    DescripcionCentroCosto = entity.DescripcionTipoDocumento,
                    NUM_POLIZA = entity.NUM_POLIZA,
                    Fecha_Poliza = entity.Fecha_Poliza,
                    ConceptoEncabezado = entity.ConceptoEncabezado,
                    ConceptoDetalle = entity.ConceptoDetalle,
                    TOTAL_POLIZA = entity.TOTAL_POLIZA,
                    TIPO_DOCTO = entity.TIPO_DOCTO,
                    STAT_POLIZA = entity.STAT_POLIZA,
                    CARGO = entity.CARGO,
                    ABONO = entity.ABONO,
                    CategoriaOrden = entity.CategoriaOrden
                })
                .ToListAsync ( );

            return result
                .OrderBy (item => item.CategoriaOrden)
                .ThenBy (item => item.NombreCuenta)
                .ToList ( );
        }
        catch (Exception e) {
            logger.LogError (e, "Error en {Class}.{Method}", nameof (ReportsRepository), nameof (GetDataForDiarioMayor));
            return new List<ReporteDiarioMayorFromFunc> ( );
        }
    }

    public async Task<List<ReporteEstadoResultadosDetalle>> GetDataForEstadoResultados (string codCia, string startDate, string endDate) {
        try {
            var data = await dbContext.ReporteEstadoResultadosFromFunc
                .FromSqlRaw ("SELECT * FROM Catalana.Rpt_Saldos_Cta_Resultados(@p0, @p1, @p2)",
                            startDate, endDate, codCia)  

                .ToListAsync ( );
            return data;
        }
        catch (Exception e) {
            logger.LogError (e, "Error al obtener datos para Estado de Resultados");
            return new List<ReporteEstadoResultadosDetalle> ( );
        }
    }

}
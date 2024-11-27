using System.Data;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IDmgPolizaRepository
{
    Task<List<DmgPolizaResultSet>> GetAllBy(string? codCia = null, int? periodo = null, string? tipoDocto = null,
        int? numPoliza = null, int? pageSize = null, int? pageNumber = null, string? searchTerm = null);

    Task<DataTableResultSet<List<DmgPolizaResultSet>>?> GetAllBy(DataTabletDto dataTabletDto, string codCia,
        int? periodo = null, string? tipoDocto = null, int? numPoliza = null, string? fechaInicio = null, string? fechaFin = null);

    Task<DmgPolizaResultSet?> GetOneBy(string? codCia, int? periodo, string? tipoDocto, int? numPoliza);

    Task<bool> UncapitalizeAccounts(CapitalizeAccountDto data);

    Task<bool> SetPrinted(string codCia, int periodo, string tipoDocto, int numPoliza);
    
    Task<int> GetAllCountByCia(string codCia);

    Task<int> GetPrintedCountByCia(string codCia, bool printed);
}

public class DmgPolizaRepository(
    DbContext dbContext,
    ILogger<DmgPolizaRepository> logger
) : IDmgPolizaRepository
{

    private string[] _columns = 
        ["RowNum", "NUM_POLIZA", "TIPO_DOCTO", "CONCEPTO", "TOTAL_POLIZA", "DiferenciaCargoAbono", "Asiento_Impreso"];

    public Task<List<DmgPolizaResultSet>> GetAllBy(string? codCia = null, int? periodo = null, string? tipoDocto = null,
        int? numPoliza = null, int? pageSize = null, int? pageNumber = null, string? searchTerm = null)
    {
        return dbContext.ObtenerDatosDmgPolizaFromFunc
            .FromSqlRaw(
                "SELECT * FROM CATALANA.ObtenerDatosdmgpoliza({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                codCia!=null ? codCia : DBNull.Value,
                periodo!=null ? periodo : DBNull.Value,
                tipoDocto!=null ? tipoDocto : DBNull.Value,
                numPoliza!=null ? numPoliza : DBNull.Value,
                pageSize!=null ? pageSize : DBNull.Value,
                pageNumber!=null ? pageNumber : DBNull.Value,
                searchTerm!=null ? searchTerm : DBNull.Value
            )
            .Select(repositorio => DmgPolizaResultSet.FuncToResultSet(repositorio))
            .ToListAsync();
    }

    public async Task<DataTableResultSet<List<DmgPolizaResultSet>>?> GetAllBy(DataTabletDto dataTabletDto,
        string codCia, int? periodo = null, string? tipoDocto = null, int? numPoliza = null, string? fechaInicio = null,
        string? fechaFin = null)
    {
        try
        {
            var efQuery = dbContext.DmgPolizaView
                .Where(entity => entity.COD_CIA == codCia);

            if (periodo != null) efQuery = efQuery.Where(entity => entity.PERIODO == periodo);
            if (tipoDocto != null) efQuery = efQuery.Where(entity => entity.TIPO_DOCTO == tipoDocto);
            if (numPoliza != null) efQuery = efQuery.Where(entity => entity.NUM_POLIZA == numPoliza);
            if (fechaInicio != null) efQuery = efQuery.Where(entity => entity.FECHA >= DateTimeUtils.ParseFromString(fechaInicio));
            if (fechaFin != null) efQuery = efQuery.Where(entity => entity.FECHA <= DateTimeUtils.ParseFromString(fechaFin));

            if (dataTabletDto.orderIndex >= 0)
            {
                var column = _columns[dataTabletDto.orderIndex];
                efQuery = dataTabletDto.orderDirection == "asc"
                    ? efQuery.OrderBy(entity => EF.Property<object>(entity, column))
                    : efQuery.OrderByDescending(entity => EF.Property<object>(entity, column));
            }

            var total = efQuery.Count();

            if (!string.IsNullOrEmpty(dataTabletDto.search))
            {
                efQuery = dataTabletDto.search.ToLower() switch
                {
                    "impreso" => efQuery.Where(entity => entity.Asiento_Impreso == "S"),
                    "no impreso" => efQuery.Where(entity => entity.Asiento_Impreso == "N"),
                    _ => efQuery.Where(entity =>
                        EF.Functions.Like(entity.COD_CIA, $"%{dataTabletDto.search}%") ||
                        EF.Functions.Like(entity.PERIODO.ToString(), $"%{dataTabletDto.search}%") ||
                        EF.Functions.Like(entity.TIPO_DOCTO, $"%{dataTabletDto.search}%") ||
                        EF.Functions.Like(entity.NUM_POLIZA.ToString(), $"%{dataTabletDto.search}%") ||
                        EF.Functions.Like(entity.NUM_REFERENCIA, $"%{dataTabletDto.search}%") ||
                        // FECHA
                        // || EF.Functions.Like(entity.ANIO.ToString(), $"%{dataTabletDto.search}%") ||
                        // EF.Functions.Like(entity.MES.ToString(), $"%{dataTabletDto.search}%") ||
                        EF.Functions.Like(entity.CONCEPTO, $"%{dataTabletDto.search}%") ||
                        EF.Functions.Like(entity.TOTAL_POLIZA.ToString(), $"%{dataTabletDto.search}%"))
                };
            }

            var totalFiltered = efQuery.Count();

            var data = await efQuery
                .Skip(dataTabletDto.start)
                .Take(dataTabletDto.length)
                .Select(entity => DmgPolizaResultSet.ViewToResultSet(entity))
                .ToListAsync();

            return new DataTableResultSet<List<DmgPolizaResultSet>>
            {
                success = true,
                message = "Access data",
                data = data,
                recordsFiltered = totalFiltered,
                draw = dataTabletDto.draw,
                recordsTotal = total
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetAllBy));
            // logger.LogInformation("Input data: {@dataTabletDto}, {@codCia}, {@periodo}, {@tipoDocto}, {@numPoliza}, {@fechaInicio}", dataTabletDto);
            return null;
        }
    }

    public Task<DmgPolizaResultSet?> GetOneBy(
        string? codCia,
        int? periodo,
        string? tipoDocto,
        int? numPoliza
    )
    {
        try
        {
            return dbContext.ObtenerDatosDmgPolizaFromFunc
                .FromSqlRaw(
                    "SELECT * FROM CATALANA.ObtenerDatosdmgpoliza({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    codCia!=null ? codCia : DBNull.Value,
                    periodo!=null ? periodo : DBNull.Value,
                    tipoDocto!=null ? tipoDocto : DBNull.Value,
                    numPoliza!=null ? numPoliza : DBNull.Value,
                    DBNull.Value,
                    DBNull.Value,
                    DBNull.Value
                )
                .Select(repositorio => DmgPolizaResultSet.FuncToResultSet(repositorio))
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetOneBy));
            return Task.FromResult<DmgPolizaResultSet?>(null);
        }
    }

    public async Task<bool> UncapitalizeAccounts(CapitalizeAccountDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"[{CC.SCHEMA}].[DesMayorizarAsiento]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.codCia==null ? DBNull.Value : data.codCia });
            command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = data.periodo==null ? DBNull.Value : data.periodo });
            command.Parameters.Add(new SqlParameter("@TIPO_DOCTO", SqlDbType.VarChar) { Value = data.tipoDocto==null ? DBNull.Value : data.tipoDocto });
            command.Parameters.Add(new SqlParameter("@NUM_POLIZA", SqlDbType.Int) { Value = data.numPoliza==null ? DBNull.Value : data.numPoliza });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(UncapitalizeAccounts));
            logger.LogInformation("Input data: {@data}", data);
            return false;
        }
    }

    public async Task<bool> SetPrinted(string codCia, int periodo, string tipoDocto, int numPoliza)
    {
        try
        {
            var dmgPoliza = await dbContext.DmgPoliza
                .Where(p => p.COD_CIA == codCia && p.PERIODO == periodo&& p.TIPO_DOCTO == tipoDocto && p.NUM_POLIZA == numPoliza)
                .FirstOrDefaultAsync();

            if (dmgPoliza == null) return false;
            dmgPoliza.Asiento_Impreso = CC.DMGPOLIZA_PRINTED;
            dbContext.DmgPoliza.Update(dmgPoliza);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(SetPrinted));
            return false;
        }
    }

    public Task<int> GetAllCountByCia(string codCia)
    {
        try
        {
            return dbContext.DmgPoliza
                .Where(p => p.COD_CIA == codCia)
                .CountAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetAllCountByCia));
            return Task.FromResult(0);
        }
    }

    public Task<int> GetPrintedCountByCia(string codCia, bool printed)
    {
        try
        {
            return dbContext.DmgPoliza
                .Where(p => p.COD_CIA == codCia 
                            && p.Asiento_Impreso == (printed ? CC.DMGPOLIZA_PRINTED : CC.DMGPOLIZA_NOT_PRINTED))
                .CountAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetPrintedCountByCia));
            return Task.FromResult(0);
        }
    }
}
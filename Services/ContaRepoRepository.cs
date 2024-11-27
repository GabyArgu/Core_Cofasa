using System.Data;
using CoreContable.Entities.Views;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IContaRepoRepository
{
    Task<List<RepositorioResultSet>> GetAllBy(string? codCia = null, int? periodo = null, string? tipoDocto = null,
        int? numPoliza = null, int? pageSize = null, int? pageNumber = null, string? searchTerm = null);

    Task<DataTableResultSet<List<RepositorioResultSet>>> GetAllBy(DataTabletDto dataTabletDto, string? codCia = null,
        int? periodo = null, string? tipoDocto = null, int? numPoliza = null);

    Task<RepositorioResultSet?> GetOneBy(string? codCia, int? periodo, string? tipoDocto, int? numPoliza);

    Task<bool> SaveOne(RepositorioDto data);

    // Task<int> ModifyOrDeleteOneBy(RepositorioDto data);

    Task<bool> ModifyOrDeleteOneBy(RepositorioDto data);

    Task<bool> CapitalizeAccounts(CapitalizeAccountDto data);

    Task<bool> UpdateTotalPoliza(string codCia, int periodo, string tipoDocto, int numPoliza);
    
    Task<int> GetAllCountByCia(string codCia);

    Task<int> GetCanBeUpperCountByCia(string codCia);

    Task<int> GetCantBeUpperCountByCia(string codCia);
}

public class ContaRepoRepository(
    DbContext dbContext,
    ILogger<ContaRepoRepository> logger,
    IDetRepoRepository detRepoRepository
) : IContaRepoRepository
{
    private string[] _columns = 
        ["RowNum", "FECHA", "NUM_POLIZA", "NOMBRE_DOCTO", "STAT_POLIZA", "CONCEPTO", "TOTAL_POLIZA", "DiferenciaCargoAbono"];

    public Task<List<RepositorioResultSet>> GetAllBy(string? codCia = null, int? periodo = null, string? tipoDocto = null,
        int? numPoliza = null, int? pageSize = null, int? pageNumber = null, string? searchTerm = null) => dbContext.ObtenerDatosRepositorioResult
        .FromSqlRaw(
            "SELECT * FROM CATALANA.ObtenerDatosRepositorio({0}, {1}, {2}, {3}, {4}, {5}, {6})",
            codCia!=null ? codCia : DBNull.Value,
            periodo!=null ? periodo : DBNull.Value,
            tipoDocto!=null ? tipoDocto : DBNull.Value,
            numPoliza!=null ? numPoliza : DBNull.Value,
            pageSize!=null ? pageSize : DBNull.Value,
            pageNumber!=null ? pageNumber : DBNull.Value,
            searchTerm!=null ? searchTerm : DBNull.Value
        )
        .Select(repositorio => new RepositorioResultSet
        {
            COD_CIA = repositorio.COD_CIA,
            PERIODO = $"{repositorio.PERIODO}",
            TIPO_DOCTO = repositorio.TIPO_DOCTO,
            NUM_POLIZA = repositorio.NUM_POLIZA,
            NUM_REFERENCIA = repositorio.NUM_REFERENCIA,
            FECHA = repositorio.FECHA,
            ANIO = $"{repositorio.ANIO}",
            MES = $"{repositorio.MES}",
            CONCEPTO = repositorio.CONCEPTO,
            TOTAL_POLIZA = repositorio.TOTAL_POLIZA,
            STAT_POLIZA = repositorio.STAT_POLIZA,
            FECHA_CAMBIO = repositorio.FECHA_CAMBIO,
            // GRABACION_USUARIO = repositorio.GRABACION_USUARIO,
            GRABACION_FECHA = repositorio.GRABACION_FECHA,
            // MODIFICACION_USUARIO = repositorio.MODIFICACION_USUARIO,
            // MODIFICACION_FECHA = repositorio.MODIFICACION_FECHA,
            DiferenciaCargoAbono = repositorio.DiferenciaCargoAbono
        })
        .ToListAsync();

    public async Task<DataTableResultSet<List<RepositorioResultSet>>> GetAllBy(DataTabletDto dataTabletDto, string? codCia = null,
        int? periodo = null, string? tipoDocto = null, int? numPoliza = null)
    {
        try
        {
            var efQuery = dbContext.RepositorioView
                .Where(entity => entity.COD_CIA == codCia);

            if (periodo != null) efQuery = efQuery.Where(entity => entity.PERIODO == periodo);
            if (tipoDocto != null) efQuery = efQuery.Where(entity => entity.TIPO_DOCTO == tipoDocto);
            if (numPoliza != null) efQuery = efQuery.Where(entity => entity.NUM_POLIZA == numPoliza);

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
                efQuery = efQuery.Where(entity =>
                    EF.Functions.Like(entity.COD_CIA, $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.PERIODO.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.TIPO_DOCTO, $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.NUM_POLIZA.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.NUM_REFERENCIA, $"%{dataTabletDto.search}%") ||
                    // FECHA
                    // || EF.Functions.Like(entity.ANIO.ToString(), $"%{dataTabletDto.search}%") ||
                    // EF.Functions.Like(entity.MES.ToString(), $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.CONCEPTO, $"%{dataTabletDto.search}%") ||
                    EF.Functions.Like(entity.TOTAL_POLIZA.ToString(), $"%{dataTabletDto.search}%"));
            }

            var totalFiltered = efQuery.Count();

            var data = await efQuery
                .Skip(dataTabletDto.start)
                .Take(dataTabletDto.length)
                .Select(entity => RepositorioResultSet.ViewToResultSet(entity))
                .ToListAsync();

            return new DataTableResultSet<List<RepositorioResultSet>>
            {
                success = true,
                message = "Access data",
                data = data,
                recordsFiltered = totalFiltered,
                draw = dataTabletDto.draw,
                recordsTotal = total
            };
        } catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetAllBy));
            return new DataTableResultSet<List<RepositorioResultSet>>
            {
                success = true,
                message = "Access data",
                data = [],
                recordsFiltered = 0,
                draw = dataTabletDto.draw,
                recordsTotal = 0
            };
        }
    }

    public Task<RepositorioResultSet?> GetOneBy(
        string? codCia,
        int? periodo,
        string? tipoDocto,
        int? numPoliza
    )
    {
        try
        {
            return dbContext.ObtenerDatosRepositorioResult
                .FromSqlRaw(
                    "SELECT * FROM CATALANA.ObtenerDatosRepositorio({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    codCia!=null ? codCia : DBNull.Value,
                    periodo!=null ? periodo : DBNull.Value,
                    tipoDocto!=null ? tipoDocto : DBNull.Value,
                    numPoliza!=null ? numPoliza : DBNull.Value,
                    DBNull.Value,
                    DBNull.Value,
                    DBNull.Value
                )
                .Select(repositorio => new RepositorioResultSet
                {
                    COD_CIA = repositorio.COD_CIA,
                    PERIODO = $"{repositorio.PERIODO}",
                    TIPO_DOCTO = repositorio.TIPO_DOCTO,
                    NUM_POLIZA = repositorio.NUM_POLIZA,
                    NUM_REFERENCIA = repositorio.NUM_REFERENCIA,
                    FECHA = repositorio.FECHA,
                    ANIO = $"{repositorio.ANIO}",
                    MES = $"{repositorio.MES}",
                    CONCEPTO = repositorio.CONCEPTO,
                    TOTAL_POLIZA = repositorio.TOTAL_POLIZA,
                    STAT_POLIZA = repositorio.STAT_POLIZA,
                    FECHA_CAMBIO = repositorio.FECHA_CAMBIO,
                    // GRABACION_USUARIO = repositorio.GRABACION_USUARIO,
                    GRABACION_FECHA = repositorio.GRABACION_FECHA,
                    // MODIFICACION_USUARIO = repositorio.MODIFICACION_USUARIO,
                    // MODIFICACION_FECHA = repositorio.MODIFICACION_FECHA,
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetOneBy));
            return Task.FromResult<RepositorioResultSet?>(null);
        }
    }

    public async Task<bool> SaveOne(RepositorioDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            // command.CommandText = $"{CC.SCHEMA}.InsertarEnRepositorio";
            command.CommandText = $"{CC.SCHEMA}.CabeceraPartida";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@p_codcia", SqlDbType.VarChar) { Value = data.COD_CIA==null ? DBNull.Value : data.COD_CIA });
            command.Parameters.Add(new SqlParameter("@p_periodo", SqlDbType.Int) { Value = data.PERIODO==null ? DBNull.Value : data.PERIODO });
            command.Parameters.Add(new SqlParameter("@p_tipo_partida", SqlDbType.VarChar) { Value = data.TIPO_DOCTO==null ? DBNull.Value : data.TIPO_DOCTO });
            command.Parameters.Add(new SqlParameter("@p_num_partida", SqlDbType.Int) { Value = data.NUM_POLIZA==null ? DBNull.Value : data.NUM_POLIZA });
            command.Parameters.Add(new SqlParameter("@p_num_referencia", SqlDbType.VarChar) { Value = data.NUM_REFERENCIA==null ? DBNull.Value : data.NUM_REFERENCIA });
            // command.Parameters.Add(new SqlParameter("@p_fecha", SqlDbType.Date) { Value = data.FECHA==null ? DBNull.Value : data.FECHA });
            command.Parameters.Add(new SqlParameter("@p_anio", SqlDbType.Int) { Value = data.ANIO==null ? DBNull.Value : data.ANIO });
            command.Parameters.Add(new SqlParameter("@p_mes", SqlDbType.Int) { Value = data.MES==null ? DBNull.Value : data.MES});
            command.Parameters.Add(new SqlParameter("@p_concepto", SqlDbType.VarChar) { Value = data.CONCEPTO==null ? DBNull.Value : data.CONCEPTO });
            command.Parameters.Add(new SqlParameter("@p_total_partida", SqlDbType.Decimal) { Value = data.TOTAL_POLIZA==null ? DBNull.Value : data.TOTAL_POLIZA });
            command.Parameters.Add(new SqlParameter("@p_stat_partida", SqlDbType.VarChar) { Value = data.STAT_POLIZA==null ? DBNull.Value : data.STAT_POLIZA });
            // command.Parameters.Add(new SqlParameter("@p_fecha_cambio", SqlDbType.Date) { Value = data.FECHA_CAMBIO==null ? DBNull.Value : data.FECHA_CAMBIO });

            command.Parameters.Add(new SqlParameter("@p_fecha", SqlDbType.Date) { Value = string.IsNullOrEmpty(data.FECHA) ? DBNull.Value : DateTimeUtils.ParseFromString(data.FECHA) });
            command.Parameters.Add(new SqlParameter("@p_fecha_cambio", SqlDbType.DateTime) { Value = data.GRABACION_FECHA==null ? DBNull.Value : data.GRABACION_FECHA });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(SaveOne));
            return false;
        }
    }

    // public Task<int> ModifyOrDeleteOneBy(RepositorioDto data)
    // {
    //     try
    //     {
    //         // TODO: LE FALTA EL TOTAL_POLIZA
    //         return dbContext.Database
    //             .ExecuteSqlRawAsync(
    //                 "EXEC dbo.ModificarOEliminarRepositorio @COD_CIA={0}, @PERIODO={1}, @TIPO_DOCTO={2}, @NUM_POLIZA={3}, @Concepto={4}, @Fecha={5}, @Estado={6}, @Operacion={7}",
    //                 data.COD_CIA, data.PERIODO, data.TIPO_DOCTO, data.NUM_POLIZA, data.CONCEPTO, data.FECHA, data.STAT_POLIZA, data.OPERACION
    //             );
    //     } catch (Exception e) {
    //         return Task.FromResult(0);
    //     }
    // }

    public async Task<bool> ModifyOrDeleteOneBy(RepositorioDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"[{CC.SCHEMA}].[ModificarOEliminarRepositorio]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.COD_CIA==null ? DBNull.Value : data.COD_CIA });
            command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = data.PERIODO==null ? DBNull.Value : data.PERIODO });
            command.Parameters.Add(new SqlParameter("@TIPO_DOCTO", SqlDbType.VarChar) { Value = data.TIPO_DOCTO==null ? DBNull.Value : data.TIPO_DOCTO });
            command.Parameters.Add(new SqlParameter("@NUM_POLIZA", SqlDbType.Int) { Value = data.NUM_POLIZA==null ? DBNull.Value : data.NUM_POLIZA });
            command.Parameters.Add(new SqlParameter("@Concepto", SqlDbType.VarChar) { Value = data.CONCEPTO==null ? DBNull.Value : data.CONCEPTO });
            // command.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.VarChar) { Value = data.FECHA==null ? DBNull.Value : data.FECHA });
            command.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.Date) { Value = string.IsNullOrEmpty(data.FECHA) ? DBNull.Value : DateTimeUtils.ParseFromString(data.FECHA) });
            command.Parameters.Add(new SqlParameter("@Estado", SqlDbType.VarChar) { Value = data.STAT_POLIZA==null ? DBNull.Value : data.STAT_POLIZA });
            command.Parameters.Add(new SqlParameter("@Operacion", SqlDbType.VarChar) { Value = data.OPERACION==null ? DBNull.Value : data.OPERACION });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(ModifyOrDeleteOneBy));
            return false;
        }
    }

    public async Task<bool> CapitalizeAccounts(CapitalizeAccountDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"[{CC.SCHEMA}].[MayorizarAsiento]";
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
                nameof(SecurityRepository), nameof(CapitalizeAccounts));
            return false;
        }
    }

    public async Task<bool> UpdateTotalPoliza(string codCia, int periodo, string tipoDocto, int numPoliza)
    {
        try
        {
            var accountsByHeader = await detRepoRepository.GetAllBy(codCia, periodo, tipoDocto, numPoliza);
            if (accountsByHeader.Count == 0) return false;

            var newTotal = accountsByHeader.Sum(account => account.ABONO) ?? 0;

            var repositoryEntity = await dbContext.Repositorio
                .Where(rep => rep.COD_CIA == codCia && rep.PERIODO == periodo
                                                    && rep.TIPO_DOCTO == tipoDocto && rep.NUM_POLIZA == numPoliza)
                .FirstOrDefaultAsync();

            if (repositoryEntity == null) return false;
            repositoryEntity.TOTAL_POLIZA = newTotal;
            dbContext.Repositorio.Update(repositoryEntity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(UpdateTotalPoliza));
            return false;
        }
    }

    public Task<int> GetAllCountByCia(string codCia)
    {
        try
        {
            return dbContext.RepositorioView
                .Where(rep => rep.COD_CIA == codCia)
                .CountAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetAllCountByCia));
            return Task.FromResult(0);
        }
    }

    public Task<int> GetCanBeUpperCountByCia(string codCia)
    {
        try {
            return dbContext.RepositorioView
                .Where(rep => rep.COD_CIA == codCia && rep.DiferenciaCargoAbono == 0)
                .CountAsync();
        } catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetCanBeUpperCountByCia));
            return Task.FromResult(0);
        }
    }

    public Task<int> GetCantBeUpperCountByCia(string codCia)
    {
        try {
            return dbContext.RepositorioView
                .Where(rep => rep.COD_CIA == codCia && rep.DiferenciaCargoAbono != 0)
                .CountAsync();
        } catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetCantBeUpperCountByCia));
            return Task.FromResult(0);
        }
    }
}
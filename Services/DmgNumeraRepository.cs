using System.Data;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IDmgNumeraRepository
{
    Task<int> GenerateNumPoliza(string codCia, string tipoDocto, int periodo, int mes);
    Task<DmgNumeraResultSet?> GetNumPolizaBy(string codCia, string tipoDocto, int periodo, int mes);
}

public class DmgNumeraRepository(
    DbContext dbContext,
    ILogger<DmgNumeraRepository> logger
) : IDmgNumeraRepository
{
    public async Task<int> GenerateNumPoliza(string codCia, string tipoDocto, int periodo, int mes)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.numerapoliza";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@pcia", SqlDbType.VarChar) { Value = codCia==null ? DBNull.Value : codCia });
            command.Parameters.Add(new SqlParameter("@ptipo_doc", SqlDbType.VarChar) { Value = tipoDocto==null ? DBNull.Value : tipoDocto });
            command.Parameters.Add(new SqlParameter("@panio", SqlDbType.Int) { Value = periodo==null ? DBNull.Value : periodo });
            command.Parameters.Add(new SqlParameter("@pmes", SqlDbType.Int) { Value = mes==null ? DBNull.Value : mes });
            command.Parameters.Add(new SqlParameter("@pnum_poliza", SqlDbType.Int) { Value = 1 });
            command.Parameters.Add(new SqlParameter("@resultado", SqlDbType.Int) { Value = 1 });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            var numera = await GetNumPolizaBy(codCia, tipoDocto, periodo, mes);
            return numera?.CONTADOR_POLIZA ?? 0;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgNumeraRepository), nameof(GenerateNumPoliza));
            return 0;
        }
    }

    public Task<DmgNumeraResultSet?> GetNumPolizaBy(string codCia, string tipoDocto, int periodo, int mes)
    {
        try
        {
            return dbContext.DmgNumera
                .Where(num => num.COD_CIA==codCia && num.TIPO_DOCTO==tipoDocto && num.ANIO==periodo && num.MES==mes)
                .Select(entity => new DmgNumeraResultSet
                {
                    COD_CIA = entity.COD_CIA,
                    TIPO_DOCTO = entity.TIPO_DOCTO,
                    ANIO = entity.ANIO,
                    MES = entity.MES,
                    CONTADOR_POLIZA = entity.CONTADOR_POLIZA
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgNumeraRepository), nameof(GetNumPolizaBy));
            return Task.FromResult<DmgNumeraResultSet?>(null);
        }
    }
}
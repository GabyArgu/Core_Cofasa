using System.Data;
using System.Data.Common;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface ICurrencyRepository
{
    Task<AcMonMoneda?> GetCurrencyById(string id);

    Task<List<CurrencyResultSet>> GetCurrencies();

    Task<CurrencyResultSet?> GetOneCurrency(string codCurrency);

    Task<bool> SaveOrUpdateCurrency(CurrencyDto data);

    Task<List<Select2ResultSet>> CallGetCurrencies();
}

public class CurrencyRepository(
    DbContext dbContext,
    ILogger<CurrencyRepository> logger
) : ICurrencyRepository
{
    public Task<AcMonMoneda?> GetCurrencyById(string id) =>
        dbContext.AcMonMoneda
            .Where(currency => currency.MonCodigo == id)
            .FirstOrDefaultAsync();

    public Task<List<CurrencyResultSet>> GetCurrencies() =>
        dbContext.AcMonMoneda
            .FromSqlRaw("SELECT * FROM CATALANA.Obtener_Monedas()")
            .Select(entity => new CurrencyResultSet
            {
                MON_CODIGO = entity.MonCodigo,
                MON_NOMBRE = entity.MonNombre,
                MON_SIGLAS = entity.MonSiglas,
                MON_SIMBOLO = entity.MonSimbolo
            })
            .ToListAsync();

    public async Task<CurrencyResultSet?> GetOneCurrency(string codCurrency)
    {
        CurrencyResultSet? data = null;
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.ConsultarMoneda";
            command.CommandType = CommandType.StoredProcedure;
            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();

            command.Parameters.Add(new SqlParameter("@MON_CODIGO", SqlDbType.VarChar) { Value = codCurrency });
            DbDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data = new CurrencyResultSet
                {
                    MON_CODIGO = Convert.ToString(reader["MON_CODIGO"]),
                    MON_NOMBRE = Convert.ToString(reader["MON_NOMBRE"]),
                    MON_SIGLAS = Convert.ToString(reader["MON_SIGLAS"]),
                    MON_SIMBOLO = Convert.ToString(reader["MON_SIMBOLO"])
                };
            
                break;
            }

            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            return data;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CurrencyRepository), nameof(GetOneCurrency));
            return null;
        }
    }

    public async Task<bool> SaveOrUpdateCurrency(CurrencyDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.InsertarOActualizarMoneda";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@MON_CODIGO", SqlDbType.VarChar) { Value = data.MON_CODIGO==null ? DBNull.Value : data.MON_CODIGO });
            command.Parameters.Add(new SqlParameter("@MON_NOMBRE", SqlDbType.VarChar) { Value = data.MON_NOMBRE==null ? DBNull.Value : data.MON_NOMBRE });
            command.Parameters.Add(new SqlParameter("@MON_SIGLAS", SqlDbType.VarChar) { Value = data.MON_SIGLAS==null ? DBNull.Value : data.MON_SIGLAS });
            command.Parameters.Add(new SqlParameter("@MON_SIMBOLO", SqlDbType.VarChar) { Value = data.MON_SIMBOLO==null ? DBNull.Value : data.MON_SIMBOLO });
            
            command.Parameters.Add(new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar) { Value = data.UsuarioCreacion==null ? DBNull.Value : data.UsuarioCreacion });
            command.Parameters.Add(new SqlParameter("@UsuarioModificacion", SqlDbType.VarChar) { Value = data.UsuarioModificacion==null ? DBNull.Value : data.UsuarioModificacion });
            command.Parameters.Add(new SqlParameter("@FechaCreacion", SqlDbType.DateTime) { Value = data.FechaCreacion==null ? DBNull.Value : data.FechaCreacion });
            command.Parameters.Add(new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = data.FechaModificacion==null ? DBNull.Value : data.FechaModificacion });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(CurrencyRepository), nameof(SaveOrUpdateCurrency));
            return false;
        }
    }
    
    public Task<List<Select2ResultSet>> CallGetCurrencies() =>
        dbContext.AcMonMoneda
            .FromSql($"SELECT * FROM CATALANA.Obtener_Monedas()")
            .Select(currency => new Select2ResultSet
            {
                id = currency.MonCodigo,
                text = $"({currency.MonSiglas}/{currency.MonSimbolo}) - {currency.MonNombre}"
            })
            .ToListAsync();
}
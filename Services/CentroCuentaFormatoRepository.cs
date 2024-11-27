using System.Data;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreContable.Services;

public interface ICentroCuentaFormatoRepository {
    Task<List<CentroCuentaFormatoResultSet>> GetAllByCia(string codCia, string codCC, string? q = null);
    Task<CentroCuentaFormatoResultSet?> GetOne(string codCia, string centroCosto, string cta1, string cta2, string cta3, string cta4, string cta5, string cta6);
    Task<bool> SaveOrUpdate(CentroCuentaFormatoDto data);
}

public class CentroCuentaFormatoRepository : ICentroCuentaFormatoRepository {
    private readonly DbContext _dbContext;
    private readonly ILogger<CentroCuentaFormatoRepository> _logger;

    public CentroCuentaFormatoRepository(DbContext dbContext, ILogger<CentroCuentaFormatoRepository> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<List<CentroCuentaFormatoResultSet>> GetAllByCia(string codCia, string codCc, string? q = null) =>
        _dbContext.Set<CentroCuentaFormatoResultSet>()
            .FromSqlRaw("SELECT * FROM CATALANA.centro_cuenta WHERE COD_CIA = {0} AND CENTRO_COSTO = {1}", codCia, codCc)
            .ToListAsync();

    public Task<CentroCuentaFormatoResultSet?> GetOne(string codCia, string centroCosto, string cta1, string cta2, string cta3, string cta4, string cta5, string cta6) {
        return _dbContext.Set<CentroCuentaFormatoResultSet>()
            .FromSqlRaw(
                "SELECT * FROM CATALANA.centro_cuenta WHERE COD_CIA = {0} AND CENTRO_COSTO = {1} AND CTA_1 = {2} AND CTA_2 = {3} AND CTA_3 = {4} AND CTA_4 = {5} AND CTA_5 = {6} AND CTA_6 = {7}",
                codCia, centroCosto, cta1, cta2, cta3, cta4, cta5, cta6
            ).FirstOrDefaultAsync();
    }

    public async Task<bool> SaveOrUpdate(CentroCuentaFormatoDto data) {
        var command = _dbContext.Database.GetDbConnection().CreateCommand();

        try {
            command.CommandText = $"{CC.SCHEMA}.InsertaoActualizaCentroCuenta";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = (object)data.COD_CIA ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CENTRO_COSTO", SqlDbType.VarChar) { Value = (object)data.CENTRO_COSTO ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CTA_1", SqlDbType.Int) { Value = (object)data.CTA_1 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CTA_2", SqlDbType.Int) { Value = (object)data.CTA_2 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CTA_3", SqlDbType.Int) { Value = (object)data.CTA_3 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CTA_4", SqlDbType.Int) { Value = (object)data.CTA_4 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CTA_5", SqlDbType.Int) { Value = (object)data.CTA_5 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CTA_6", SqlDbType.Int) { Value = (object)data.CTA_6 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@ESTADO", SqlDbType.VarChar) { Value = (object)data.ESTADO ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@USUARIO_MODIFICACION", SqlDbType.VarChar) { Value = (object)data.USUARIO_MODIFICACION ?? DBNull.Value });


            if (command.Connection?.State != ConnectionState.Open) await _dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open) await _dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await _dbContext.Database.CloseConnectionAsync();
            _logger.LogError(e, "Error en SaveOrUpdate para CentroCuentaFormato");
            return false;
        }
    }
}

using System.Data;
using CoreContable.Entities.Views;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ICentroCuentaRepository
{
    Task<List<CentroCuentaResultSet>> GetAllByCia(string codCia, string codCC);

    Task<List<Select2ResultSet>> GetForSelect2(
        string codCia, string codCC, string? query = null, int pageNumber = 1, int pageSize = 10);

    Task<CentroCuentaResultSet?> GetOne(string codCia, string codCentroCosto, string cta1, string cta2, string cta3, string cta4, string cta5, string cta6);

    Task<bool> SaveOrUpdate(CentroCuentaDto data);
}

public class CentroCuentaRepository(
    DbContext dbContext,
    ILogger<CentroCuentaRepository> logger
) : ICentroCuentaRepository
{
    public Task<List<CentroCuentaResultSet>> GetAllByCia(string codCia, string codCc) =>
        dbContext.ConsultarCentroCuentaFromFunc
            .FromSqlRaw(
                "SELECT * FROM CONTABLE.ConsultarCENTRO_CUENTA({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                codCia, codCc, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value
            )
            .Select(entity => new CentroCuentaResultSet
            {
                COD_CIA = entity.COD_CIA,
                CENTRO_COSTO = entity.CENTRO_COSTO,
                CuentasConcatenadas = entity.CuentasConcatenadas,
                CuentaContable = entity.CuentaContable,
                CTA_1 = entity.CTA_1,
                CTA_2 = entity.CTA_2,
                CTA_3 = entity.CTA_3,
                CTA_4 = entity.CTA_4,
                CTA_5 = entity.CTA_5,
                CTA_6 = entity.CTA_6,
                ESTADO = entity.ESTADO,
                FECHA_CREACION = entity.FECHA_CREACION,
                FECHA_MODIFICACION = entity.FECHA_MODIFICACION,
                USUARIO_CREACION = entity.USUARIO_CREACION,
                USUARIO_MODIFICACION = entity.USUARIO_MODIFICACION,
                Descripcion_CTA = entity.Descripcion_CTA
            })
            .ToListAsync();

    public Task<List<Select2ResultSet>> GetForSelect2(
        string codCia, string codCC, string? query = null, int pageNumber = 1, int pageSize = 10)
    {
        var efQuery = dbContext.CentroCuentaView
            .Where(entity => entity.COD_CIA == codCia && entity.CENTRO_COSTO == codCC);

        if (!query.IsNullOrEmpty())
        {
            efQuery = dbContext.CentroCuentaView
                .Where(entity => entity.COD_CIA == codCia && entity.CENTRO_COSTO == codCC)
                .Where(
                    entity => EF.Functions.Like(entity.CuentaContable, $"%{query}%")
                        || EF.Functions.Like(entity.Descripcion_CTA, $"%{query}%")
                );
        }

        var count = efQuery.Count();

        return efQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(entity => new Select2ResultSet
            {
                id = $"{entity.COD_CIA}|{entity.CENTRO_COSTO}|{entity.CTA_1}|{entity.CTA_2}|{entity.CTA_3}|{entity.CTA_4}|{entity.CTA_5}|{entity.CTA_6}",
                text = entity.Cta_CONTABLE + " - " + entity.Descripcion_CTA,
                more = pageNumber * pageSize < count
            })
            .ToListAsync();
    }

    public Task<CentroCuentaResultSet?> GetOne(
        string codCia,
        string codCentroCosto,
        string cta1,
        string cta2,
        string cta3,
        string cta4,
        string cta5,
        string cta6
    ) => dbContext.ConsultarCentroCuentaFromFunc
        .FromSqlRaw(
            "SELECT * FROM CONTABLE.ConsultarCENTRO_CUENTA({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
            codCia, codCentroCosto, cta1, cta2, cta3, cta4, cta5, cta6
        ).Select(entity => new CentroCuentaResultSet
        {
            COD_CIA = entity.COD_CIA,
            CENTRO_COSTO = entity.CENTRO_COSTO,
            CuentasConcatenadas = entity.CuentasConcatenadas,
            CuentaContable = entity.CuentaContable,
            CTA_1 = entity.CTA_1,
            CTA_2 = entity.CTA_2,
            CTA_3 = entity.CTA_3,
            CTA_4 = entity.CTA_4,
            CTA_5 = entity.CTA_5,
            CTA_6 = entity.CTA_6,
            ESTADO = entity.ESTADO,
            FECHA_CREACION = entity.FECHA_CREACION,
            FECHA_MODIFICACION = entity.FECHA_MODIFICACION,
            USUARIO_CREACION = entity.USUARIO_CREACION,
            USUARIO_MODIFICACION = entity.USUARIO_MODIFICACION,
            Descripcion_CTA = entity.Descripcion_CTA
        })
        .FirstOrDefaultAsync();

    public async Task<bool> SaveOrUpdate(CentroCuentaDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.InsertaoActualizaCentroCuenta";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.COD_CIA==null ? DBNull.Value : data.COD_CIA });
            command.Parameters.Add(new SqlParameter("@CENTRO_COSTO", SqlDbType.VarChar) { Value = data.CENTRO_COSTO });
            command.Parameters.Add(new SqlParameter("@CTA_1", SqlDbType.Int) { Value = data.CTA_1 });
            command.Parameters.Add(new SqlParameter("@CTA_2", SqlDbType.Int) { Value = data.CTA_2 });
            command.Parameters.Add(new SqlParameter("@CTA_3", SqlDbType.Int) { Value = data.CTA_3 });
            command.Parameters.Add(new SqlParameter("@CTA_4", SqlDbType.Int) { Value = data.CTA_4 });
            command.Parameters.Add(new SqlParameter("@CTA_5", SqlDbType.Int) { Value = data.CTA_5 });
            command.Parameters.Add(new SqlParameter("@CTA_6", SqlDbType.Int) { Value = data.CTA_6 });
            command.Parameters.Add(new SqlParameter("@ESTADO", SqlDbType.VarChar) { Value = data.ESTADO });
            command.Parameters.Add(new SqlParameter("@USUARIO_MODIFICACION", SqlDbType.VarChar) { Value = data.USUARIO_MODIFICACION==null ? DBNull.Value : data.USUARIO_MODIFICACION });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(SaveOrUpdate));
            return false;
        }
    }
}
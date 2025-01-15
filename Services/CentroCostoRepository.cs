using System.Data;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ICentroCostoRepository {
    Task<List<CentroCostoResultSet>> GetAllBy (string codCia, string? query);
    Task<List<Select2ResultSet>> GetForSelect2 (string codCia, string? query = null, int pageNumber = 1, int pageSize = 10);

    Task<CentroCostoResultSet?> GetOne (string codCia, string codCentroCosto);

    Task<bool> SaveOrUpdate (CentroCostoDto data);

    Task<bool> DoCopy (string origen, string destination);
}

public class CentroCostoRepository (
    DbContext dbContext,
    ILogger<CentroCostoRepository> logger
) : ICentroCostoRepository {

    public Task<List<CentroCostoResultSet>> GetAllBy (string codCia, string? query) {
        if (query.IsNullOrEmpty ( )) {
            return dbContext.CentroCosto
                    .FromSqlRaw ("SELECT * FROM CONTABLE.ConsultarCentro_Costo({0}, {1})", codCia, DBNull.Value)
                .Select (entity => new CentroCostoResultSet {
                    COD_CIA = entity.COD_CIA,
                    CENTRO_COSTO = entity.CENTRO_COSTO,
                    DESCRIPCION = entity.DESCRIPCION,
                    ACEPTA_DATOS = entity.ACEPTA_DATOS,
                    FECHA_CREACION = entity.FECHA_CREACION,
                    FECHA_MODIFICACION = entity.FECHA_MODIFICACION,
                    USUARIO_CREACION = entity.USUARIO_CREACION,
                    USUARIO_MODIFICACION = entity.USUARIO_MODIFICACION
                })
                .ToListAsync ( );
        }

        return dbContext.CentroCosto
            .Where (entity => entity.COD_CIA == codCia)
            .Where (entity => EF.Functions.Like (entity.CENTRO_COSTO, $"%{query}%")
                             || EF.Functions.Like (entity.DESCRIPCION, $"%{query}%"))
            .Select (entity => new CentroCostoResultSet {
                COD_CIA = entity.COD_CIA,
                CENTRO_COSTO = entity.CENTRO_COSTO,
                DESCRIPCION = entity.DESCRIPCION,
                ACEPTA_DATOS = entity.ACEPTA_DATOS,
                FECHA_CREACION = entity.FECHA_CREACION,
                FECHA_MODIFICACION = entity.FECHA_MODIFICACION,
                USUARIO_CREACION = entity.USUARIO_CREACION,
                USUARIO_MODIFICACION = entity.USUARIO_MODIFICACION
            })
            .ToListAsync ( );
    }

    public Task<List<Select2ResultSet>> GetForSelect2 (string codCia, string? query = null, int pageNumber = 1, int pageSize = 10) {
        IQueryable<CentroCosto> efQuery = dbContext.CentroCosto
            .Where (entity => entity.COD_CIA == codCia); // Filtrar por COD_CIA

        // Si hay una búsqueda por query, agregarlo al filtro
        if (!query.IsNullOrEmpty ( )) {
            efQuery = efQuery
                .Where (
                    entity =>
                        EF.Functions.Like (entity.CENTRO_COSTO, $"%{query}%")
                        || EF.Functions.Like (entity.DESCRIPCION, $"%{query}%")
                );
        }

        var count = efQuery.Count ( );

        return efQuery
            .Skip ((pageNumber - 1) * pageSize)
            .Take (pageSize)
            .Select (entity => new Select2ResultSet {
                id = entity.CENTRO_COSTO,
                text = $"{entity.CENTRO_COSTO} - {entity.DESCRIPCION}",
                more = pageNumber * pageSize < count
            })
            .ToListAsync ( );
    }


    public Task<CentroCostoResultSet?> GetOne (string codCia, string codCentroCosto) =>
        dbContext.CentroCosto
            .FromSqlRaw (
                "SELECT * FROM CONTABLE.ConsultarCentro_Costo({0}, {1})", codCia, codCentroCosto
            ).Select (entity => new CentroCostoResultSet {
                COD_CIA = entity.COD_CIA,
                CENTRO_COSTO = entity.CENTRO_COSTO,
                DESCRIPCION = entity.DESCRIPCION,
                ACEPTA_DATOS = entity.ACEPTA_DATOS,
                FECHA_CREACION = entity.FECHA_CREACION,
                FECHA_MODIFICACION = entity.FECHA_MODIFICACION,
                USUARIO_CREACION = entity.USUARIO_CREACION,
                USUARIO_MODIFICACION = entity.USUARIO_MODIFICACION
            })
            .FirstOrDefaultAsync ( );

    public async Task<bool> SaveOrUpdate (CentroCostoDto data) {
        var command = dbContext.Database.GetDbConnection ( ).CreateCommand ( );

        try {
            command.CommandText = $"{CC.SCHEMA}.InsertaOActualizaCentroCosto";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add (new SqlParameter ("@COD_CIA", SqlDbType.VarChar) { Value = data.COD_CIA == null ? DBNull.Value : data.COD_CIA });
            command.Parameters.Add (new SqlParameter ("@CENTRO_COSTO", SqlDbType.VarChar) { Value = data.CENTRO_COSTO == null ? DBNull.Value : data.CENTRO_COSTO });
            command.Parameters.Add (new SqlParameter ("@DESCRIPCION", SqlDbType.VarChar) { Value = data.DESCRIPCION == null ? DBNull.Value : data.DESCRIPCION });
            command.Parameters.Add (new SqlParameter ("@ACEPTA_DATOS", SqlDbType.VarChar) { Value = data.ACEPTA_DATOS == null ? "N" : data.ACEPTA_DATOS });
            command.Parameters.Add (new SqlParameter ("@USUARIO_MODIFICACION", SqlDbType.VarChar) { Value = data.USUARIO_MODIFICACION == null ? DBNull.Value : data.USUARIO_MODIFICACION });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync ( );
            await command.ExecuteNonQueryAsync ( );
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync ( );

            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync ( );
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (SecurityRepository), nameof (SaveOrUpdate));
            return false;
        }
    }

    public async Task<bool> DoCopy (string origen, string destination) {
        var command = dbContext.Database.GetDbConnection ( ).CreateCommand ( );

        try {
            command.CommandText = $"{CC.SCHEMA}.DuplicarRegistrosCentroCuenta";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add (new SqlParameter ("@CentroCostoOrigen", SqlDbType.VarChar) { Value = origen });
            command.Parameters.Add (new SqlParameter ("@CentroCostoDestino", SqlDbType.VarChar) { Value = destination });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync ( );
            await command.ExecuteNonQueryAsync ( );
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync ( );

            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync ( );
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (SecurityRepository), nameof (DoCopy));
            return false;
        }
    }
}
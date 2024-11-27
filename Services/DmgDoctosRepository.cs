using System.Data;
using System.Data.Common;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface IDmgDoctosRepository
{
    Task<List<DmgDoctosResultSet>> GetAllBy(string codCia, string query);

    Task<List<DmgDoctosResultSet>> GetDmgDoctosByCia(string codCia);

    Task<DmgDoctosResultSet?> GetOneDmgDoctoByCia(string codCia, string doctoType);

    Task<bool> SaveOrUpdateDmgDocto(DmgDoctosDto data);
    
    Task<List<Select2ResultSet>> GetForSelect2(
        string codCia, string? query = null, int pageNumber = 1, int pageSize = 10);
}

public class DmgDoctosRepository(
    DbContext dbContext,
    ILogger<DmgDoctosRepository> logger
) : IDmgDoctosRepository
{
    public Task<List<DmgDoctosResultSet>> GetAllBy(string codCia, string query)
    {
        try
        {
            if (query.IsNullOrEmpty())
            {
                return dbContext.DmgDoctos
                    .Where(entity => entity.COD_CIA == codCia)
                    .Select(entity => new DmgDoctosResultSet
                    {
                        COD_CIA = entity.COD_CIA,
                        TIPO_DOCTO = entity.TIPO_DOCTO,
                        CONTADOR_POLIZA = entity.CONTADOR_POLIZA,
                        DESCRIP_TIPO = entity.DESCRIP_TIPO,
                        PROCESO = entity.PROCESO,
                        POLIZA_MANUAL = entity.POLIZA_MANUAL,
                        UsuarioCreacion = entity.UsuarioCreacion,
                        FechaCreacion = entity.FechaCreacion,
                        UsuarioModificacion = entity.UsuarioModificacion,
                        FechaModificacion = entity.FechaModificacion
                    })
                    .ToListAsync();
            }

            return dbContext.DmgDoctos
                .Where(entity => entity.COD_CIA == codCia)
                .Where(entity => EF.Functions.Like(entity.DESCRIP_TIPO, $"%{query}%"))
                .Select(entity => new DmgDoctosResultSet
                {
                    COD_CIA = entity.COD_CIA,
                    TIPO_DOCTO = entity.TIPO_DOCTO,
                    CONTADOR_POLIZA = entity.CONTADOR_POLIZA,
                    DESCRIP_TIPO = entity.DESCRIP_TIPO,
                    PROCESO = entity.PROCESO,
                    POLIZA_MANUAL = entity.POLIZA_MANUAL,
                    UsuarioCreacion = entity.UsuarioCreacion,
                    FechaCreacion = entity.FechaCreacion,
                    UsuarioModificacion = entity.UsuarioModificacion,
                    FechaModificacion = entity.FechaModificacion
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosRepository), nameof(GetAllBy));
            return Task.FromResult(new List<DmgDoctosResultSet>());
        }
    }

    public Task<List<DmgDoctosResultSet>> GetDmgDoctosByCia(string codCia)
    {
        try
        {
            return dbContext.DmgDoctos
                .FromSqlRaw("SELECT * FROM CONTABLE.ConsultarDocumentos({0}, {1})", codCia, DBNull.Value)
                .Select(entity => new DmgDoctosResultSet()
                {
                    COD_CIA = entity.COD_CIA,
                    TIPO_DOCTO = entity.TIPO_DOCTO,
                    CONTADOR_POLIZA = entity.CONTADOR_POLIZA,
                    DESCRIP_TIPO = entity.DESCRIP_TIPO,
                    PROCESO = entity.PROCESO,
                    POLIZA_MANUAL = entity.POLIZA_MANUAL,
                    UsuarioCreacion = entity.UsuarioCreacion,
                    FechaCreacion = entity.FechaCreacion,
                    UsuarioModificacion = entity.UsuarioModificacion,
                    FechaModificacion = entity.FechaModificacion
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosRepository), nameof(GetDmgDoctosByCia));
            return Task.FromResult(new List<DmgDoctosResultSet>());
        }
    }

    public Task<DmgDoctosResultSet?> GetOneDmgDoctoByCia(string codCia, string doctoType)
    {
        try
        {
            return dbContext.DmgDoctos
                .FromSqlRaw("SELECT * FROM CONTABLE.ConsultarDocumentos({0}, {1})", codCia, doctoType)
                .Select(entity => new DmgDoctosResultSet()
                {
                    COD_CIA = entity.COD_CIA,
                    TIPO_DOCTO = entity.TIPO_DOCTO,
                    CONTADOR_POLIZA = entity.CONTADOR_POLIZA,
                    DESCRIP_TIPO = entity.DESCRIP_TIPO,
                    PROCESO = entity.PROCESO,
                    POLIZA_MANUAL = entity.POLIZA_MANUAL,
                    UsuarioCreacion = entity.UsuarioCreacion,
                    FechaCreacion = entity.FechaCreacion,
                    UsuarioModificacion = entity.UsuarioModificacion,
                    FechaModificacion = entity.FechaModificacion
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosRepository), nameof(GetOneDmgDoctoByCia));
            return Task.FromResult<DmgDoctosResultSet?>(null);
        }
    }

    public async Task<bool> SaveOrUpdateDmgDocto(DmgDoctosDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.InsertarOActualizarDocumento";
            command.CommandType = CommandType.StoredProcedure;

            command = AddParamsForSaveOrUpdate(command, data);

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosRepository), nameof(SaveOrUpdateDmgDocto));
            return false;
        }
    }

    public Task<List<Select2ResultSet>> GetForSelect2(string codCia, string? query = null, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            IQueryable<DmgDoctos> efQuery = dbContext.DmgDoctos
                .Where(entity => entity.COD_CIA == codCia);

            if (!query.IsNullOrEmpty())
            {
                efQuery = dbContext.DmgDoctos
                    .Where(
                        entity => 
                            EF.Functions.Like(entity.TIPO_DOCTO, $"%{query}%")
                            || EF.Functions.Like(entity.DESCRIP_TIPO, $"%{query}%")
                    );
            }

            var count = efQuery.Count();

            return efQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(entity => new Select2ResultSet
                {
                    id = entity.TIPO_DOCTO,
                    text = entity.DESCRIP_TIPO,
                    more = pageNumber * pageSize < count
                })
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgDoctosRepository), nameof(GetForSelect2));
            return Task.FromResult(new List<Select2ResultSet>());
        }
    }

    private DbCommand AddParamsForSaveOrUpdate(DbCommand command, DmgDoctosDto data)
    {
        command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.COD_CIA==null ? DBNull.Value : data.COD_CIA });
        command.Parameters.Add(new SqlParameter("@TIPO_DOCTO", SqlDbType.VarChar) { Value = data.TIPO_DOCTO==null ? DBNull.Value : data.TIPO_DOCTO });
        command.Parameters.Add(new SqlParameter("@CONTADOR_POLIZA", SqlDbType.Int) { Value = data.CONTADOR_POLIZA==null ? DBNull.Value : data.CONTADOR_POLIZA });
        command.Parameters.Add(new SqlParameter("@DESCRIP_TIPO", SqlDbType.VarChar) { Value = data.DESCRIP_TIPO==null ? DBNull.Value : data.DESCRIP_TIPO });
        command.Parameters.Add(new SqlParameter("@PROCESO", SqlDbType.VarChar) { Value = data.PROCESO==null ? DBNull.Value : data.PROCESO });
        command.Parameters.Add(new SqlParameter("@POLIZA_MANUAL", SqlDbType.VarChar) { Value = data.POLIZA_MANUAL==null ? DBNull.Value : data.POLIZA_MANUAL });
        command.Parameters.Add(new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar) { Value = data.UsuarioCreacion==null ? DBNull.Value : data.UsuarioCreacion });
        command.Parameters.Add(new SqlParameter("@FechaCreacion", SqlDbType.DateTime) { Value = data.FechaCreacion==null ? DBNull.Value : data.FechaCreacion });
        command.Parameters.Add(new SqlParameter("@UsuarioModificacion", SqlDbType.VarChar) { Value = data.UsuarioModificacion==null ? DBNull.Value : data.UsuarioModificacion });
        command.Parameters.Add(new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = data.FechaModificacion==null ? DBNull.Value : data.FechaModificacion });

        return command;
    }
}
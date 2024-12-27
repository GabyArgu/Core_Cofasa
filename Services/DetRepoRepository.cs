using System.Data;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IDetRepoRepository
{
    Task<List<DetRepositorioResultSet>> GetAllBy(
        string? codCia = null,
        int? periodo = null,
        string? tipoDocto = null,
        int? numPoliza = null
    );

    Task<bool> SaveOne(DetRepositorioDto data);

    Task<bool> ModifyOrDeleteOneBy(DetRepositorioDto data);

    Task<bool> DeleteOne(string codCia, int period, string doctoType, int polizeNumber, int correlative);

    Task<bool> UpdateOne(DetRepositorioDto data);
}

public class DetRepoRepository(
    DbContext dbContext,
    ILogger<DetRepoRepository> logger
) : IDetRepoRepository
{
    public Task<List<DetRepositorioResultSet>> GetAllBy(
        string? codCia = null,
        int? periodo = null,
        string? tipoDocto = null,
        int? numPoliza = null
    )
    {
        return dbContext.DetRepositorioFromFuncForDt
            .FromSqlRaw(
                "SELECT * FROM CONTABLE.ObtenerDetallesRepositorio({0}, {1}, {2}, {3}, {4})",
                codCia!=null ? codCia : DBNull.Value,
                periodo!=null ? periodo : DBNull.Value,
                tipoDocto!=null ? tipoDocto : DBNull.Value,
                numPoliza!=null ? numPoliza : DBNull.Value,
                DBNull.Value // Correlativo
            )
            // .Select(detRepo => new DetRepositorioResultSet().ToResultSet(detRepo))
            .Select(detRepo => new DetRepositorioResultSet
            {
                COD_CIA = detRepo.COD_CIA,
                PERIODO = detRepo.PERIODO,
                TIPO_DOCTO = detRepo.TIPO_DOCTO,
                NUM_POLIZA = detRepo.NUM_POLIZA,
                Desc_CCosto = detRepo.Desc_CCosto,
                selCentroCosto = new Select2ResultSet
                {
                    id = detRepo.CENTRO_COSTO,
                    text = detRepo.Desc_CCosto
                },
                selCentroCuenta = new Select2ResultSet {
                    id = $"{detRepo.CTA_1}{detRepo.CTA_2}{detRepo.CTA_3.ToString ( ).PadLeft (2, '0')}{detRepo.CTA_4.ToString ( ).PadLeft (2, '0')}{detRepo.CTA_5.ToString ( ).PadLeft (2, '0')}{detRepo.CTA_6.ToString ( ).PadLeft (3, '0')}",
                    text = $"{detRepo.CTA_1}{detRepo.CTA_2}{detRepo.CTA_3.ToString ( ).PadLeft (2, '0')}{detRepo.CTA_4.ToString ( ).PadLeft (2, '0')}{detRepo.CTA_5.ToString ( ).PadLeft (2, '0')}{detRepo.CTA_6.ToString ( ).PadLeft (3, '0')}"
                },
                CORRELAT = detRepo.CORRELAT,
                CTA_1 = detRepo.CTA_1,
                CTA_2 = detRepo.CTA_2,
                CTA_3 = detRepo.CTA_3,
                CTA_4 = detRepo.CTA_4,
                CTA_5 = detRepo.CTA_5,
                CTA_6 = detRepo.CTA_6,
                Desc_CContable = detRepo.Desc_CContable,
                CONCEPTO = detRepo.CONCEPTO,
                CARGO = detRepo.CARGO,
                ABONO = detRepo.ABONO
            })
            .ToListAsync();
    }

    public async Task<bool> SaveOne(DetRepositorioDto data) {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.InsertarEnDetRepositorio";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.det_COD_CIA==null ? DBNull.Value : data.det_COD_CIA });
            command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = data.det_PERIODO==null ? DBNull.Value : data.det_PERIODO });
            command.Parameters.Add(new SqlParameter("@TIPO_DOCTO", SqlDbType.VarChar) { Value = data.det_TIPO_DOCTO==null ? DBNull.Value : data.det_TIPO_DOCTO });
            command.Parameters.Add(new SqlParameter("@NUM_POLIZA", SqlDbType.Int) { Value = data.det_NUM_POLIZA==null ? DBNull.Value : data.det_NUM_POLIZA });
            command.Parameters.Add(new SqlParameter("@CORRELAT", SqlDbType.Int) { Value = data.det_CORRELAT==null ? DBNull.Value : data.det_CORRELAT });
            command.Parameters.Add(new SqlParameter("@CTA_1", SqlDbType.VarChar) { Value = data.CTA_1==null ? DBNull.Value : data.CTA_1 });
            command.Parameters.Add(new SqlParameter("@CTA_2", SqlDbType.VarChar) { Value = data.CTA_2==null ? DBNull.Value : data.CTA_2 });
            command.Parameters.Add(new SqlParameter("@CTA_3", SqlDbType.VarChar) { Value = data.CTA_3==null ? DBNull.Value : data.CTA_3 });
            command.Parameters.Add(new SqlParameter("@CTA_4", SqlDbType.VarChar) { Value = data.CTA_4==null ? DBNull.Value : data.CTA_4 });
            command.Parameters.Add(new SqlParameter("@CTA_5", SqlDbType.VarChar) { Value = data.CTA_5==null ? DBNull.Value : data.CTA_5 });
            command.Parameters.Add(new SqlParameter("@CTA_6", SqlDbType.VarChar) { Value = data.CTA_6==null ? DBNull.Value : data.CTA_6 });
            command.Parameters.Add(new SqlParameter("@CONCEPTO", SqlDbType.VarChar) { Value = data.det_CONCEPTO==null ? DBNull.Value : data.det_CONCEPTO });
            command.Parameters.Add(new SqlParameter("@CARGO", SqlDbType.Decimal) { Value = data.CARGO==null ? DBNull.Value : data.CARGO });
            command.Parameters.Add(new SqlParameter("@ABONO", SqlDbType.Decimal) { Value = data.ABONO==null ? DBNull.Value : data.ABONO });
            command.Parameters.Add(new SqlParameter("@CENTRO_COSTO", SqlDbType.VarChar) { Value = data.CENTRO_COSTO==null ? DBNull.Value : data.CENTRO_COSTO });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DetRepoRepository), nameof(SaveOne));
            return false;
        }
    }

    public async Task<bool> DeleteOne(
        string codCia, int period, string doctoType, int polizeNumber, int correlative)
    {
        try
        {
            var detRepo = await dbContext.DetRepositorio
                .Where(det => det.COD_CIA == codCia && det.PERIODO == period && det.TIPO_DOCTO == doctoType 
                              && det.NUM_POLIZA == polizeNumber && det.CORRELAT == correlative)
                .FirstOrDefaultAsync();
            
            if (detRepo == null) return false;
            dbContext.Remove(detRepo);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DetRepoRepository), nameof(DeleteOne));
            return false;
        }
    }

    public async Task<bool> UpdateOne(DetRepositorioDto data)
    {
        try
        {
            var detRepoEntity = await dbContext
                .DetRepositorio
                .Where(det => det.COD_CIA == data.det_COD_CIA && det.PERIODO == data.det_PERIODO
                        && det.TIPO_DOCTO == data.det_TIPO_DOCTO && det.NUM_POLIZA == data.det_NUM_POLIZA
                        && det.CORRELAT == data.det_CORRELAT)
                .FirstOrDefaultAsync();

            if (detRepoEntity == null) return false;
            dbContext.DetRepositorio.Update(detRepoEntity.ToEntity(data));
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DetRepoRepository), nameof(UpdateOne));
            return false;
        }
    }

    public async Task<bool> ModifyOrDeleteOneBy(DetRepositorioDto data)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = $"{CC.SCHEMA}.ActualizarOEliminarDetallesRepositorio";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.det_COD_CIA==null ? DBNull.Value : data.det_COD_CIA });
            command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = data.det_PERIODO==null ? DBNull.Value : data.det_PERIODO });
            command.Parameters.Add(new SqlParameter("@TIPO_DOCTO", SqlDbType.VarChar) { Value = data.det_TIPO_DOCTO==null ? DBNull.Value : data.det_TIPO_DOCTO });
            command.Parameters.Add(new SqlParameter("@NUM_POLIZA", SqlDbType.Int) { Value = data.det_NUM_POLIZA==null ? DBNull.Value : data.det_NUM_POLIZA });
            command.Parameters.Add(new SqlParameter("@CORRELAT", SqlDbType.Int) { Value = data.det_CORRELAT==null ? DBNull.Value : data.det_CORRELAT });
            command.Parameters.Add(new SqlParameter("@Operacion", SqlDbType.VarChar) { Value = data.detOPERACION==null ? DBNull.Value : data.detOPERACION });
            command.Parameters.Add(new SqlParameter("@CTA_1", SqlDbType.VarChar) { Value = data.CTA_1==null ? DBNull.Value : data.CTA_1 });
            command.Parameters.Add(new SqlParameter("@CTA_2", SqlDbType.VarChar) { Value = data.CTA_2==null ? DBNull.Value : data.CTA_2 });
            command.Parameters.Add(new SqlParameter("@CTA_3", SqlDbType.VarChar) { Value = data.CTA_3==null ? DBNull.Value : data.CTA_3 });
            command.Parameters.Add(new SqlParameter("@CTA_4", SqlDbType.VarChar) { Value = data.CTA_4==null ? DBNull.Value : data.CTA_4 });
            command.Parameters.Add(new SqlParameter("@CTA_5", SqlDbType.VarChar) { Value = data.CTA_5==null ? DBNull.Value : data.CTA_5 });
            command.Parameters.Add(new SqlParameter("@CTA_6", SqlDbType.VarChar) { Value = data.CTA_6==null ? DBNull.Value : data.CTA_6 });
            command.Parameters.Add(new SqlParameter("@CONCEPTO", SqlDbType.VarChar) { Value = data.det_CONCEPTO==null ? DBNull.Value : data.det_CONCEPTO });
            command.Parameters.Add(new SqlParameter("@CARGO", SqlDbType.Decimal) { Value = data.CARGO==null ? DBNull.Value : data.CARGO });
            command.Parameters.Add(new SqlParameter("@ABONO", SqlDbType.Decimal) { Value = data.ABONO==null ? DBNull.Value : data.ABONO });
            command.Parameters.Add(new SqlParameter("@CENTRO_COSTO", SqlDbType.VarChar) { Value = data.CENTRO_COSTO==null ? DBNull.Value : data.CENTRO_COSTO });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DetRepoRepository), nameof(ModifyOrDeleteOneBy));
            return false;
        }
    }

}
using CoreContable.Entities;
using CoreContable.Models.ResultSet;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IDmgCieCierreRepository
{

    Task<List<DmgCieCierreResultSet>> GetDmgCieCierreForDt(string codCia, int period);
    Task<DmgCieCierreResultSet?> GetOneBy(string codCia, int year, int month);

    Task<bool> SaveList(string codCia, int period, List<int> monthRange);

    Task<bool> DeleteList(string codCia, int period, List<int> monthRange);
}

public class DmgCieCierreRepository(
    DbContext dbContext,
    ILogger<DmgCieCierreRepository> logger
) : IDmgCieCierreRepository
{
    public Task<List<DmgCieCierreResultSet>> GetDmgCieCierreForDt(string codCia, int period)
    {
        try
        {
            return dbContext.DmgCieCierre
                .Where(entity => entity.CIE_CODCIA == codCia && entity.CIE_ANIO == period)
                .Select(entity => new DmgCieCierreResultSet
                {
                    CIE_CODCIA = entity.CIE_CODCIA,
                    CIE_CODIGO = entity.CIE_CODIGO,
                    CIE_ANIO = entity.CIE_ANIO,
                    CIE_MES = entity.CIE_MES,
                    CIE_FECHA_CIERRE = entity.CIE_FECHA_CIERRE,
                    CIE_ESTADO = entity.CIE_ESTADO
                })
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCieCierreRepository), nameof(GetDmgCieCierreForDt));
            return Task.FromResult(new List<DmgCieCierreResultSet>());
        }
    }

    public async Task<DmgCieCierreResultSet?> GetOneBy(string codCia, int year, int month)
    {
        try
        {
            var efQuery = dbContext.DmgCieCierre
                .Where(entity => entity.CIE_CODCIA==codCia 
                                 && entity.CIE_CODIGO==int.Parse($"{year:D4}{month:D2}"));

            return await efQuery
                .Select(entity => new DmgCieCierreResultSet
                {
                    CIE_CODCIA = entity.CIE_CODCIA,
                    CIE_CODIGO = entity.CIE_CODIGO,
                    CIE_ANIO = entity.CIE_ANIO,
                    CIE_MES = entity.CIE_MES,
                    CIE_FECHA_CIERRE = entity.CIE_FECHA_CIERRE,
                    CIE_ESTADO = entity.CIE_ESTADO
                })
                .FirstOrDefaultAsync();
        } catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCieCierreRepository), nameof(GetOneBy));
            return null;
        }
    }

    public async Task<bool> SaveList(string codCia, int period, List<int> monthRange)
    {
        if (string.IsNullOrWhiteSpace(codCia) || period <= 0 || monthRange.Count == 0) return false;
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var cieCierreRecords = monthRange.Select(month => new DmgCieCierre
            {
                CIE_CODCIA = codCia,
                CIE_CODIGO = int.Parse($"{period:D4}{month:D2}"),
                CIE_ANIO = period,
                CIE_MES = month,
                // CIE_FECHA_CIERRE = DateTime.Now,
                CIE_ESTADO = "A"
            }).ToList();
    
            await dbContext.DmgCieCierre.AddRangeAsync(cieCierreRecords);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
    
            return true;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCieCierreRepository), nameof(SaveList));
            return false;
        }
    }

    public async Task<bool> DeleteList(string codCia, int period, List<int> monthRange)
    {
        try
        {
            if (monthRange.Count == 0) return false;
            var monthsToFiltered = await dbContext.DmgCieCierre
                .Where(entity => entity.CIE_CODCIA == codCia
                                 && entity.CIE_ANIO == period).ToListAsync();

            var monthsToDelete = monthsToFiltered
                .Where(entity => monthRange.Contains(entity.CIE_MES ?? 0)).ToList();

            dbContext.DmgCieCierre.RemoveRange(monthsToDelete);
            await dbContext.SaveChangesAsync();

            return true;
        } catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(DmgCieCierreRepository), nameof(DeleteList));
            return false;
        }
    }
}
using CoreContable.Entities;

namespace CoreContable.Services;

public interface IRepositoryImportLogRepository
{
    Task<int> SaveOne(RepositoryImportLog data);
}

public class RepositoryImportLogRepository(
    DbContext dbContext,
    ILogger<RepositoryImportLogRepository> logger
) : IRepositoryImportLogRepository
{

    public async Task<int> SaveOne(RepositoryImportLog data)
    {
        try
        {
            await dbContext.RepositoryImportLog.AddAsync(data);
            await dbContext.SaveChangesAsync();
            return data.Id;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RepositoryImportLogRepository), nameof(SaveOne));
            return 0;
        }
    }
}
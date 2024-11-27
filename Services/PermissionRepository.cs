using CoreContable.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IPermissionRepository
{
    Task<List<Permission>> GetFirstLevelPermissions();
    Task<List<Permission>> GetSecondLevelPermissionsByIdFirstLevel(int idParent);
    Task<List<Permission>> GetThirdLevelPermissionsByIdSecondLevel(int idParent);
}

public class PermissionRepository(
    DbContext dbContext,
    ILogger<PermissionRepository> logger
) : IPermissionRepository
{
    public Task<List<Permission>> GetFirstLevelPermissions()
    {
        try
        {
            return dbContext.Permission
                .Where(permission => permission.PermissionFatherId == null)
                .OrderBy(permission => permission.PriorityOrder)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(PermissionRepository), nameof(GetFirstLevelPermissions));
            return Task.FromResult(new List<Permission>());
        }
    }

    public Task<List<Permission>> GetSecondLevelPermissionsByIdFirstLevel(int idParent)
    {
        try
        {
            return dbContext.Permission
                .Where(permission => permission.PermissionFatherId==idParent && permission.VisibilityPermission)
                .OrderBy(permission => permission.PriorityOrder)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(PermissionRepository), nameof(GetSecondLevelPermissionsByIdFirstLevel));
            return Task.FromResult(new List<Permission>());
        }
    }

    public Task<List<Permission>> GetThirdLevelPermissionsByIdSecondLevel(int idParent)
    {
        try
        {
            return dbContext.Permission
                .Where(permission => permission.PermissionFatherId==idParent && !permission.VisibilityPermission)
                .OrderBy(permission => permission.PriorityOrder)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(PermissionRepository), nameof(GetThirdLevelPermissionsByIdSecondLevel));
            return Task.FromResult(new List<Permission>());
        }
    }
}
using CoreContable.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IRolePermissionRepository
{
    Task<List<RolePermission>> GetAllPermissionByRole(int roleId);
    Task Save(RolePermission entity, bool isEditing);
    Task DeleteRolePermission(int idRole,int idPermission);
}

public class RolePermissionRepository(
    DbContext dbContext,
    ILogger<RolePermissionRepository> logger
) : IRolePermissionRepository
{
    public Task<List<RolePermission>> GetAllPermissionByRole(int roleId)
    {
        try
        {
            return dbContext.RolePermission
                // .Include(entity => entity.Permission)
                .Where(entity => entity.IdRole==roleId)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolePermissionRepository), nameof(GetAllPermissionByRole));
            return Task.FromResult(new List<RolePermission>());
        }
    }

    public async Task Save(RolePermission entity, bool isEditing)
    {
        try
        {
            if (isEditing) dbContext.RolePermission.Update(entity);
            else await dbContext.RolePermission.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolePermissionRepository), nameof(Save));
        }
    }

    public async Task DeleteRolePermission(int idRole, int idPermission)
    {
        try
        {
            var rolePermission = await dbContext.RolePermission
                .Where(rp => rp.IdRole==idRole && rp.IdPermission==idPermission)
                .FirstOrDefaultAsync();

            if (rolePermission != null)
            {
                dbContext.Remove(rolePermission);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolePermissionRepository), nameof(DeleteRolePermission));
        }
    }
}
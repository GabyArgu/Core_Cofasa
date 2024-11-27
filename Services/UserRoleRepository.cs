using CoreContable.Entities;
using CoreContable.Models.ResultSet;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IUserRoleRepository {
    Task<bool> SaveUserRoles(List<int> roles, int user);
    Task<bool> DeleteUserRoles(List<int> roles, int user);
    Task<List<UserRoleResultSet>> GetUserUserRoles(int user, string codCia);
}

public class UserRoleRepository(
    DbContext dbContext,
    ILogger<UserRoleRepository> logger
) : IUserRoleRepository {
    public async Task<bool> SaveUserRoles(List<int> roles, int user) {
        try {
            await dbContext.UserRole.AddRangeAsync(
                roles.Select(roleId => new UserRole {
                    IdUser = user,
                    IdRole = roleId,
                    CreatedAt = DateTime.Now
                })
            );

            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRoleRepository), nameof(SaveUserRoles));
            return false;
        }
    }

    public async Task<bool> DeleteUserRoles(List<int> roles, int userId) {
        try {
            if (roles.Count == 0) {
                return false;
            }

            var userRolesToRemove = await dbContext.UserRole
                .Where(ur => ur.IdUser == userId && roles.Contains(ur.IdRole))
                .ToListAsync();

            if (userRolesToRemove.Count == 0) {
                return true;
            }

            dbContext.UserRole.RemoveRange(userRolesToRemove);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRoleRepository), nameof(DeleteUserRoles));
            return false;
        }
    }

    public Task<List<UserRoleResultSet>> GetUserUserRoles(int user, string codCia) {
        try {
            IQueryable<UserRole> efQuery = dbContext.UserRole
                .Include(entity => entity.Role)
                .Where(entity => entity.IdUser == user && entity.DeletedAt == null);

            if (!string.IsNullOrEmpty(codCia)) {
                efQuery = efQuery
                    .Where(entity => entity.Role.CodCia == codCia);
            }

            return efQuery
                .Select(entity => new UserRoleResultSet {
                    Id = entity.Id,
                    IdUser = entity.IdUser,
                    IdRole = entity.IdRole,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt,
                    RoleName = entity.Role.Name
                })
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRoleRepository), nameof(GetUserUserRoles));
            return Task.FromResult(new List<UserRoleResultSet>());
        }
    }
}
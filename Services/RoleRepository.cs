using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface IRoleRepository {
    Task<List<RoleResultSet>> GetAll(string cia, string query);
    Task<List<RoleResultSet>> GetAllDt(string cia);
    Task<Role?> GetById(int roleId);
    Task<RoleResultSet?> GetOneById(int roleId);
    Task<bool> DeleteOneById(int roleId);
    Task<int> SaveOrUpdate(RoleDto data);
}

public class RoleRepository(
    DbContext dbContext,
    ILogger<RoleRepository> logger
) : IRoleRepository {
    public Task<List<RoleResultSet>> GetAll(string cia, string query) {
        try {
            if (query.IsNullOrEmpty()) {
                return dbContext.Role
                    .Where(role => role.CodCia == cia && role.Active == true && role.DeletedAt == null)
                    .Select(role => new RoleResultSet {
                        Id = role.Id,
                        Name = role.Name,
                        Active = role.Active,
                        CodCia = role.CodCia,
                    })
                    .ToListAsync();
            }

            return dbContext.Role
                .Where(role => role.CodCia == cia && role.Active == true && role.DeletedAt == null)
                .Where(role => EF.Functions.Like(role.Name, $"%{query}%"))
                .Select(role => new RoleResultSet {
                    Id = role.Id,
                    Name = role.Name,
                    Active = role.Active,
                    CodCia = role.CodCia,
                })
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RoleRepository), nameof(GetAll));
            return Task.FromResult(new List<RoleResultSet>());
        }
    }

    public Task<List<RoleResultSet>> GetAllDt(string cia) {
        try {
            return dbContext.Role
                .Where(role => role.CodCia == cia && role.DeletedAt == null)
                .Select(role => new RoleResultSet {
                    Id = role.Id,
                    Name = role.Name,
                    Active = role.Active,
                    CodCia = role.CodCia,
                    CreatedAt = role.CreatedAt
                })
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RoleRepository), nameof(GetAllDt));
            return Task.FromResult(new List<RoleResultSet>());
        }
    }

    public Task<Role?> GetById(int roleId) {
        try {
            return dbContext.Role
                .Where(role => role.Id == roleId && role.Active == true && role.DeletedAt == null)
                .FirstOrDefaultAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RoleRepository), nameof(GetById));
            throw;
        }
    }

    public Task<RoleResultSet?> GetOneById(int roleId) {
        try {
            return dbContext.Role
                // .Where(role => role.Id == roleId && role.Active==true && role.DeletedAt==null)
                .Where(role => role.Id == roleId && role.DeletedAt == null)
                .Select(role => new RoleResultSet {
                    Id = role.Id,
                    Name = role.Name,
                    Active = role.Active,
                    CodCia = role.CodCia,
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RoleRepository), nameof(GetOneById));
            throw;
        }
    }

    public async Task<bool> DeleteOneById(int roleId) {
        try {
            var role = await dbContext.Role.Where(role => role.Id == roleId).FirstOrDefaultAsync();
            if (role == null) return true;

            role.DeletedAt = DateTime.Now;
            dbContext.Role.Update(role);
            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RoleRepository), nameof(DeleteOneById));
            return false;
        }
    }

    public async Task<int> SaveOrUpdate(RoleDto data) {
        try {
            Role? roleEntity;

            if (data.Id is > 0)
            {
                roleEntity = await dbContext.Role.Where(role => role.Id == data.Id).FirstOrDefaultAsync();

                if (roleEntity != null) {
                    roleEntity.Name = data.Name;
                    roleEntity.Active = data.Active;
                    roleEntity.UpdatedAt = DateTime.Now;
                    dbContext.Role.Update(roleEntity);
                }
            }
            else {
                roleEntity = new Role {
                    Name = data.Name,
                    Active = data.Active,
                    CreatedAt = DateTime.Now,
                    CodCia = data.CodCia
                };

                await dbContext.Role.AddAsync(roleEntity);
            }

            await dbContext.SaveChangesAsync();
            return roleEntity?.Id ?? 0;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RoleRepository), nameof(SaveOrUpdate));
            return 0;
        }
    }
}
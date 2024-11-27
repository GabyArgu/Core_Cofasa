using CoreContable.Entities;
using CoreContable.Models.ResultSet;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IUserCiaRepository {
    Task<bool> SaveList(List<string> cias, int userId);
    Task<bool> DeleteList(List<string> cias, int userId);
    Task<List<UserCiaResultSet>> GetUserCias(int userId);
}

public class UserCiaRepository(
    DbContext dbContext,
    ILogger<UserCiaRepository> logger
) : IUserCiaRepository {
    public async Task<bool> SaveList(List<string> cias, int userId) {
        try {
            await dbContext.UserCia.AddRangeAsync(
                cias.Select(ciaCod => new UserCia {
                    UserAppId = userId,
                    CodCia = ciaCod,
                })
            );

            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserCiaRepository), nameof(SaveList));
            return false;
        }
    }

    public async Task<bool> DeleteList(List<string> cias, int userId) {
        try {
            var userCiasToRemove = await dbContext.UserCia
                .Where(uc => cias.Contains(uc.CodCia) && uc.UserAppId == userId)
                .ToListAsync();

            if (userCiasToRemove.Count == 0) {
                return true;
            }

            dbContext.UserCia.RemoveRange(userCiasToRemove);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserCiaRepository), nameof(DeleteList));
            return false;
        }
    }

    public Task<List<UserCiaResultSet>> GetUserCias(int userId) {
        try {
            return dbContext.UserCia
                .Include(userRole => userRole.Cia)
                .Where(userCia => userCia.UserAppId == userId)
                .Select(userCia => new UserCiaResultSet {
                    Id = userCia.Id,
                    UserAppId = userCia.UserAppId,
                    CodCia = userCia.CodCia,
                    CiaName = userCia.Cia.NomComercial ?? ""
                })
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserCiaRepository), nameof(GetUserCias));
            return Task.FromResult(new List<UserCiaResultSet>());
        }
    }
}
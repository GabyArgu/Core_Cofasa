using System.Data;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

public interface IUserRepository {
    Task<List<UserAppResultSet>> GetAll();
    Task<UserAppResultSet?> GetOneById(int id);

    Task<UserAppResultSet?> GetOneByUsername(string codCia, string username, string? email);

    // Task<bool> SaveOrUpdate(UserAppDto data);
    Task<int> SaveOrUpdate(UserAppDto data);
    Task<bool> DeleteOne(int id);
    Task<int> GetCount();
}

public class UserRepository(
    DbContext dbContext,
    ICoreHash coreHash,
    ILogger<UserRepository> logger
) : IUserRepository {
    public Task<List<UserAppResultSet>> GetAll() {
        try {
            return dbContext.UserApp
                .FromSqlRaw(
                    "SELECT * FROM CONTABLE.ObtenerUsuarios({0}, {1}, {2})",
                    DBNull.Value, DBNull.Value, DBNull.Value
                )
                .Select(entity => new UserAppResultSet {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Active = entity.Active,
                    CreatedAt = entity.CreatedAt
                })
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRepository), nameof(GetAll));
            return Task.FromResult(new List<UserAppResultSet>());
        }
    }

    public Task<UserAppResultSet?> GetOneById(int id) {
        try {
            return dbContext.UserApp
                .FromSqlRaw(
                    "SELECT * FROM CONTABLE.ObtenerUsuarios({0}, {1}, {2})",
                    id, DBNull.Value, DBNull.Value
                )
                .Select(entity => new UserAppResultSet {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Active = entity.Active
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRepository), nameof(GetOneById));
            return Task.FromResult<UserAppResultSet?>(null);
        }
    }

    public Task<UserAppResultSet?> GetOneByUsername(string codCia, string username, string? email) {
        try {
            IQueryable<UserApp> efQuery = dbContext.UserApp
                .Where(user =>
                    user.UserCia != null && user.UserCia.Any(uc => uc.CodCia == codCia)
                    && user.Active == true && user.DeletedAt == null
                );

            efQuery = email is not null
                ? efQuery.Where(user => user.Email == email)
                : efQuery.Where(user => user.UserName == username);

            return efQuery
                .Select(entity => new UserAppResultSet {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Active = entity.Active
                })
                .FirstOrDefaultAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRepository), nameof(GetOneByUsername));
            return Task.FromResult<UserAppResultSet?>(null);
        }
    }

    public async Task<int> SaveOrUpdate(UserAppDto data) {
        try {
            UserApp? userEntity;

            if (data.Id is > 0) {
                userEntity = await dbContext.UserApp.Where(user => user.Id == data.Id).FirstOrDefaultAsync();

                if (userEntity != null) {
                    userEntity.UserName = data.UserName;
                    userEntity.Email = data.Email;
                    userEntity.FirstName = data.FirstName;
                    userEntity.LastName = data.LastName;
                    userEntity.Active = data.Active;
                    userEntity.UpdatedAt = DateTime.Now;
                    dbContext.UserApp.Update(userEntity);

                    if (data.Password is not (null or "")) {
                        var encrypted = coreHash.Hash(data.Password);
                        userEntity.PasswordHash = encrypted;
                    }
                }
            }
            else {
                var encrypted = coreHash.Hash(data.Password);

                userEntity = new UserApp {
                    UserName = data.UserName,
                    Email = data.Email,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Active = data.Active,
                    PasswordHash = encrypted,
                    CreatedAt = DateTime.Now
                };
                await dbContext.UserApp.AddAsync(userEntity);
            }

            await dbContext.SaveChangesAsync();
            return userEntity?.Id ?? 0;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRepository), nameof(SaveOrUpdate));
            return 0;
        }
    }

    public async Task<bool> DeleteOne(int id) {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try {
            command.CommandText = $"{CC.SCHEMA}.BorrarUsuarioLogico";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();
            return true;
        }
        catch (Exception e) {
            if (command.Connection?.State == ConnectionState.Open) await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(UserRepository), nameof(DeleteOne));
            return false;
        }
    }

    public Task<int> GetCount() => dbContext.UserApp
        .Where(us => us.DeletedAt == null && us.Active == true)
        .CountAsync();
}
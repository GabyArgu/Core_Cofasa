using System.Security.Claims;
using CoreContable.Entities;
using CoreContable.Entities.FuntionResult;
using CoreContable.Models;
using CoreContable.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Services;

public interface ISecurityRepository {
    Task<List<ValidateUserOnLoginFromFunctionResult>?> GetLoginUser(string userName, string codCia);
    SessionViewModel? GetCurrentSession();
    int GetSessionUserId();
    string GetSessionFullName();
    string GetSessionUserName();
    string GetSessionCiaCode();
    string GetSessionCiaName();
    bool HasPermission(string permission);
    string GetSessionUserPermissionList();
    Task<bool> SaveSession(List<ValidateUserOnLoginFromFunctionResult> userAccessList);
    void ChangeSessionCia(string ciaCode);
    Task DestroySession();

    Task<List<UserMenuPermissionFromFunctionResult>> LoadUserMenuByCompany(string username, string ciaCode,
        int? perFatherId = null);

    Task<List<ItemMenuViewModel>> GetMenuSession(string username, string ciaCode);

    // Task<UserApp?> Authenticate(string codCia, string username, string pwd);
    Task<UserApp?> Authenticate(string username, string pwd);
}

public class SecurityRepository(
    DbContext dbContext,
    ICoreHash coreHash,
    IHttpContextAccessor httpContext,
    ILogger<SecurityRepository> logger
) : ISecurityRepository {
    public async Task<List<ValidateUserOnLoginFromFunctionResult>?> GetLoginUser(string userName, string codCia) {
        try {
            return await dbContext.ValidateUserOnLoginFromFunctionResult
                .FromSql($"SELECT * FROM CONTABLE.validate_user_on_login({userName}, {codCia})")
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetLoginUser));
            return null;
        }
    }

    private string GetClaimValue(string key) {
        return httpContext.HttpContext?.User.Claims?.FirstOrDefault(c => c.Type == key)?.Value!;
    }

    public SessionViewModel? GetCurrentSession() {
        try {
            if (GetClaimValue(CC.CiaCodeKey) == null) {
                DestroySession();
                return null;
            }

            if (httpContext.HttpContext is { Session.IsAvailable: true }) {
                var model = new SessionViewModel {
                    CiaCode = GetClaimValue(CC.CiaCodeKey),
                    CiaName = GetClaimValue(CC.CiaNameKey),
                    UserId = int.Parse(GetClaimValue(CC.UserIdKey)),
                    UserName = GetClaimValue(CC.UserNameKey),
                    FullName = GetClaimValue(CC.FullNameKey),
                    UserPermissions = GetClaimValue(CC.UserPermissionsKey)
                };

                return model;
            }
        }
        catch (Exception e) {
            DestroySession();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetCurrentSession));
            return null;
        }

        return null;
    }

    public int GetSessionUserId() {
        try {
            var user = GetCurrentSession();
            return user?.UserId ?? 0;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetSessionUserId));
            return 0;
        }
    }

    public string GetSessionFullName() {
        try {
            var user = GetCurrentSession();
            return user?.FullName ?? "";
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetSessionFullName));
            return "";
        }
    }

    public string GetSessionUserName() {
        try {
            var user = GetCurrentSession();
            return user?.UserName ?? "";
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetSessionUserName));
            return "";
        }
    }

    public string GetSessionCiaCode() {
        try {
            var user = GetCurrentSession();
            return user?.CiaCode ?? "";
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetSessionCiaCode));
            return "";
        }
    }

    public string GetSessionCiaName() {
        try {
            var user = GetCurrentSession();
            return user?.CiaName ?? "";
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetSessionCiaName));
            return "";
        }
    }

    public bool HasPermission(string permission) => GetSessionUserPermissionList().Contains(permission);

    public string GetSessionUserPermissionList() {
        try {
            var user = GetCurrentSession();
            return user?.UserPermissions ?? "";
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetSessionUserPermissionList));
            return "";
        }
    }

    public async Task<bool> SaveSession(List<ValidateUserOnLoginFromFunctionResult>? userAccessList) {
        try {
            if (userAccessList.IsNullOrEmpty()) return false;
            var rawUser = userAccessList?.FirstOrDefault();
            if (rawUser == null) return false;
            var permissions = string.Join(",", userAccessList
                .Select(per => per.PerAlias));

            var claims = new List<Claim> {
                new(CC.CiaCodeKey, rawUser.CodCia),
                new(CC.CiaNameKey, rawUser.CiaName),
                new(CC.UserNameKey, rawUser.UserName),
                new(CC.UserIdKey, $"{rawUser.UserId}"),
                new(CC.FullNameKey, $"{rawUser.FirstName} {rawUser.LastName}"),
                new(CC.UserPermissionsKey, permissions)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A
                // value set here overrides the ExpireTimeSpan option of
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http
                // redirect response value.
            };

            await httpContext.HttpContext?.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(SaveSession));
            return false;
        }

        return true;
    }

    public void ChangeSessionCia(string ciaCode) => httpContext.HttpContext?.Session.SetString(CC.CiaCodeKey, ciaCode);

    public async Task DestroySession() {
        try {
            await httpContext.HttpContext?
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)!;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(DestroySession));
        }
    }

    public Task<List<UserMenuPermissionFromFunctionResult>> LoadUserMenuByCompany(
        string username,
        string ciaCode,
        int? perFatherId = null
    ) {
        try {
            return dbContext.UserMenuPermissionFromFunctionResult
                .FromSqlRaw(
                    "SELECT * FROM CONTABLE.get_user_permissions_by_cia({0}, {1}, {2})",
                    username, ciaCode, perFatherId != null ? perFatherId : DBNull.Value
                )
                .ToListAsync();
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(LoadUserMenuByCompany));
            return Task.FromResult(new List<UserMenuPermissionFromFunctionResult>());
        }
    }

    public async Task<List<ItemMenuViewModel>> GetMenuSession(string username, string ciaCode) {
        var fatherList = new List<ItemMenuViewModel>();

        try {
            var permissionsFirstLevelList = await LoadUserMenuByCompany(username, ciaCode, null);

            if (permissionsFirstLevelList.IsNullOrEmpty()) return fatherList;

            foreach (var firstLevel in permissionsFirstLevelList) {
                if (fatherList.Any(fa => fa.FatherAlias == firstLevel.PerAlias)) {
                    continue;
                }

                var itemMenuFather = new ItemMenuViewModel {
                    FatherId = firstLevel.PerId,
                    FatherName = firstLevel.PerName,
                    FatherAlias = firstLevel.PerAlias,
                    FatherUrl = firstLevel.PerUrl,
                    FatherIcon = firstLevel.PerIcon,
                    FatherVisibility = firstLevel.PerVisibility
                };

                var permissionsSecondLevelList = await LoadUserMenuByCompany(username, ciaCode, firstLevel.PerId);
                var childrenList = permissionsSecondLevelList?.Select(secondLevel => new ItemMenuChildrenViewModel {
                    ChildrenId = secondLevel.PerId,
                    ChildrenName = secondLevel.PerName,
                    ChildrenAlias = secondLevel.PerAlias,
                    ChildrenUrl = secondLevel.PerUrl,
                    ChildrenIcon = secondLevel.PerIcon,
                    ChildrenVisibility = secondLevel.PerVisibility
                }).ToList() ?? new List<ItemMenuChildrenViewModel>();

                //// Verifica si el menú principal es "Contabilidad" y agrega "Centro Cuenta Formato" antes de "Asientos Mayorizados"
                //if (itemMenuFather.FatherName == "Contabilidad") {
                //    var centroCuentaFormato = new ItemMenuChildrenViewModel {
                //        ChildrenId = 2002, // Nuevo ID único
                //        ChildrenName = "Centro Cuenta Formato",
                //        ChildrenAlias = "CentroCuentaFormato",
                //        ChildrenUrl = "/CentroCuentaFormato/Index", // URL de la nueva opción
                //        ChildrenIcon = "fas fa-book",
                //        ChildrenVisibility = true
                //    };

                //    var index = childrenList.FindIndex(c => c.ChildrenName == "Asientos Mayorizados");

                //    // Inserta "Centro Cuenta Formato" antes de "Asientos Mayorizados" si existe, o al final si no
                //    if (index >= 0) {
                //        childrenList.Insert(index, centroCuentaFormato);
                //    }
                //    else {
                //        childrenList.Add(centroCuentaFormato);
                //    }
                //}

                itemMenuFather.ChildrenList = childrenList;
                fatherList.Add(itemMenuFather);
            }
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(GetMenuSession));
        }

        return fatherList;
    }



    public async Task<bool> CheckFirstPermission(string username, string ciaCode, string alias) {
        var permissionList = await GetMenuSession(username, ciaCode);
        return permissionList.Any(permission => permission.FatherAlias.Equals(alias));
    }

    public async Task<bool> CheckSecondPermission(string username, string ciaCode, string alias) {
        var result = false;
        var permissionList = await GetMenuSession(username, ciaCode);

        foreach (
            var childrens
            in permissionList
                .Select(permission => permission.ChildrenList)
                .Where(childrens => !childrens.IsNullOrEmpty())
        ) {
            if (childrens == null) break;

            if (childrens.Any(children => !children.ChildrenAlias
                    .IsNullOrEmpty() && children.ChildrenAlias.Contains(alias)))
                result = true;

            if (result) break;
        }

        return result;
    }

    public async Task<UserApp?> Authenticate(string username, string pwd) {
        try {
            var user = await dbContext.UserApp
                .Include(user => user.UserCia)
                // .Where(user => user.UserCia.Any(uc => uc.CodCia == codCia) && user.UserName == username)
                .Where(user => user.UserName == username && user.DeletedAt == null && user.Active == true)
                .FirstOrDefaultAsync();

            if (user == null) return null;
            return coreHash.Verify(pwd, user.PasswordHash ?? "") ? user : null;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(SecurityRepository), nameof(Authenticate));
            return null;
        }
    }
}
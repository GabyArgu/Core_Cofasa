using System.Text.Json;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

[Authorize]
public class UsersController (
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IUserCiaRepository userCiaRepository,
    ISecurityRepository securityRepository,
    ILogger<UsersController> logger
) : Controller {
    [IsAuthorized (alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_USERS)]
    public IActionResult Index ( ) {
        return View ( );
    }

    [IsAuthorized (alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_USERS)]
    [HttpGet]
    public async Task<JsonResult> GetUsersForDt ( ) {
        List<UserAppResultSet>? data;

        try {
            data = await userRepository.GetAll ( );
        }
        catch (Exception e) {
            data = null;
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (GetUsersForDt));
        }

        return Json (new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    //[IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOneUser ([FromQuery] int userId) {
        UserAppResultSet? data;

        try {
            data = await userRepository.GetOneById (userId);

            if (data != null) {
                // Getting user roles
                var currentCia = securityRepository.GetSessionCiaCode ( );
                var userRoles = await userRoleRepository.GetUserUserRoles (userId, currentCia);

                data.SelRoles = userRoles
                    .GroupBy (ur => ur.IdRole)
                    .Select (ur => new Select2ResultSet {
                        // id = $"{ur.IdRole}",
                        // text = ur.RoleName,
                        id = $"{ur.First ( ).IdRole}",
                        text = ur.First ( ).RoleName ?? ""
                    })
                    .ToList ( );
                data.OldRoles = data.SelRoles.Select (role => int.Parse (role.id)).ToList ( );

                // Getting user Cias.
                var userCias = await userCiaRepository.GetUserCias (userId);
                data.SelCias = userCias
                    .GroupBy (cia => cia.CodCia)
                    .Select (userCia => new Select2ResultSet {
                        id = userCia.First ( ).CodCia,
                        text = userCia.First ( ).CiaName
                    })
                    .ToList ( );
                data.OldCias = userCias.Select (userCia => userCia.CodCia).ToList ( );
            }
        }
        catch (Exception e) {
            data = null;
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (GetOneUser));
        }

        return Json (new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    /*[IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE}")]*/
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate ([FromForm] UserAppDto data) {
        bool result;
        var isUpdating = data.Id is > 0;
        var currentCia = securityRepository.GetSessionCiaCode ( );

        try {
            var isUserNameInUse = await userRepository.GetOneByUsername (currentCia, data.UserName, null);
            var isEmailInUse = await userRepository.GetOneByUsername (currentCia, data.UserName, data.Email);

            if (isUserNameInUse != null && isUserNameInUse.Id != data.Id) {
                return Json (new {
                    success = false,
                    message = "El nombre de usuario ya está en uso"
                });
            }

            if (isEmailInUse != null && isEmailInUse.Id != data.Id) {
                return Json (new {
                    success = false,
                    message = "El correo electrónico ya está en uso"
                });
            }

            data.Active = isUpdating ? data.Active : true;

            if (isUpdating) {
                data.UpdatedAt = DateTime.Now;
            }
            else {
                data.CreatedAt = DateTime.Now;
            }

            var userId = await userRepository.SaveOrUpdate (data);
            result = userId > 0;

            var selRoles = data.SelRoles?.Split (",").Select (x => int.Parse (x)).ToList ( ) ?? [];
            var oldRoles = data.OldRoles?.Split (",").Select (x => int.Parse (x)).ToList ( ) ?? [];

            var selCias = data.SelCias?.Split (",").ToList ( ) ?? [];
            var oldCias = data.OldCias?.Split (",").ToList ( ) ?? [];

            bool rolesSaved = await ProcessUserRoles (selRoles, oldRoles, userId);
            bool ciasSaved = await ProcessUserCias (selCias, oldCias, userId);
        }
        catch (Exception e) {
            result = false;
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (SaveOrUpdate));
        }

        var message = isUpdating
            ? "Usuario actualizado correctamente"
            : "Usuario guardado correctamente";

        var errorMessage = isUpdating
            ? "Ocurrió un error al actualizar el usuario"
            : "Ocurrió un error al guardar el registro";

        return Json (new {
            success = result,
            message = result ? message : errorMessage
        });
    }

    [IsAuthorized (alias: $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_DELETE}")]
    [HttpDelete]
    public async Task<JsonResult> DeleteOne ([FromQuery] int userId) {
        bool result;

        try {
            result = await userRepository.DeleteOne (userId);
        }
        catch (Exception e) {
            result = false;
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (DeleteOne));
        }

        return Json (new {
            success = result,
            message = result
                ? "Usuario eliminado exitosamente"
                : "Ocurrió un error al eliminar el usuario"
        });
    }

    private async Task<bool> ProcessUserRoles (List<int> selRoles, List<int> oldRoles, int userId) {
        var newRoles = new List<int> ( );
        var toRemoveRoles = new List<int> ( );

        try {
            if (selRoles.Count == 0 && oldRoles.Count == 0) {
                return true;
            }

            // Caso 1: las listas tienen los mismo valores.
            if (selRoles.All (oldRoles.Contains) && selRoles.Count == oldRoles.Count) {
                return true;
            }

            // Caso 2: La lista de roles tiene valores pero la lista de valores anteriores no.
            if (selRoles.Count > 0 && oldRoles.Count == 0) {
                newRoles = selRoles;
                await DoUserRolesDbProcess (newRoles, toRemoveRoles, userId);
                return true;
            }

            // Caso 3: La lista de roles anteriores tiene valores pero la del formulario no.
            // En este caso se remueven todos los roles.
            if (oldRoles.Count > 0 && selRoles.Count == 0) {
                toRemoveRoles = oldRoles;
                await DoUserRolesDbProcess (newRoles, toRemoveRoles, userId);
                return true;
            }

            // Caso 4: Ambas listas tienen valores pero presentan diferente lenght.
            // Se asume que se elimo o se agrego un rol o ambos casos.
            if (selRoles.Count != oldRoles.Count || selRoles.Count == oldRoles.Count) {
                newRoles = selRoles.Except (oldRoles).ToList ( );
                toRemoveRoles = oldRoles.Except (selRoles).ToList ( );
                await DoUserRolesDbProcess (newRoles, toRemoveRoles, userId);
                return true;
            }

            return false;
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (ProcessUserRoles));
            return false;
        }
    }

    private async Task DoUserRolesDbProcess (List<int> newRoles, List<int> toRemoveRoles, int userId) {
        if (newRoles.Count > 0) {
            await userRoleRepository.SaveUserRoles (newRoles, userId);
        }

        if (toRemoveRoles.Count > 0) {
            await userRoleRepository.DeleteUserRoles (toRemoveRoles, userId);
        }
    }

    private async Task<bool> ProcessUserCias (List<string> selCias, List<string> oldCias, int userId) {
        var newCias = new List<string> ( );
        var toRemoveCias = new List<string> ( );

        try {
            if (selCias.Count == 0 && oldCias.Count == 0) {
                return true;
            }

            // Caso 1: las listas tienen los mismo valores.
            if (selCias.All (oldCias.Contains) && selCias.Count == oldCias.Count) {
                return true;
            }

            // Caso 2: La lista de roles tiene valores pero la lista de valores anteriores no.
            if (selCias.Count > 0 && oldCias.Count == 0) {
                newCias = selCias;
                await DoUserCiasDbProcess (newCias, toRemoveCias, userId);
                return true;
            }

            // Caso 3: La lista de roles anteriores tiene valores pero la del formulario no.
            // En este caso se remueven todos los roles.
            if (oldCias.Count > 0 && selCias.Count == 0) {
                toRemoveCias = oldCias;
                await DoUserCiasDbProcess (newCias, toRemoveCias, userId);
                return true;
            }

            // Caso 4: Ambas listas tienen valores pero presentan diferente lenght.
            // Se asume que se elimo o se agrego un rol o ambos casos.
            if (selCias.Count != oldCias.Count || selCias.Count == oldCias.Count) {
                newCias = selCias.Except (oldCias).ToList ( );
                toRemoveCias = oldCias.Except (selCias).ToList ( );
                await DoUserCiasDbProcess (newCias, toRemoveCias, userId);
                return true;
            }

            return false;
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (ProcessUserCias));
            return false;
        }
    }

    private async Task DoUserCiasDbProcess (List<string> newCias, List<string> toRemoveCias, int userId) {
        if (newCias.Count > 0) {
            await userCiaRepository.SaveList (newCias, userId);
        }

        if (toRemoveCias.Count > 0) {
            await userCiaRepository.DeleteList (toRemoveCias, userId);
        }
    }

    [IsAuthorized (alias: CC.THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY)]
    [HttpGet]
    public async Task<JsonResult> GetUserRolesByCia ([FromQuery] string codCia, [FromQuery] int userId) {
        var roles = new List<Select2ResultSet> ( );
        var oldRoles = new List<string> ( );

        try {
            var userRoles = await userRoleRepository.GetUserUserRoles (userId, codCia);
            roles = userRoles.Select (ur => new Select2ResultSet {
                id = $"{ur.IdRole}",
                text = ur.RoleName
            })
                .ToList ( );

            oldRoles = roles.Select (role => role.id).ToList ( );
        }
        catch (Exception e) {
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (GetUserRolesByCia));
        }

        var data = new {
            roles,
            oldRoles
        };

        return Json (new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized (alias: CC.THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY)]
    [HttpPost]
    public async Task<JsonResult> UpdateCompanyAccess ([FromForm] UserCompanyAccessDto data) {
        bool result;

        try {
            var selRoles = data.SelRoles?.Split (",").Select (x => int.Parse (x)).ToList ( ) ?? [];
            var oldRoles = data.OldRoles?.Split (",").Select (x => int.Parse (x)).ToList ( ) ?? [];

            bool rolesSaved = await ProcessUserRoles (selRoles, oldRoles, data.UserId);
            result = rolesSaved;
        }
        catch (Exception e) {
            result = false;
            logger.LogError (e, "Ocurrió un error en {Class}.{Method}",
                nameof (UsersController), nameof (SaveOrUpdate));
        }

        return Json (new {
            success = result,
            message = result
                ? "Acceso modificado para la empresa seleccionada"
                : "Ocurrió un error al modificar el acceso a la empresa seleccionada"
        });
    }
}
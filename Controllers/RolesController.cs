using System.Text.Json;
using CoreContable.Entities;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers;

public class RolesController(
    IRoleRepository roleRepository,
    ISecurityRepository securityRepository,
    IRolePermissionRepository rolePermissionRepository,
    ILogger<RolesController> logger
) : Controller {
    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_ROLES)]
    public IActionResult Index() {
        return View();
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_ROLES_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_ROLES_CAN_UPDATE}")]
    [HttpPost]
    public async Task<JsonResult> SaveOrUpdate([FromForm] RoleDto data) {
        bool result;
        var isEditing = data.Id is > 0;
        int role;

        try {
            data.Active ??= false;
            var lstPermissionsIds = data.idsPermissions?.Split(",")
                .Select(x => int.Parse(x)).ToList() ?? [];

            role = await roleRepository.SaveOrUpdate(data);

            if (role > 0 && lstPermissionsIds.Count > 0) {
                if (!isEditing) {
                    foreach (var permId in lstPermissionsIds) { 
                        var rolePermission = new RolePermission() {
                            IdPermission = permId,
                            IdRole = role,
                            CreatedAt = DateTime.Now
                        };

                        await rolePermissionRepository.Save(rolePermission, isEditing);
                    }
                }
                else {
                    var lstRolePermissionObjs = await rolePermissionRepository.GetAllPermissionByRole(role);

                    if (lstRolePermissionObjs.Count > 0) {
                        var permissionOldOk = new List<int>();
                        var permissionForm = lstPermissionsIds;
                        var permissionData = lstRolePermissionObjs.Select(rp => rp.IdPermission).ToList();

                        foreach (var id in permissionForm) {
                            if (permissionData.Contains(id)) {
                                permissionOldOk.Add(id);
                            }
                            else {
                                var rolePermission = new RolePermission {
                                    IdPermission = id,
                                    IdRole = role,
                                    CreatedAt = DateTime.Now
                                };

                                await rolePermissionRepository.Save(rolePermission, false);
                            }
                        }

                        foreach (var id in permissionData.Where(id => !permissionOldOk.Contains(id))) {
                            await rolePermissionRepository.DeleteRolePermission(role, id);
                        }
                    }
                    else {
                        foreach (var permId in lstPermissionsIds) {
                            var rolePermission = new RolePermission() {
                                IdPermission = permId,
                                IdRole = role,
                                CreatedAt = DateTime.Now
                            };

                            await rolePermissionRepository.Save(rolePermission, isEditing);
                        }
                    }
                }
            }

            result = true;
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolesController), nameof(SaveOrUpdate));
            result = false;
        }

        var message = isEditing
            ? "Rol y permisos actualizados correctamente"
            : "Rol y permisos guardados correctamente";

        var errorMessage = isEditing
            ? "Ocurrió un error al actualizar el rol y sus permisos"
            : "Ocurrió un error al guardar el rol y sus permisos";

        return Json(new {
            success = result,
            message = result ? message : errorMessage,
        });
    }

    [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_ROLES)]
    [HttpGet]
    public async Task<JsonResult> GetRolesForDt([FromQuery] string ciaCod) {
        List<RoleResultSet>? data;

        try {
            data = await roleRepository.GetAllDt(ciaCod);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolesController), nameof(GetRolesForDt));
            data = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_ROLES_CAN_UPDATE)]
    [HttpGet]
    public async Task<JsonResult> GetOneRole([FromQuery] int roleId) {
        RoleResultSet? data;

        try {
            data = await roleRepository.GetOneById(roleId);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolesController), nameof(GetOneRole));
            data = null;
        }

        return Json(new {
            success = true,
            message = "Access data",
            data
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_ROLES_CAN_DELETE)]
    [HttpDelete]
    public async Task<JsonResult> DeleteOneRole([FromQuery] int roleId) {
        bool result;

        try {
            result = await roleRepository.DeleteOneById(roleId);
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolesController), nameof(DeleteOneRole));
            result = false;
        }

        return Json(new {
            success = result,
            message = result ? "Rol eliminado exitosamente" : "Ocurrió un error al eliminar el rol",
        }, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE}" +
                         $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY}")]
    [HttpGet]
    public async Task<JsonResult> GetToSelect2([FromQuery] string q, [FromQuery] string codCia) {
        var roles = new List<Select2ResultSet>();
        var currentCia = codCia.IsNullOrEmpty() ? securityRepository.GetSessionCiaCode() : codCia;

        try {
            var rawRoles = await roleRepository.GetAll(currentCia, q);

            if (rawRoles.Count > 0) {
                roles = rawRoles.Select(raw => new Select2ResultSet {
                    id = $"{raw.Id}",
                    text = raw.Name
                }).ToList();
            }
        }
        catch (Exception e) {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(RolesController), nameof(GetToSelect2));
        }

        return Json(new {
            results = roles,
        });
    }
}
using CoreContable.Entities;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.Controllers;

public class PermissionController(
    IPermissionRepository permissionRepository,
    IRolePermissionRepository rolePermissionRepository,
    ILogger<PermissionController> logger
) : Controller
{
    [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_ROLES_CAN_ADD}," +
                         $"{CC.THIRD_LEVEL_PERMISSION_ROLES_CAN_UPDATE}")]
    [HttpGet]
    public async Task<JsonResult> GetPermissions([FromQuery] int roleId)
    {
        List<Permission> lstObjFirstLevel;
        var lstFirstLevel = new List<JsTreeResultSet>();
        var selectedPermissions = new Dictionary<int, bool>();
        bool withDefault = true;

        try
        {
            if (roleId > 0)
            {
                List<RolePermission> perms = await rolePermissionRepository.GetAllPermissionByRole(roleId);
                foreach (var permission in perms)
                {
                    selectedPermissions.Add(permission.IdPermission, true);
                }
                withDefault = false;
            }

            // FIRST LEVEL
            lstObjFirstLevel = await permissionRepository.GetFirstLevelPermissions();

            foreach (var objFirst in lstObjFirstLevel)
            {
                var itemFirst = new JsTreeResultSet()
                {
                    id = objFirst.Id,
                    text = objFirst.Name,
                    icon = false
                };

                List<JsTreeResultSet> lstSecondLevel = new List<JsTreeResultSet>();
                List<Permission> lstObjSecondLevel = await permissionRepository
                    .GetSecondLevelPermissionsByIdFirstLevel(itemFirst.id);

                if (lstObjSecondLevel.Count > 0)
                {
                    itemFirst.state = new JSTreeStateResultSet();

                    foreach (var secondObj in lstObjSecondLevel)
                    {
                        var itemSecond = new JsTreeResultSet()
                        {
                            id = secondObj.Id,
                            text = secondObj.Name,
                            icon = false
                        };
                        
                        List<JsTreeResultSet> lstThirdLevel;
                        List<Permission> lstObjThirdLevel = await permissionRepository
                            .GetThirdLevelPermissionsByIdSecondLevel(itemSecond.id);

                        if (lstObjThirdLevel.Count > 0)
                        {
                            itemSecond.state = new JSTreeStateResultSet();
                            lstThirdLevel = GetScreenPermissions(withDefault, lstObjThirdLevel, selectedPermissions);
                            itemSecond.children = lstThirdLevel;
                        }
                        else
                        {
                            if (withDefault)
                            {
                                itemSecond.state = new JSTreeStateResultSet();
                            }
                            else
                            {
                                itemSecond.state = new JSTreeStateResultSet()
                                {
                                    opened = true,
                                    selected = selectedPermissions.ContainsKey(secondObj.Id)
                                };
                            }
                        }
                        lstSecondLevel.Add(itemSecond);
                    }
                    itemFirst.children = lstSecondLevel;
                }
                else
                {
                    List<Permission> lstObjThirdLevel = await permissionRepository
                        .GetThirdLevelPermissionsByIdSecondLevel(itemFirst.id);
                    
                    if(lstObjThirdLevel.Count > 0)
                    {
                        itemFirst.state = new JSTreeStateResultSet();
                        List<JsTreeResultSet> lstThirdLevel = GetScreenPermissions(withDefault, lstObjThirdLevel, selectedPermissions);
                        itemFirst.children = lstThirdLevel;
                    }else{
                        if (withDefault)
                        {
                            itemFirst.state = new JSTreeStateResultSet();
                        }
                        else
                        {
                            itemFirst.state = new JSTreeStateResultSet()
                            {
                                opened = true,
                                selected = selectedPermissions.ContainsKey(objFirst.Id)
                            };
                        }
                    }
                }
                lstFirstLevel.Add(itemFirst);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(PermissionController), nameof(GetPermissions));
        }

        return Json(new
        {
            success = true,
            message = "",
            data = lstFirstLevel
        });
    }

    private List<JsTreeResultSet> GetScreenPermissions(
        bool withDefault,
        List<Permission> lstObjThirdLevel,
        Dictionary<int, bool> selectedPermissions
    )
    {
        List<JsTreeResultSet> lstScreenPerm = new List<JsTreeResultSet>();

        foreach (var thirdObj in lstObjThirdLevel)
        {
            var itemThird = new JsTreeResultSet()
            {
                id = thirdObj.Id,
                text = thirdObj.Name,
                icon = false
            };

            if (withDefault)
            {
                itemThird.state = new JSTreeStateResultSet();
            }
            else
            {
                itemThird.state = new JSTreeStateResultSet()
                {
                    opened = true,
                    selected = selectedPermissions.ContainsKey(thirdObj.Id)
                };
            }
            
            lstScreenPerm.Add(itemThird);
        }

        return lstScreenPerm;
    }
}
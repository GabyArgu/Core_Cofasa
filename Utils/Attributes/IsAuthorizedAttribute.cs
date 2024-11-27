using CoreContable.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreContable.Utils.Attributes;

///
/// https://blog.iamdavidfrancis.com/posts/aspnet-filter-dependency-injection/
///
/// For all permission levels.
///
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IsAuthorizedAttribute : TypeFilterAttribute
{
    public IsAuthorizedAttribute(string alias)
        : base(typeof(IsAuthorizedFilter))
    {
        Arguments = [alias];
    }
}

public class IsAuthorizedFilter(
    string alias,
    ISecurityRepository securityRepository
) : IAuthorizationFilter
{
    public void OnAuthorization(
        AuthorizationFilterContext context
    )
    {
        var userPermissions = securityRepository.GetSessionUserPermissionList();
        var canAccess = userPermissions.Split(",").Intersect(alias.Split(",")).Any();

        if (canAccess) return;
        if (userPermissions.Contains(CC.FIST_LEVEL_PERMISSION_DASHBOARD))
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                    {
                        action = CC.DashboardActionName,
                        controller = CC.HomeControllerName
                    }
                ));
        }
        else
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                    {
                        action = CC.LoginActionName,
                        controller = CC.SecurityControllerName
                    }
                ));
        }
    }
}
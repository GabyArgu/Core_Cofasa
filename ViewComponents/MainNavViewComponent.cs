using CoreContable.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreContable.ViewComponents;

public class MainNavViewComponent : ViewComponent
{

    private readonly ISecurityRepository _securityRepository;

    public MainNavViewComponent(ISecurityRepository securityRepository)
    {
        _securityRepository = securityRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var username = _securityRepository.GetSessionUserName();
        var currentCia = _securityRepository.GetSessionCiaCode();
        var menuList = await _securityRepository.GetMenuSession(username, currentCia);

        return View(menuList);
    }
}
using CoreContable.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoreContable.ViewComponents;

public class TopNavViewComponent(
    ICiasRepository ciasRepository,
    ISecurityRepository securityRepository)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var currentCia = securityRepository.GetSessionCiaCode();
        var userId = securityRepository.GetSessionUserId();
        var cias = await ciasRepository.GetUserCias(userId);

        var ciasToSelect = cias.Select(cia => new SelectListItem
        {
            Value = cia.Cod,
            Text = cia.NomComercial,
            Selected = cia.Cod == currentCia,
        }).ToList();

        return View(ciasToSelect);
    }

}
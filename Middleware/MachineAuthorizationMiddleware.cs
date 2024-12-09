using System.Net.NetworkInformation;
using CoreContable.Services;
using CoreContable.Utils;

namespace CoreContable.Middleware;

public class MachineAuthorizationMiddleware(
    RequestDelegate next, ICoreHash coreHash, IConfiguration configuration)
{

    private readonly List<string> _filteredUrls =
    [
        "/security/login",

        "/",
        "/users/index",
        "/roles/index",

        "/cias/index",
        "/params/conta",
        "/params/period",
        "/dmgdoctos/index",
        "/currency/index",
        "/dmgcuentas/index",
        "/centrocosto/index",

        "/repository/index",
        "/dmgpoliza/index",

        "/reports/index"
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        var requestPath = context.Request.Path.Value ?? "";
        var jojo = configuration[CC.JOJO] ?? "";

        if (!_filteredUrls.Contains(requestPath.ToLower()) || coreHash.Verify(CC.DIO, jojo))
        {
            await next(context);
            return;
        }

        var hostName = System.Net.Dns.GetHostName();
        var macAddress = GetMacAddress();
        var ada = configuration[CC.ADA] ?? "";
        var merged = $"{hostName}///{macAddress}";

        // context.Response.StatusCode = StatusCodes.Status403Forbidden;
        // await context.Response.WriteAsync($"hostname: {hostName} - mac: {macAddress} is Not authorized.");

        if (!coreHash.Verify(merged, ada))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            // await context.Response.WriteAsync("This machine is not authorized to run this application.");
            await context.Response.WriteAsync($"403:{hostName}<>{macAddress}");
            return;
        }

        await next(context);
        }

    private static string GetMacAddress()
    {
        var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up && 
                                   nic.NetworkInterfaceType != NetworkInterfaceType.Loopback);

        return networkInterface?.GetPhysicalAddress().ToString() ?? string.Empty;
    }
}
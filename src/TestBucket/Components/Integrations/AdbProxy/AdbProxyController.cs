using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace TestBucket.Components.Integrations.AdbProxy;

[ApiController]
public class AdbProxyController : ControllerBase
{

    public AdbProxyController()
    {
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpGet("/api/integrations/adb-proxy/_health")]
    public void GetHealth()
    {
    }

    [Authorize("ApiKeyOrBearer")]
    [HttpPost("/api/integrations/adb-proxy/inform")]
    public async Task<IActionResult> InformAdbDevicesChangedAsync([FromBody] AdbDevice[] devices)
    {
        var ip = HttpContext.Connection.RemoteIpAddress;
        if (ip is not null)
        {
            foreach(var device in devices)
            {
                device.Url = $"{ip}:{device.Port}";
            }
        }

        await Task.Delay(1000);
        return Ok();
    }
}

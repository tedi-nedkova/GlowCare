using System.Security.Claims;

namespace GlowCare.Common.Helpers;

public static class ControllerHelper
{
    public static string GetCurrentClientId(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}


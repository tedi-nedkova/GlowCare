using System.Security.Claims;
using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace GlowCare.Tests;

internal static class ControllerTestHelpers
{
    public static Mock<UserManager<GlowUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<GlowUser>>();
        return new Mock<UserManager<GlowUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    public static T AttachHttpContext<T>(T controller, Guid? userId = null) where T : Controller
    {
        var httpContext = new DefaultHttpContext();

        if (userId.HasValue)
        {
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) },
                    authenticationType: "TestAuth"));
        }

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        return controller;
    }
}

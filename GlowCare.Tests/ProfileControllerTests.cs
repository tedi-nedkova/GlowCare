using GlowCare.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class ProfileControllerTests
{
    [Fact]
    public async Task Index_ShouldRedirectToLogin_WhenUserIdIsInvalid()
    {
        var userService = new Mock<IUserService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns("bad-guid");
        var controller = ControllerTestHelpers.AttachHttpContext(new ProfileController(userService.Object, userManager.Object, Mock.Of<ILogger<ProfileController>>()));

        var result = await controller.Index();

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Identity/Account/Login", redirect.Url);
    }

    [Fact]
    public async Task Index_ShouldReturnView_WhenUserProfileLoads()
    {
        var userId = Guid.NewGuid();
        var expected = new UserProfileViewModel();
        var userService = new Mock<IUserService>();
        userService.Setup(x => x.GetUserProfileAsync(userId)).ReturnsAsync(expected);
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ProfileController(userService.Object, userManager.Object, Mock.Of<ILogger<ProfileController>>()), userId);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(expected, view.Model);
    }
}

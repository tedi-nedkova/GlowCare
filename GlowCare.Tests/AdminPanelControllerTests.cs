using GlowCare.Areas.Admin.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using GlowCare.ViewModels.Services;
using GlowCare.ViewModels.SpecialistRequest;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class AdminPanelControllerTests
{
    private static AdminPanelController CreateController(
        Mock<IUserService>? userService = null,
        Mock<ISpecialistApplicationService>? appService = null,
        Mock<IServiceService>? serviceService = null,
        Mock<IReviewService>? reviewService = null,
        Mock<Microsoft.AspNetCore.Identity.UserManager<GlowUser>>? userManager = null)
    {
        return ControllerTestHelpers.AttachHttpContext(
            new AdminPanelController(
                (userService ?? new Mock<IUserService>()).Object,
                (appService ?? new Mock<ISpecialistApplicationService>()).Object,
                (serviceService ?? new Mock<IServiceService>()).Object,
                (reviewService ?? new Mock<IReviewService>()).Object,
                (userManager ?? ControllerTestHelpers.CreateUserManagerMock()).Object,
                Mock.Of<ILogger<AdminPanelController>>()),
            Guid.NewGuid());
    }

    [Fact]
    public async Task ServiceManagement_Get_ShouldReturnViewModel()
    {
        var serviceService = new Mock<IServiceService>();
        serviceService.Setup(x => x.GetAdminServiceManagementViewModelAsync(null, 1, 6, null))
            .ReturnsAsync(new AdminServiceManagementViewModel());
        var controller = CreateController(serviceService: serviceService);

        var result = await controller.ServiceManagement(null, 1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<AdminServiceManagementViewModel>(view.Model);
    }

    [Fact]
    public async Task ServiceManagement_Post_ShouldReturnAddServiceView_WhenModelStateIsInvalid()
    {
        var serviceService = new Mock<IServiceService>();
        serviceService.Setup(x => x.GetAdminServiceManagementViewModelAsync(null, 1, 6, It.IsAny<AddServiceViewModel>()))
            .ReturnsAsync(new AdminServiceManagementViewModel());
        var controller = CreateController(serviceService: serviceService);
        controller.ModelState.AddModelError("NewService.Name", "Required");
        var model = new AdminServiceManagementViewModel { NewService = new AddServiceViewModel() };

        var result = await controller.ServiceManagement(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("AddService", view.ViewName);
    }

    [Fact]
    public async Task DeleteReview_ShouldRedirectBackToReviewsManagement()
    {
        var reviewService = new Mock<IReviewService>();
        var controller = CreateController(reviewService: reviewService);

        var result = await controller.DeleteReview(4);

        reviewService.Verify(x => x.SoftDeleteReviewAsync(4), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminPanelController.ReviewsManagement), redirect.ActionName);
    }

    [Fact]
    public async Task AdminRoleAssign_ShouldBlockSpecialistRole()
    {
        var controller = CreateController();

        var result = await controller.AdminRoleAssign(Guid.NewGuid(), "Specialist");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminPanelController.UserManagement), redirect.ActionName);
    }

    [Fact]
    public async Task AdminRoleAssign_ShouldRedirect_WhenUserDoesNotExist()
    {
        var userService = new Mock<IUserService>();
        userService.Setup(x => x.UserExistsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(false);
        var controller = CreateController(userService: userService);

        var result = await controller.AdminRoleAssign(Guid.NewGuid(), "Admin");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminPanelController.UserManagement), redirect.ActionName);
    }

    [Fact]
    public async Task DeleteUser_ShouldRedirectWithSuccess_WhenDeletionSucceeds()
    {
        var userId = Guid.NewGuid();
        var userService = new Mock<IUserService>();
        userService.Setup(x => x.UserExistsByIdAsync(userId)).ReturnsAsync(true);
        userService.Setup(x => x.DeleteUserAsync(userId)).ReturnsAsync(true);
        var controller = CreateController(userService: userService);

        var result = await controller.DeleteUser(userId);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminPanelController.UserManagement), redirect.ActionName);
    }

    [Fact]
    public async Task PendingApplications_ShouldReturnView_WhenSuccessful()
    {
        var appService = new Mock<ISpecialistApplicationService>();
        appService.Setup(x => x.GetPendingApplicationsAsync()).ReturnsAsync(new List<SpecialistApplicationViewModel> { new() { Id = 1 } });
        var controller = CreateController(appService: appService);

        var result = await controller.PendingApplications();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<SpecialistApplicationViewModel>>(view.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task ApplicationDetails_ShouldReturnNotFound_WhenMissing()
    {
        var appService = new Mock<ISpecialistApplicationService>();
        appService.Setup(x => x.GetByIdAsync(3)).ReturnsAsync((SpecialistApplicationViewModel?)null);
        var controller = CreateController(appService: appService);

        var result = await controller.ApplicationDetails(3);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Decline_ShouldRedirectToDetails_WhenModelStateIsInvalid()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("RejectionReason", "Required");

        var result = await controller.Decline(new ReviewApplicationViewModel { Id = 8 });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AdminPanelController.ApplicationDetails), redirect.ActionName);
        Assert.Equal(8, redirect.RouteValues!["id"]);
    }
}

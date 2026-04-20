using GlowCare.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class ReviewControllerTests
{
    [Fact]
    public async Task Index_ShouldRedirectToError_WhenEmployeeReviewsMissing()
    {
        var reviewService = new Mock<IReviewService>();
        reviewService.Setup(x => x.GetReviewsByEmployeeIdAsync(It.IsAny<Guid>())).ReturnsAsync((ReviewIndexViewModel?)null);
        var controller = ControllerTestHelpers.AttachHttpContext(new ReviewController(reviewService.Object, Mock.Of<ILogger<ReviewController>>(), ControllerTestHelpers.CreateUserManagerMock().Object), Guid.NewGuid());

        var result = await controller.Index(Guid.NewGuid());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Error", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Add_Get_ShouldReturnView_WhenModelExists()
    {
        var userId = Guid.NewGuid();
        var model = new AddReviewViewModel { EmployeeId = Guid.NewGuid(), ProcedureId = 1, Comment = "valid comment", Rating = 5 };
        var reviewService = new Mock<IReviewService>();
        reviewService.Setup(x => x.GetAddReviewModelAsync(It.IsAny<Guid>(), userId, 1)).ReturnsAsync(model);
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ReviewController(reviewService.Object, Mock.Of<ILogger<ReviewController>>(), userManager.Object), userId);

        var result = await controller.Add(model.EmployeeId, 1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(model, view.Model);
    }

    [Fact]
    public async Task Add_Post_ShouldReturnViewWithRefreshedData_WhenModelStateIsInvalid()
    {
        var userId = Guid.NewGuid();
        var reviewService = new Mock<IReviewService>();
        reviewService.Setup(x => x.GetAddReviewModelAsync(It.IsAny<Guid>(), userId, 2)).ReturnsAsync(
            new AddReviewViewModel
            {
                EmployeeId = Guid.NewGuid(),
                ProcedureId = 2,
                SpecialistName = "Niki",
                ServiceId = 5,
                AvailableProcedures = new[] { new ProcedureOptionViewModel { ProcedureId = 2, DisplayName = "Proc" } }
            });
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ReviewController(reviewService.Object, Mock.Of<ILogger<ReviewController>>(), userManager.Object), userId);
        controller.ModelState.AddModelError("Comment", "Required");
        var model = new AddReviewViewModel { EmployeeId = Guid.NewGuid(), ProcedureId = 2, Comment = "x", Rating = 5 };

        var result = await controller.Add(model);

        var view = Assert.IsType<ViewResult>(result);
        var returned = Assert.IsType<AddReviewViewModel>(view.Model);
        Assert.Equal("Niki", returned.SpecialistName);
        Assert.Equal(5, returned.ServiceId);
        Assert.Single(returned.AvailableProcedures);
    }

    [Fact]
    public async Task Add_Post_ShouldRedirectToEmployeeDetails_WhenCreateSucceeds()
    {
        var userId = Guid.NewGuid();
        var reviewService = new Mock<IReviewService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ReviewController(reviewService.Object, Mock.Of<ILogger<ReviewController>>(), userManager.Object), userId);
        var model = new AddReviewViewModel { EmployeeId = Guid.NewGuid(), ProcedureId = 1, Comment = "very good visit", Rating = 5 };

        var result = await controller.Add(model);

        reviewService.Verify(x => x.CreateReviewAsync(model, userId), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("Employee", redirect.ControllerName);
    }
}

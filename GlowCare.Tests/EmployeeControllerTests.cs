using GlowCare.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Employees;
using GlowCare.ViewModels.SpecialistRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class EmployeeControllerTests
{
    [Fact]
    public async Task Index_ShouldReturnViewWithEmployees()
    {
        var employeeService = new Mock<IEmployeeService>();
        employeeService.Setup(x => x.GetEmployeesForIndexAsync("spa", "Massage"))
            .ReturnsAsync(new EmployeeIndexViewModel { SearchTerm = "spa", SelectedService = "Massage" });
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(employeeService.Object, new Mock<ISpecialistApplicationService>().Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()));

        var result = await controller.Index("spa", "Massage");

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EmployeeIndexViewModel>(view.Model);
        Assert.Equal("spa", model.SearchTerm);
    }

    [Fact]
    public async Task Details_ShouldReturnNotFound_WhenEmployeeMissing()
    {
        var employeeService = new Mock<IEmployeeService>();
        employeeService.Setup(x => x.GetEmployeeByIdAsync(It.IsAny<Guid>())).ReturnsAsync((EmployeeInfoViewModel?)null);
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(employeeService.Object, new Mock<ISpecialistApplicationService>().Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()));

        var result = await controller.Details(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Apply_Get_ShouldRedirectHome_WhenUserIdMissing()
    {
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(new Mock<IEmployeeService>().Object, new Mock<ISpecialistApplicationService>().Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()));

        var result = await controller.Apply();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Apply_Get_ShouldHideForm_WhenPendingApplicationExists()
    {
        var userId = Guid.NewGuid();
        var appService = new Mock<ISpecialistApplicationService>();
        appService.Setup(x => x.UserHasPendingApplicationAsync(userId)).ReturnsAsync(true);
        appService.Setup(x => x.UserIsAlreadySpecialistAsync(userId)).ReturnsAsync(false);
        appService.Setup(x => x.GetLatestByUserIdAsync(userId)).ReturnsAsync((SpecialistApplicationViewModel?)null);
        appService.Setup(x => x.GetApplicationDraftAsync(userId)).ReturnsAsync(new ApplySpecialistViewModel { Occupation = "Therapist", ExperienceYears = 4 });
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(new Mock<IEmployeeService>().Object, appService.Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()), userId);

        var result = await controller.Apply();

        var view = Assert.IsType<ViewResult>(result);
        Assert.True((bool)controller.ViewBag.HideApplicationForm);
        Assert.IsType<ApplySpecialistViewModel>(view.Model);
    }

    [Fact]
    public async Task Apply_Get_ShouldShowDeclinedWarning_WhenLatestApplicationWasDeclined()
    {
        var userId = Guid.NewGuid();
        var appService = new Mock<ISpecialistApplicationService>();
        appService.Setup(x => x.UserHasPendingApplicationAsync(userId)).ReturnsAsync(false);
        appService.Setup(x => x.UserIsAlreadySpecialistAsync(userId)).ReturnsAsync(false);
        appService.Setup(x => x.GetLatestByUserIdAsync(userId)).ReturnsAsync(new SpecialistApplicationViewModel { Id = 1, Status = RequestStatus.Declined, RejectionReason = "Need more experience" });
        appService.Setup(x => x.GetApplicationDraftAsync(userId)).ReturnsAsync(new ApplySpecialistViewModel { Occupation = "Therapist", ExperienceYears = 2 });
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(new Mock<IEmployeeService>().Object, appService.Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()), userId);

        var result = await controller.Apply();

        Assert.IsType<ViewResult>(result);
        Assert.Equal("Последната ви заявка беше отхвърлена.", controller.ViewBag.ApplicationWarningMessage);
    }

    [Fact]
    public async Task Apply_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(new Mock<IEmployeeService>().Object, new Mock<ISpecialistApplicationService>().Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()), Guid.NewGuid());
        controller.ModelState.AddModelError("Occupation", "Required");
        var model = new ApplySpecialistViewModel();

        var result = await controller.Apply(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(model, view.Model);
    }

    [Fact]
    public async Task Apply_Post_ShouldSubmitAndRedirect_WhenSuccessful()
    {
        var userId = Guid.NewGuid();
        var appService = new Mock<ISpecialistApplicationService>();
        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(new Mock<IEmployeeService>().Object, appService.Object, new Mock<IUserService>().Object, Mock.Of<ILogger<EmployeeController>>()), userId);
        var model = new ApplySpecialistViewModel { Occupation = "Therapist", ExperienceYears = 5, Biography = "Bio" };

        var result = await controller.Apply(model);

        appService.Verify(x => x.ApplyAsync(userId, model), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(EmployeeController.Apply), redirect.ActionName);
    }


    [Fact]
    public async Task Details_ShouldLoadProcedures_WhenCurrentUserOwnsSpecialistProfile()
    {
        var userId = Guid.NewGuid();
        var employeeService = new Mock<IEmployeeService>();
        employeeService.Setup(x => x.GetEmployeeByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new EmployeeInfoViewModel { Id = Guid.NewGuid(), UserId = userId, FullName = "Spec" });

        var userService = new Mock<IUserService>();
        userService.Setup(x => x.GetUserProfileAsync(userId)).ReturnsAsync(new GlowCare.ViewModels.Users.UserProfileViewModel
        {
            IsSpecialist = true,
            Procedures = new List<GlowCare.ViewModels.Users.UserProfileProcedureViewModel>
            {
                new() { Id = 1, ServiceName = "Massage", SpecialistName = "Spec", ClientName = "Client", AppointmentDate = "01.01.2026 10:00", Status = "Планирана", CanBeRejectedBySpecialist = true }
            }
        });

        var controller = ControllerTestHelpers.AttachHttpContext(new EmployeeController(employeeService.Object, new Mock<ISpecialistApplicationService>().Object, userService.Object, Mock.Of<ILogger<EmployeeController>>()), userId);

        var result = await controller.Details(Guid.NewGuid());

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EmployeeInfoViewModel>(view.Model);
        Assert.True(model.IsCurrentSpecialistOwner);
        Assert.Single(model.Procedures);
    }

}

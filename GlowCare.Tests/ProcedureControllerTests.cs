using GlowCare.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class ProcedureControllerTests
{
    [Fact]
    public async Task Add_Get_ShouldLoadDropdownsAndReturnView()
    {
        var procedureService = new Mock<IProcedureService>();
        procedureService.Setup(x => x.GetEmployeeSelectListAsync()).ReturnsAsync(new List<SelectListItem> { new() { Value = "1", Text = "Emp" } });
        procedureService.Setup(x => x.GetServiceSelectListAsync()).ReturnsAsync(new List<SelectListItem> { new() { Value = "1", Text = "Srv" } });
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object));

        var result = await controller.Add();

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<IndexViewModel>(view.Model);
        Assert.NotNull(controller.ViewBag.Employees);
        Assert.NotNull(controller.ViewBag.Services);
    }

    [Fact]
    public async Task Add_Post_ShouldRedirectToLogin_WhenUserIsNotAuthenticated()
    {
        var procedureService = new Mock<IProcedureService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object));
        controller.ControllerContext.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());

        var result = await controller.Add(new IndexViewModel());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public async Task Add_Post_ShouldRedirectHome_WhenModelStateIsInvalid()
    {
        var procedureService = new Mock<IProcedureService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object), Guid.NewGuid());
        controller.ModelState.AddModelError("AppointmentDate", "Required");

        var result = await controller.Add(new IndexViewModel());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Add_Post_ShouldRedirectToLogin_WhenUserIdIsInvalid()
    {
        var procedureService = new Mock<IProcedureService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns("bad-guid");
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object), Guid.NewGuid());

        var result = await controller.Add(new IndexViewModel());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    [Fact]
    public async Task Add_Post_ShouldRedirectHome_WhenSlotIsUnavailable()
    {
        var procedureService = new Mock<IProcedureService>();
        procedureService.Setup(x => x.IsSlotAvailableAsync(It.IsAny<Guid>(), 5, It.IsAny<DateTime>()))
            .ReturnsAsync(new AvailabilityCheckResultViewModel { IsAvailable = false, Message = "Taken" });
        var userId = Guid.NewGuid();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object), userId);

        var result = await controller.Add(new IndexViewModel { EmployeeId = Guid.NewGuid(), ServiceId = 5, AppointmentDate = DateTime.UtcNow });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task Add_Post_ShouldCreateProcedure_WhenRequestIsValid()
    {
        var procedureService = new Mock<IProcedureService>();
        procedureService.Setup(x => x.IsSlotAvailableAsync(It.IsAny<Guid>(), 5, It.IsAny<DateTime>()))
            .ReturnsAsync(new AvailabilityCheckResultViewModel { IsAvailable = true, Message = "OK" });
        var userId = Guid.NewGuid();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object), userId);
        var model = new IndexViewModel { EmployeeId = Guid.NewGuid(), ServiceId = 5, AppointmentDate = DateTime.UtcNow };

        var result = await controller.Add(model);

        procedureService.Verify(x => x.CreateProcedureAsync(model, userId), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    [Fact]
    public async Task CheckAvailability_ShouldReturnPartialViewResult()
    {
        var expected = new AvailabilityCheckResultViewModel { IsAvailable = true, Message = "Free" };
        var procedureService = new Mock<IProcedureService>();
        procedureService.Setup(x => x.IsSlotAvailableAsync(It.IsAny<Guid>(), 1, It.IsAny<DateTime>())).ReturnsAsync(expected);
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), ControllerTestHelpers.CreateUserManagerMock().Object));

        var result = await controller.CheckAvailability(Guid.NewGuid(), 1, DateTime.UtcNow);

        var partial = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_CheckAvailabilityPopup", partial.ViewName);
        Assert.Same(expected, partial.Model);
    }

    [Fact]
    public async Task GetServicesByEmployee_ShouldReturnJsonResult()
    {
        var services = new List<SelectListItem> { new() { Value = "1", Text = "Massage" } };
        var procedureService = new Mock<IProcedureService>();
        procedureService.Setup(x => x.GetServicesByEmployeeIdAsync(It.IsAny<Guid>())).ReturnsAsync(services);
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), ControllerTestHelpers.CreateUserManagerMock().Object));

        var result = await controller.GetServicesByEmployee(Guid.NewGuid());

        var json = Assert.IsType<JsonResult>(result);
        Assert.Same(services, json.Value);
    }

    [Fact]
    public async Task Reject_ShouldRedirectToLogin_WhenUserIdIsMissing()
    {
        var procedureService = new Mock<IProcedureService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns((string?)null);
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object));

        var result = await controller.Cancel(7);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Identity/Account/Login", redirect.Url);
    }

    [Fact]
    public async Task Cancel_ShouldRedirectToLogin_WhenUserIdIsMissing()
    {
        var procedureService = new Mock<IProcedureService>();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns((string?)null);
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object));

        var result = await controller.Cancel(7);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/Identity/Account/Login", redirect.Url);
    }

    [Fact]
    public async Task Cancel_ShouldRedirectToProfile_WhenCancellationSucceeds()
    {
        var procedureService = new Mock<IProcedureService>();
        var userId = Guid.NewGuid();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object), userId);

        var result = await controller.Cancel(11);

        procedureService.Verify(x => x.CancelProcedureAsync(11, userId), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Profile", redirect.ControllerName);
        Assert.Equal("Процедурата беше отказана успешно.", controller.TempData["ProfileMessage"]);
    }

    [Fact]
    public async Task Reject_ShouldRedirectToProfile_WhenRejectSucceeds()
    {
        var procedureService = new Mock<IProcedureService>();
        var userId = Guid.NewGuid();
        var userManager = ControllerTestHelpers.CreateUserManagerMock();
        userManager.Setup(x => x.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId.ToString());
        var controller = ControllerTestHelpers.AttachHttpContext(new ProcedureController(procedureService.Object, Mock.Of<ILogger<ProcedureController>>(), userManager.Object), userId);

        var result = await controller.Cancel(9);

        procedureService.Verify(x => x.CancelProcedureAsync(9, userId), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Profile", redirect.ControllerName);
        Assert.Equal("Процедурата беше отказана успешно.", controller.TempData["ProfileMessage"]);
    }

}


using GlowCare.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.Models;
using GlowCare.ViewModels.Services;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class HomeControllerTests
{
    [Fact]
    public async Task Index_ShouldLoadEmployeesAndServices()
    {
        var procedureService = new Mock<IProcedureService>();
        var serviceService = new Mock<IServiceService>();
        procedureService.Setup(x => x.GetEmployeeSelectListAsync()).ReturnsAsync(new List<SelectListItem> { new() { Value = "1", Text = "Emp" } });
        serviceService.Setup(x => x.GetAllServicesAsync()).ReturnsAsync(new List<ServiceInfoViewModel> { new() { Id = 1, Name = "Massage" } });
        var controller = new HomeController(procedureService.Object, serviceService.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<IndexViewModel>(view.Model);
        Assert.Single(model.ServicesInfo);
        Assert.NotNull(controller.ViewBag.Employees);
    }

    [Fact]
    public void Error_ShouldReturnErrorViewModel()
    {
        var controller = new HomeController(new Mock<IProcedureService>().Object, new Mock<IServiceService>().Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };

        var result = controller.Error();

        var view = Assert.IsType<ViewResult>(result);
        Assert.IsType<ErrorViewModel>(view.Model);
    }
}

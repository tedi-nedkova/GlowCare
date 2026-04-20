using GlowCare.Controllers;
using GlowCare.Core.Contracts;
using GlowCare.ViewModels.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GlowCare.Tests;

public class ServiceControllerTests
{
    [Fact]
    public async Task Index_ShouldReturnViewWithPaginatedServicesAndFilters()
    {
        var procedureService = new Mock<IProcedureService>();
        var serviceService = new Mock<IServiceService>();
        serviceService.Setup(x => x.GetFilteredServicesAsync(2, "50-100", "morning"))
            .ReturnsAsync(Enumerable.Range(1, 8).Select(i => new ServiceInfoViewModel { Id = i, Name = $"Service {i}" }));
        serviceService.Setup(x => x.GetCategoryOptionsAsync())
            .ReturnsAsync(new List<ServiceCategoryOptionViewModel> { new() { Id = 2, Name = "Massage" } });

        var controller = new ServiceController(procedureService.Object, serviceService.Object, Mock.Of<ILogger<HomeController>>());

        var result = await controller.Index(2, "50-100", "morning", 2);

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ServiceIndexViewModel>(view.Model);
        Assert.Equal(2, model.SelectedCategoryId);
        Assert.Equal("50-100", model.SelectedPriceRange);
        Assert.Equal("morning", model.SelectedAvailabilityRange);
        Assert.Equal(2, ((List<ServiceInfoViewModel>)model.Services).Count);
        Assert.Equal(2, controller.ViewBag.CurrentPage);
        Assert.Equal(2, controller.ViewBag.TotalPages);
    }
}

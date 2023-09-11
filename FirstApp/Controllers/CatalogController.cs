using FirstApp.Models;
using FirstApp.Services;
using FirstApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FirstApp.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly ICatalogViewModelService _catalogVMServer;

        public CatalogController(ILogger<CatalogController> logger, ICatalogViewModelService catalogVMServer)
        {
            _logger = logger;
            _catalogVMServer = catalogVMServer;
        }

        public IActionResult Index(CatalogIndexViewModel catalogModel, int? pageId)
        {
//            CatalogIndexViewModel vm = _catalogVMServer.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.BrandFilterApplied, catalogModel.TypesFilterApplied);
//            return View("CatalogIndex", vm);
            return View("CatalogIndex", null);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
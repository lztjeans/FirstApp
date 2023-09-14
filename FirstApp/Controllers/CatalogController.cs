using FirstApp.Services;
using FirstApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NLog;

namespace FirstApp.Controllers
{
    public class CatalogController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICatalogViewModelService _catalogViewModelService;

        public CatalogController(ICatalogViewModelService catalogViewModelService)
        {
            _catalogViewModelService = catalogViewModelService;
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> IndexAsync()
        //{
        //    ////return View();
        //    ////return View(Repository.AllEmpoyees);
        //    var vm = await _catalogViewModelService.GetCatalogItems(0, Constants.ITEMS_PER_PAGE, null, null);
        //    return View(vm);
        //}



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(CatalogIndexViewModel vm, int? pageId)
        {
            vm = await _catalogViewModelService.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, vm.BrandFilterApplied, vm.TypesFilterApplied);
            return View(vm);
        }

        //public async Task OnGet(CatalogIndexViewModel catalogModel, int? pageId)
        //{
        //    var CatalogModel = await _catalogViewModelService.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.BrandFilterApplied, catalogModel.TypesFilterApplied);
        //}
    }

}


using System.Collections.Generic;
using System.Threading.Tasks;
using FirstApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FirstApp.Services;

public interface ICatalogViewModelService
{
    Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
    Task<IEnumerable<SelectListItem>> GetBrands();
    Task<IEnumerable<SelectListItem>> GetTypes();
}

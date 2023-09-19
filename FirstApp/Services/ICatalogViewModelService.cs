using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using FirstApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using FirstApp.Models;

namespace FirstApp.Services;

public interface ICatalogViewModelService
{
    Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
    Task<IEnumerable<SelectListItem>> GetBrands();
    Task<IEnumerable<SelectListItem>> GetTypes();

    Task<IEnumerable<CatalogItemViewModel>> GetAllItems();
    CatalogItemViewModel CreateItem(CatalogItem? entity);
    Task<CatalogItemViewModel> GetItemById(int id);
    void DeleteItem(CatalogItem entity);
}

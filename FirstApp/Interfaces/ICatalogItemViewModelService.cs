using System.Threading.Tasks;
using FirstApp.ViewModels;

namespace FirstApp.Services;

public interface ICatalogItemViewModelService
{
    Task UpdateCatalogItem(CatalogItemViewModel viewModel);
}

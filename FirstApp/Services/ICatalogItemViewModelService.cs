using System.Threading.Tasks;
using FirstApp.ViewModels;

namespace FirstApp.Interfaces;

public interface ICatalogItemViewModelService
{
    Task UpdateCatalogItem(CatalogItemViewModel viewModel);
}

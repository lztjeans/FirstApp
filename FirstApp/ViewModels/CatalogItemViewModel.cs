using FirstApp.Models;
using FirstApp.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirstApp.ViewModels;

public class CatalogItemViewModel
{
    public CatalogItemViewModel()
    {
    }

    public CatalogItemViewModel(CatalogItem i)
    {
        Id = i.Id;
        Name = i.Name;
        //PictureUri = new UriComposer().ComposePicUri(i.PictureUri);
        Price = i.Price;
        Description = i.Description;
        TypeId = i.CatalogTypeId;
        BrandId = i.CatalogBrandId;
    }

    public int Id { get; set; }
    public string? Name { get; set; }
    public string? PictureUri { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; } = string.Empty;

    public int BrandId { get; set; }
    public string? BrandName { get; set; }

    public int TypeId { get; set; }
    public string? TypeName { get; set; }

    internal CatalogItem ToEntity()
    {
        return new CatalogItem(TypeId, BrandId, Description, Name, Price, PictureUri);
    }

}
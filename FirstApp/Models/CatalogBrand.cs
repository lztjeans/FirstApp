

namespace FirstApp.Models;

public class CatalogBrand : BaseEntity, IAggregateRoot
{
    public string Brand { get; private set; }
    public CatalogBrand(string brand)
    {
        Brand = brand;
    }

    public CatalogBrand(int id, string brand)
    {
        base.Id = id;
        Brand = brand;
    }


}

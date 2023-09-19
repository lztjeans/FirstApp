
namespace FirstApp.Models;

public class CatalogType : BaseEntity, IAggregateRoot
{
    public string Type { get; private set; }
    public CatalogType(string type)
    {
        Type = type;
    }
    public CatalogType(int id, string type)
    {
        base.Id = id;
        Type = type;
    }
}

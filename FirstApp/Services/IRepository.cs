using Ardalis.Specification;

namespace FirstApp.Models;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}

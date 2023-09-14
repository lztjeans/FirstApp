using Ardalis.Specification;
using FirstApp.Models;

namespace FirstApp.Services;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}

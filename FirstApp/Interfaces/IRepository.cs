using Ardalis.GuardClauses;
using Ardalis.Specification;

namespace FirstApp.Interfaces;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}

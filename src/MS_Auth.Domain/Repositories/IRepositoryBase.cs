using System.Linq.Expressions;

namespace MS_Auth.Domain.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<T> ObterPorFiltroAsync(Expression<Func<T, bool>> predicate);
    Task<T> CriarAsync(T entity);
    Task<T> AtualizarAsync(T entity);
    Task<bool> ExcluirAsync(Guid id);
    Task<List<T>> LerTodosAsync();
}
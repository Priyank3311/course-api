using System.Linq.Expressions;

namespace Course.Repositories.BaseRepository;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<List<T>> GetDataAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int page = 1, int size = 10);
    Task<bool> DeleteIfAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, Func<T, bool>? condition = null);
    Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector,Func<IQueryable<T>, IQueryable<T>>? include = null);
}

using System.Linq.Expressions;
using Course.DataModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace Course.Repositories.BaseRepository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly StudentCourseDbContext _context;

    public BaseRepository(StudentCourseDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<T>> GetDataAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int page = 1, int size = 10)
    {
        IQueryable<T> query = _context.Set<T>();

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);

        return await query
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<bool> DeleteIfAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, Func<T, bool>? condition = null)
    {
        var query = _context.Set<T>().AsQueryable();

        if (include != null)
            query = include(query);

        var entity = await query.FirstOrDefaultAsync(predicate);
        if (entity == null || (condition != null && !condition(entity)))
            return false;

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<TResult>> GetProjectedAsync<TResult>(Expression<Func<T, bool>> predicate,Expression<Func<T, TResult>> selector,Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        var query = _context.Set<T>().Where(predicate);

        if (include != null)
            query = include(query);

        return await query.Select(selector).ToListAsync();
    }

}

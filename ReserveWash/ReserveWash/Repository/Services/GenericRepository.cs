using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;


    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<IQueryable<T>> GetAllAsync()
    {
        return await Task.FromResult(_dbSet.AsQueryable());
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> GetByIdAsyncAsQuery(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // اعمال Include برای بارگذاری داده‌های مرتبط
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // فیلتر کردن بر اساس شناسه
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }


    public async Task AddAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        }

        try
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        catch (Exception ex)
        {
            throw new Exception("Error adding entity to the database", ex);
        }
    }


    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task DeleteAsync(IEnumerable<T> entities)
    {
        if (entities.Any())
        {
            _dbSet.RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}

using System.Linq;
using System.Threading.Tasks;

public interface IGenericRepository<T> where T : class
{
    Task<IQueryable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

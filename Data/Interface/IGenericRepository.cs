using Data.Contexts;

namespace Data.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        DataContext Context { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task<T> CreateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object id);
        Task RollbackTransactionsAync();
        Task UpdateAsync(T entity);
    }
}
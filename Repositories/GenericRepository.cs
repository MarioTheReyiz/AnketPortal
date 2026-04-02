using AnketPortal.API.Data;
using Microsoft.EntityFrameworkCore;

namespace AnketPortal.API.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        // Bu metot çağrılmadan SQL tarafında hiçbir değişiklik kalıcı olmaz.
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        // YENİ EKLENDİ: Metodun gövdesi
        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}
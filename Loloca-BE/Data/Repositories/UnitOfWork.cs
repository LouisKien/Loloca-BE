using Loloca_BE.Data.Entities;
using System.Drawing.Drawing2D;

namespace Loloca_BE.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LolocaDbContext _dbContext;

        public UnitOfWork(LolocaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}

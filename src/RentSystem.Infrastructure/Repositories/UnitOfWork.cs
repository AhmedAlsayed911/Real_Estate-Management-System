using Microsoft.EntityFrameworkCore.Storage;
using RentSystem.Domain.Interfaces;
using RentSystem.Infrastructure.Data;

namespace RentSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IPropertyRepository Properties { get; }
        public IBookingRepository Bookings { get; }
        public IReviewRepository Reviews { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepository,
            IPropertyRepository propertyRepository,
            IBookingRepository bookingRepository,
            IReviewRepository reviewRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _context = context;
            Users = userRepository;
            Properties = propertyRepository;
            Bookings = bookingRepository;
            Reviews = reviewRepository;
            RefreshTokens = refreshTokenRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

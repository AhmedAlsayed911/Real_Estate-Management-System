namespace RentSystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IPropertyRepository Properties { get; }
        IBookingRepository Bookings { get; }
        IReviewRepository Reviews { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

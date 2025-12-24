using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Businesses = new BusinessRepository(_context);
        Members = new MemberRepository(_context);
        SubscriptionPlans = new SubscriptionPlanRepository(_context);
        Memberships = new MembershipRepository(_context);
        Payments = new PaymentRepository(_context);
        AuditLogs = new Repository<AuditLog>(_context);
        Notifications = new Repository<Notification>(_context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Business> Businesses { get; }
    public IRepository<Member> Members { get; }
    public IRepository<SubscriptionPlan> SubscriptionPlans { get; }
    public IRepository<Membership> Memberships { get; }
    public IRepository<Payment> Payments { get; }
    public IRepository<AuditLog> AuditLogs { get; }
    public IRepository<Notification> Notifications { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
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




using Microsoft.EntityFrameworkCore.Storage;
using ShippingSvc.Application.Abstractions;

namespace ShippingSvc.Infrastructure.Persistence;

public sealed class EfUnitOfWork(ShippingDbContext db) : IUnitOfWork
{
    private readonly ShippingDbContext _db = db;
    private IDbContextTransaction? _tx;

    public async Task BeginAsync(CancellationToken ct) =>
        _tx = await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
        await (_tx?.CommitAsync(ct) ?? Task.CompletedTask);
    }

    public Task RollbackAsync(CancellationToken ct) =>
        _tx?.RollbackAsync(ct) ?? Task.CompletedTask;
}
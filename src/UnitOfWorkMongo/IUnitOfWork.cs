using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace UnitOfWorkMongo
{
    public interface IUnitOfWork
    {
        Task<TResult> WithTransactionAsync<TResult>(Func<IClientSessionHandle, CancellationToken, Task<TResult>> callbackAsync, CancellationToken cancellationToken = default);
        TResult WithTransaction<TResult>(Func<IClientSessionHandle, CancellationToken, TResult> callback, CancellationToken cancellationToken);
    }
}

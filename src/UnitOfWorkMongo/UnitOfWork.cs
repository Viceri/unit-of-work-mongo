using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace UnitOfWorkMongo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoClient mongoClient;
        private readonly TransactionOptions transactionOptions;
        public UnitOfWork(IMongoClient mongoClient, TransactionOptions transactionOptions)
        {
            this.transactionOptions = transactionOptions;
            this.mongoClient = mongoClient;

        }

        public async Task<TResult> WithTransactionAsync<TResult>(Func<IClientSessionHandle, CancellationToken, Task<TResult>> callbackAsync, CancellationToken cancellationToken = default)
        {
            using var session = await mongoClient.StartSessionAsync();

            return await session.WithTransactionAsync(callbackAsync, transactionOptions, cancellationToken);
        }

        public TResult WithTransaction<TResult>(Func<IClientSessionHandle, CancellationToken, TResult> callback, CancellationToken cancellationToken = default)
        {
            using var session = mongoClient.StartSession();

            return session.WithTransaction(callback, transactionOptions, cancellationToken);
        }
    }
}

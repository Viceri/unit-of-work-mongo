using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NSubstitute;
using UnitOfWorkMongo;
using Xunit;

namespace UnitOfWork.Tests
{
    public class UnitOfWorkTest
    {
        private readonly TransactionOptions transactionOptions;
        private readonly CancellationToken cancellationToken;
        private readonly IMongoClient mongoClient;
        private readonly IClientSessionHandle clientSessionHandle;
        private readonly IUnitOfWork unitOfWork;

        public UnitOfWorkTest()
        {
            transactionOptions = new TransactionOptions(readPreference: ReadPreference.Primary, readConcern: ReadConcern.Local, writeConcern: WriteConcern.WMajority);
            cancellationToken = CancellationToken.None;

            clientSessionHandle = Substitute.For<IClientSessionHandle>();
            clientSessionHandle
                .WithTransactionAsync(
                    Arg.Any<Func<IClientSessionHandle, CancellationToken, Task<bool>>>(), 
                    Arg.Any<TransactionOptions>(), 
                    Arg.Any<CancellationToken>())
                .Returns(true);

            clientSessionHandle
                .WithTransaction(
                    Arg.Any<Func<IClientSessionHandle, CancellationToken, bool>>(), 
                    Arg.Any<TransactionOptions>(), 
                    Arg.Any<CancellationToken>())
                .Returns(true);

            mongoClient = Substitute.For<IMongoClient>();
            mongoClient.StartSessionAsync().Returns(clientSessionHandle);
            mongoClient.StartSession().Returns(clientSessionHandle);

            unitOfWork = new UnitOfWorkMongo.UnitOfWork(mongoClient, transactionOptions);
        }

        [Fact]
        public async Task Should_Run_CallbackAsync()
        {
            var result = await unitOfWork.WithTransactionAsync((s, ct) => Task.FromResult(true), cancellationToken);

            Assert.True(result);

            await mongoClient.Received().StartSessionAsync();

            await clientSessionHandle.Received().WithTransactionAsync(Arg.Any<Func<IClientSessionHandle, CancellationToken, Task<bool>>>(), transactionOptions, cancellationToken);
        }

        [Fact]
        public void Should_Run_Callback()
        {
            var result = unitOfWork.WithTransaction((s, ct) => true, cancellationToken);

            Assert.True(result);

            mongoClient.Received().StartSession();

            clientSessionHandle.Received().WithTransaction(Arg.Any<Func<IClientSessionHandle, CancellationToken, bool>>(), transactionOptions, cancellationToken);
        }
    }
}

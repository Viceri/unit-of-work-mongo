# Unit of work mongodb

> Para aplicar o pattern Unit of Work no mongo é preciso habilitar o replica set, senão o seguinte erro irá retornar:
> **This MongoDB deployment does not support retryable writes. Please add retryWrites=false to your connection string.**
> Para mais informações veja a documentação do mongodb sobre o [retry writes](https://docs.mongodb.com/manual/core/retryable-writes/).

> Se estiver usando [docker](https://github.com/docker-library/mongo/issues/55).

## nuget link
    link nuget: https://www.nuget.org/packages/UnitOfWorkMongo/

## Install
   
    Install-Package UnitOfWorkMongo
    
## Usage

```csharp

    var mongoClient = new MongoClient("mongodb://localhost");
    var transactionOptions = new TransactionOptions(readPreference: ReadPreference.Primary, readConcern: ReadConcern.Local, writeConcern: WriteConcern.WMajority);
    var unitOfWorkMongo =  new UnitOfWorkMongo.UnitOfWork(mongoClient, transactionOptions); 
    
    var database1 = client.GetDatabase("mydb1");
    var collection1 = database1.GetCollection<BsonDocument>("foo").WithWriteConcern(WriteConcern.WMajority);
    collection1.InsertOne(new BsonDocument("abc", 0));
    
    var database2 = client.GetDatabase("mydb2");
    var collection2 = database2.GetCollection<BsonDocument>("bar").WithWriteConcern(WriteConcern.WMajority);
    collection2.InsertOne(new BsonDocument("xyz", 0));
    
    await unitOfWorkMongo.WithTransactionAsync(async (s, c) =>
    {
    	collection1.InsertOne(s, new BsonDocument("abc", 1), cancellationToken: ct);
    	collection2.InsertOne(s, new BsonDocument("xyz", 999), cancellationToken: ct);
    	return "Inserted into collections in different databases";
    },
    CancellationToken.None);
```

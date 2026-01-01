module MongoAltasTests
open System
open Xunit
open System.IO
open System.Text.Json
open MongoDB.Bson
open MongoDB.Driver
open BasicObjects
open MongoDAL

type MongoAltasTestsFixture() =
    let connectionUri = JsonDocument.Parse(File.ReadAllText("appsettings.json")).RootElement.GetProperty("connectionUri").GetString()

    [<Fact>]
    let ``appsettings.json works`` () =
        let expected = "mongodb+srv://wcdeich4_db_user:8L#v3M#ng#DB@cluster0.x01hubz.mongodb.net/?appName=Cluster0"
        Assert.Equal(connectionUri, expected)
        0
        
    [<Fact>]
    let ``insert and find`` () =
        let mongoContext: IMongoContext = new MongoContext(connectionUri)
        let newEntry = new KeyValueWithId()
        newEntry.Id <- ObjectId.GenerateNewId()
        newEntry.Key <- "TestKey1"
        newEntry.Value <- "TestValue1"
        let emptyList = List.empty<KeyValueWithId>
        let KeyValueWithIdList = newEntry :: emptyList
        mongoContext.Insert<KeyValueWithId>("TestDB1", "KeyValueWithIdTable", KeyValueWithIdList)

        let found = mongoContext.Find<KeyValueWithId>("TestDB1", "KeyValueWithIdTable", Builders<KeyValueWithId>.Filter.Eq("Key", "TestKey1"))
        let mutable foundInsertedID = false
        for item in found do
            if item.Id = KeyValueWithIdList.[0].Id then
                foundInsertedID <- true
        
        Assert.True(foundInsertedID)
        0

    [<Fact>]
    let ``insert and delete`` () =
        let mongoContext: IMongoContext = new MongoContext(connectionUri)
        let newEntry = new KeyValueWithId()
        newEntry.Id <- ObjectId.GenerateNewId()
        newEntry.Key <- "TestKey1"
        newEntry.Value <- "TestValue1"
        let emptyList = List.empty<KeyValueWithId>
        let KeyValueWithIdList = newEntry :: emptyList
        mongoContext.Insert<KeyValueWithId>("TestDB1", "KeyValueWithIdTable", KeyValueWithIdList)

        let filter = Builders<KeyValueWithId>.Filter.Eq("Key", "TestKey1")
        mongoContext.Delete<KeyValueWithId>("TestDB1", "KeyValueWithIdTable", filter)

        let found = mongoContext.Find<KeyValueWithId>("TestDB1", "KeyValueWithIdTable", filter)
        let mutable foundInsertedID = false
        for item in found do
            if item.Id = KeyValueWithIdList.[0].Id then
                foundInsertedID <- true
        
        Assert.False(foundInsertedID)
        0
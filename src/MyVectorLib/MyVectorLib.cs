using Milvus.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milvus.Client.Grpc;

namespace MyVectorLib
{
    public class VectorDbService
    {
        private readonly MilvusClient _client;

        public VectorDbService(IConfiguration configuration)
        {
            var milvusAddress = configuration["Milvus:Address"] ?? throw new ArgumentNullException("Milvus:Address configuration is missing");
            //_client = new MilvusClient(milvusAddress);
            _client = new MilvusClient(configuration["Milvus:Address"], 
                Convert.ToInt32(configuration["Milvus:Port"]), 
                false);
                // configuration["Milvus:Username"], 
                // configuration["Milvus:Password"], 
                // configuration["Milvus:Database"]);
            //MilvusHealthState result = await milvusClient.HealthAsync();
        }

        public async Task CheckDatabaseAsync()
        {
            var result = await _client.HealthAsync();

            if (result.IsHealthy)
            {
                Console.WriteLine("Milvus healthy is fine!.");
                var version = await _client.GetVersionAsync();
                Console.WriteLine($"Versio: {version}");
                var collections = await _client.ListCollectionsAsync();
                if (collections != null)
                {
                    Console.WriteLine("Milvus is connected and collections are listed.");
                    await _client.FlushAllAsync();
                }
                else
                {
                    Console.WriteLine("Failed to list collections.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to check status: {result.ErrorMsg}.");
            }
        }

        // public async Task InsertVectorAsync(string collectionName, float[] vector)
        // {
        //     MilvusCollection collection = _client.GetCollection(collectionName);
        //     Console.WriteLine("MilvusCollection: {1}-{0}",collection.ToString(), collectionName);
        //     //Check if this collection exists
        //     var hasCollection = await _client.HasCollectionAsync(collectionName);

        //     if(hasCollection){
        //         await collection.DropAsync();
        //         Console.WriteLine("Drop collection {0}",collectionName);
        //     }

        //     await _client.CreateCollectionAsync(
        //         collectionName,
        //         new[] {
        //             FieldSchema.Create<long>("file_id", isPrimaryKey:true),
        //             FieldSchema.Create<long>("word_count"),
        //             FieldSchema.CreateVarchar("file_name", 256),
        //             FieldSchema.CreateFloatVector("file_intro", 2)
        //         }
        //     );

        //     // var collection = _client.GetCollection(collectionName);
        //     // var fieldData = new List<FieldData>
        //     // {
        //     //     new FloatVectorFieldData { Vectors = new[] { vector } }
        //     // };
        //     collection = _client.GetCollection(collectionName);
        //      //await collection.InsertAsync(new() { new() { 0.1f, 0.2f } });
        //      Console.WriteLine("Vector inserted successfully.");
        // }

 public async Task InsertVectorAsync(string collectionName, float[] vector)
{
    MilvusCollection collection = _client.GetCollection(collectionName);
    Console.WriteLine("MilvusCollection: {1}-{0}", collection.ToString(), collectionName);

    // Check if the collection exists
    bool hasCollection = await _client.HasCollectionAsync(collectionName);

    if (hasCollection)
    {
        await collection.DropAsync();
        Console.WriteLine("Dropped collection {0}", collectionName);
    }

    // Create the collection
    await _client.CreateCollectionAsync(
        collectionName,
        new[]
        {
            FieldSchema.Create<long>("file_id", isPrimaryKey: true, autoId: true),
            FieldSchema.Create<long>("word_count"),
            FieldSchema.CreateVarchar("file_name", 256),
            //FieldSchema.CreateFloatVector("file_intro", vector.Length) // Ensure the dimension matches the vector length
        }
    );

    // Prepare the data to be inserted
    var fieldData = new List<FieldData>
    {
        //new FieldData<long>("file_id", new List<long> { 1L }),
        new FieldData<long>("word_count", new List<long> { 100L }),
        new FieldData<string>("file_name", new List<string> { "example.txt" }),
        //new FieldData<float[]>("file_intro", new List<float[]> { vector }) // Corrected line
    };

    Console.WriteLine($"fieldData: {fieldData}");

    // Insert the vector into the collection
    var insertResult = await collection.InsertAsync(fieldData);
    Console.WriteLine("Vector inserted successfully.");

    // Optionally, you can flush the collection to ensure the data is persisted
    await collection.FlushAsync();
    Console.WriteLine("Collection flushed successfully.");

    // Create index on the vector field
        await CreateIndexAsync(collectionName, "file_intro");
}
public async Task CreateIndexAsync(string collectionName, string fieldName)
    {
        var indexParams = new Dictionary<string, string>
        {
            { "index_type", "IVF_FLAT" }, // You can use other index types like IVF_SQ8, HNSW, etc.
            { "metric_type", "L2" }, // L2 is the Euclidean distance metric
            { "params", "{\"nlist\":128}" } // Additional parameters for the index
        };

        //await _client.CreateIndexAsync(collectionName, fieldName, indexParams);
        Console.WriteLine("Index created successfully on field {0}.", fieldName);
    }

    public async Task LoadCollectionAsync(string collectionName)
    {
        try
        {
            var collection = _client.GetCollection(collectionName);
            await collection.LoadAsync();
            Console.WriteLine($"Collection '{collectionName}' loaded successfully.");
        }
        catch (MilvusException ex)
        {
            Console.WriteLine($"Failed to load collection '{collectionName}': {ex.Message}");
        }
    }

    public async Task SearchAsync(string collectionName, float[] queryVector, int topK)
    {
        var searchParams = new Dictionary<string, string>
        {
            { "metric_type", "L2" },
            { "params", "{\"nprobe\":10}" }
        };

        // var searchResults = await _client.SearchAsync<float>(
        //     collectionName,
        //     new List<float[]> { queryVector },
        //     topK,
        //     searchParams
        // );

        // foreach (var result in searchResults)
        // {
        //     Console.WriteLine($"ID: {result.ID}, Distance: {result.Distance}");
        // }
    }


        public async Task UpdateVectorAsync(string collectionName, string id, float[] vector)
        {
        //     var collection = _client.GetCollection(collectionName);
        //     var fieldData = new List<FieldData>
        //     {
        //         new FloatVectorFieldData { Vectors = new[] { vector } }
        //     };
        //     await collection.DeleteAsync(id);
        //     await collection.InsertAsync(fieldData);
        //     Console.WriteLine("Vector updated successfully.");
        }

        public async Task DeleteVectorAsync(string collectionName, string id)
        {
            var collection = _client.GetCollection(collectionName);
            await collection.DeleteAsync(id);
            Console.WriteLine("Vector deleted successfully.");
        }


        public async Task SearchVectorAsync(string collectionName, float[] queryVector)
        {
            //MilvusCollectionLoader loader = new MilvusCollectionLoader(_client);

        //string collectionName = "your_collection_name";
        //await loader.LoadCollectionAsync(collectionName);

var collection = _client.GetCollection(collectionName);
            await collection.LoadAsync();

            // var collection = _client.GetCollection(collectionName);
            var results = await collection.SearchAsync(
                "file_intro",
                new List<ReadOnlyMemory<float>> { queryVector },
                SimilarityMetricType.L2,
                limit: 10);

            Console.WriteLine($"result: {results.ToString()}");
            // foreach (var result in results)
            // {
            //     Console.WriteLine($"Found vector with ID: {result.Id}");
            // }

            // Search
            List<string> search_output_fields = new() { "file_id" };
            List<List<float>> search_vectors = new() { new() { 0.1f, 0.2f } };
            SearchResults searchResult = await collection.SearchAsync(
                "file_intro",
                //new ReadOnlyMemory<float>[] { new[] { 0.1f, 0.2f } },
                new ReadOnlyMemory<float>[] { queryVector },
                SimilarityMetricType.L2,
                limit: 2);

            // Query
            string expr = "file_id in [2,4,6,8]";

            QueryParameters queryParameters = new ();
            queryParameters.OutputFields.Add("file_id");
            queryParameters.OutputFields.Add("word_count");

            IReadOnlyList<FieldData> queryResult = await collection.QueryAsync(
                expr,
                queryParameters);
        }

        public async Task PrintCollectionDetailsAsync(string collectionName)
        {
            // var collection = _client.GetCollection(collectionName);
            // var collectionSchema = await collection.GetSchemaAsync();
            // Console.WriteLine($"Collection Name: {collectionName}");
            // Console.WriteLine("Fields:");
            // foreach (var field in collectionSchema.Fields)
            // {
            //     Console.WriteLine($"- {field.Name} (Type: {field.DataType}, Primary Key: {field.IsPrimaryKey}, Max Length: {field.MaxLength})");
            // }
        }
    }
}

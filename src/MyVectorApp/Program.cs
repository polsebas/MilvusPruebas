// using System;
// using Milvus.Client;

// string Host = "localhost";
// int Port = 19530; // This is Milvus's default port
// bool UseSsl = false; // Default value is false
// string Database = "my_database"; // Defaults to the default Milvus database

// // See documentation for other constructor paramters
// MilvusClient milvusClient = new MilvusClient(Host, Port, UseSsl);
// MilvusHealthState result = await milvusClient.HealthAsync();
// Console.WriteLine($"result: {result.IsHealthy.ToString()}");

using Microsoft.Extensions.Configuration;
using MyVectorLib;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Configuración
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var vectorService = new VectorDbService(configuration);

        // Ejemplo de uso
        await vectorService.CheckDatabaseAsync();
        for(var i =0; i < 10; i++)
        {
            await vectorService.InsertVectorAsync($"example_collection_{i}", new float[] { 0.1f, 0.2f, 0.3f });
            
        }   
        await vectorService.InsertVectorAsync($"book_id", new float[] { 0.1f, 0.2f });     

        await vectorService.PrintCollectionDetailsAsync("example_collection_");

        await vectorService.SearchVectorAsync("example_collection", new float[] { 0.1f, 0.2f, 0.3f });
        await vectorService.SearchVectorAsync("book_id", new float[] { 0.1f, 0.2f });    
    }
}

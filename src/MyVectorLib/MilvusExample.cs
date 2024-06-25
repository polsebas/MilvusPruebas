// using Milvus.Client;
// using Milvus.Client.Grpc;
// using System;
// using System.Threading.Tasks;

// public class MilvusCollectionLoader
// {
//     private readonly MilvusClient _milvusClient;

//     public MilvusCollectionLoader(MilvusClient milvusClient)
//     {
//         _milvusClient = milvusClient;
//     }

//     public async Task LoadCollectionAsync(string collectionName)
//     {
//         try
//         {
//             await _milvusClient.LoadCollectionAsync(collectionName);
//             Console.WriteLine($"Collection '{collectionName}' loaded successfully.");
//         }
//         catch (MilvusException ex)
//         {
//             Console.WriteLine($"Failed to load collection '{collectionName}': {ex.Message}");
//         }
//     }
// }


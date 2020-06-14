using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace StorageAccountSolutions
{
    class Program
    {
        static string connection = "DefaultEndpointsProtocol=https;AccountName=az204300sa;AccountKey=EPU9Oxvr4tG58GukQJcgDs+yxeaaPSv7j+v+pBr8DUzTNScV0JC9LzJOryZgBMvAGDBTm45GM9wGaSUu/jPI2Q==;EndpointSuffix=core.windows.net";
        static string containerName = "tasks-container";
        static BlobServiceClient client;
        static string filename = "idman.txt";
        static string filepath = "E:\\idman real.txt";

        static void Main(string[] args)
        {
            client = new BlobServiceClient(connection);
            //CreateContainer().Wait();
            //CreateBlob().Wait();
            //GetBlobs().Wait();

            MetadataFunctions();

            
            Console.WriteLine("\nComplete");
            Console.ReadKey();
        }

        private static void MetadataFunctions()
        {
            GetProperties();
            GetMetadata();
            SetMetadataAsync().Wait();
            GetMetadata();
        }

        static async Task CreateBlob()
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            BlobClient blob = containerClient.GetBlobClient(filename);

            using FileStream uploadFileStream = File.OpenRead(filepath);
            await blob.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            Console.WriteLine("\nBlob Created::" + blob.Name);
        }

        static async Task CreateContainer()
        {
            BlobContainerClient container = await client.CreateBlobContainerAsync(containerName);
            Console.WriteLine("\nContainer Created::" + container.Name);
        }

        static async Task GetBlobs()
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            await foreach(BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
        }

        public static void GetProperties()
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            BlobClient blob = containerClient.GetBlobClient(filename);

            BlobProperties props = blob.GetProperties();
            Console.WriteLine("\nAccess Tier::" + props.AccessTier);
            Console.WriteLine("\nContent length::" + props.ContentLength);

        }

        static void GetMetadata()
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            BlobClient blob = containerClient.GetBlobClient(filename);
            BlobProperties props = blob.GetProperties();

            foreach(var metadata in props.Metadata)
            {
                Console.WriteLine(metadata.Key.ToString()+"::"+ metadata.Value.ToString());
                
            }
        }

        static async Task SetMetadataAsync()
        {
            string p_key = "ApplicationType";
            string p_Value = "IDM";
            BlobContainerClient containerClient = client.GetBlobContainerClient(containerName);
            BlobClient blob = containerClient.GetBlobClient(filename);
            BlobProperties props = blob.GetProperties();

            props.Metadata.Add(p_key, p_Value);
            
            IDictionary<string, string> dictObj = new Dictionary<string, string>();
            dictObj.Add(p_key, p_Value);
            await blob.SetMetadataAsync(dictObj);
            Console.WriteLine("\nMetadata set");
        }
    }
}

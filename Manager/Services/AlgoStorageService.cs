using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Manager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Manager.Services
{
    /// <summary>
    /// Представляет методы для работы с хранилищем S3
    /// </summary>
    public class AlgoStorageService : IDisposable
    {
        private AmazonS3Client client;
        private string bucketName;

        private const string ConfigurationDirectory = "Configuration";

        public AlgoStorageService(RegionEndpoint endPoint, string bucketName)
        {
            client = new AmazonS3Client(endPoint);
            this.bucketName = bucketName;
        }

        /// <summary>
        /// Загружает входные параметры для vm в хранилище
        /// </summary>
        /// <param name="instance">Экземпляр vm</param>
        /// <returns></returns>
        public async Task UploadInputParamsAsync(Ec2Instance instance)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = ConfigurationDirectory + "/" + instance.Ec2Id + "_" + instance.AlgoName,
                ContentBody = instance.Parameters
            };

            //await client.PutObjectAsync(request);
			throw new InvalidOperationException();
        }

        /// <summary>
        /// Возвращает список объектов из папки Output
        /// </summary>
        /// <param name="rootDirectory">Корневая директория, имеющая название алгоритма</param>
        /// <returns></returns>
        public async Task<IEnumerable<S3Object>> GetOutputObjectsListAsync(string rootDirectory)
        {
			throw new InvalidOperationException();
			//var response = await client.ListObjectsAsync(bucketName, rootDirectory + "/Output/");
            //return response.S3Objects;
        }
        
        /// <summary>
        /// Возвращает содержимое файла
        /// </summary>
        /// <param name="key">Ключ файла из s3 хранилища</param>
        /// <returns></returns>
        public async Task<string> GetOutputObjectBodyAsync(string key)
        {
			throw new InvalidOperationException();
			//using (var response = await client.GetObjectAsync(bucketName, key))
   //         {
   //             using (var reader = new StreamReader(response.ResponseStream))
   //             {
   //                 return reader.ReadToEnd();
   //             }
   //         }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
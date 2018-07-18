using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using Amazon.S3;
using Amazon;
using System.IO;
using Amazon.S3.Model;

using Newtonsoft.Json;
using Launcher.Models;

namespace Launcher
{
    /// <summary>
    /// Представляет методы для работы с s3 хранилищем
    /// </summary>
    public class AlgoStorage : IDisposable
    {
        private const string RootOutputDirectoryName = "Output";

        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string instanceId;

        public AlgoStorage(string bucketName, string instanceId, RegionEndpoint endPoint)
        {
            client = new AmazonS3Client(endPoint);
            this.bucketName = bucketName;
            this.instanceId = instanceId;
        }

        /// <summary>
        /// Загружает файлы алгоритма в указанный каталог
        /// </summary>
        /// <param name="savePath">Каталог для сохранения</param>
        /// <param name="algoName">Название алгоритма</param>
        public void Download(string savePath, string algoName)
        {
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var objects = client.ListObjects(bucketName, algoName + "/").S3Objects;

            foreach (var item in objects)
            {
                this.DownloadS3Object(item, savePath);
            }
        }

        /// <summary>
        /// Возвращает входные параметры алгоритма. В случае их отсутствия 
        /// повторяет попытку через заданный промежуток времени
        /// </summary>
        /// <param name="checkInterval">Интервал проверки в миллисекундах</param>
        /// <returns></returns>
        public GetInputParamsResult GetInputParams(int checkInterval)
        {
            var objects = client.ListObjects(bucketName, "Configuration").S3Objects;
            var parametersObject = objects.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p.Key).Contains(instanceId));

            if (parametersObject == null)
            {
                Thread.Sleep(checkInterval);
                return GetInputParams(checkInterval);
            }

            var algoName = Regex.Match(Path.GetFileNameWithoutExtension(parametersObject.Key), "(?<=_).+$").Value;

            using (var response = client.GetObject(bucketName, parametersObject.Key))
            {
                using (var streamReader = new StreamReader(response.ResponseStream))
                {
                    return new GetInputParamsResult
                    {
                        Parameters = streamReader.ReadToEnd(),
                        AlgoName = algoName,
                        FileName = parametersObject.Key
                    };
                }
            }
        }

        /// <summary>
        /// Удаляет файл с входными параметрами
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void DeleteInputParams(string fileName)
        {
            client.DeleteObject(bucketName, fileName);
        }

        /// <summary>
        /// Загружает результат работы в формате json в s3 папку Output
        /// </summary>
        /// <param name="obj">Загружаемый объект</param>
        /// <param name="algoName">Название алгоритма</param>
        public void UploadSuccessResult(object obj, string algoName)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = algoName + "/" + RootOutputDirectoryName + "/" + instanceId + "_" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss"),
                ContentBody = JsonConvert.SerializeObject(obj)
            };

            client.PutObject(request);
        }

        /// <summary>
        /// Загружает ошибку в формате json в s3 папку Output
        /// </summary>
        /// <param name="e">Исключение</param>
        /// <param name="algoName">Название алгоритма</param>
        public void UploadErrorResult(Exception e, string algoName)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = algoName + "/" + RootOutputDirectoryName + "/" + instanceId + "_error_" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss"),
                ContentBody = JsonConvert.SerializeObject(e)
            };

            client.PutObject(request);
        }

        /// <summary>
        /// Скачивает файл из s3 хранилища
        /// </summary>
        /// <param name="obj">Каталог для сохранения</param>
        /// <param name="savePath">Путь сохранения</param>
        public void DownloadS3Object(S3Object obj, string savePath)
        {
            using (var response = client.GetObject(bucketName, obj.Key))
            {
                var fileName = Path.GetFileName(obj.Key);
                
                if (!string.IsNullOrEmpty(fileName))
                    response.WriteResponseStreamToFile(Path.Combine(savePath, fileName));
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
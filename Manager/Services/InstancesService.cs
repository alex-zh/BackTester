using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Services
{
    /// <summary>
    /// Представляет методы для работы с экземплярами EC2
    /// </summary>
    public class InstancesService : IDisposable
    {
        private AmazonEC2Client client;

        public InstancesService(RegionEndpoint endPoint)
        {
            client = new AmazonEC2Client(endPoint);
        }

        /// <summary>
        /// Создает экземпляр vm
        /// </summary>
        /// <param name="instance">Экземпляр vm</param>
        /// <returns></returns>
        public async Task<string> CreateInstanceAsync(Ec2Instance instance)
        {
            var request = new RunInstancesRequest
            {
                ImageId = instance.Image.ImageId,
                InstanceType = instance.Type,
                MinCount = 1,
                MaxCount = 1
            };

			throw new InvalidOperationException();
			//var response = await client.RunInstancesAsync(request);
   //         var createdInstance = response.Reservation.Instances.FirstOrDefault();

   //         return createdInstance != null ? createdInstance.InstanceId 
   //             : null;
        }

        /// <summary>
        /// Завершает работу экземпляра vm
        /// </summary>
        /// <param name="instance">Экземпляр vm</param>
        /// <returns></returns>
        public async Task TerminateInstanceAsync(Ec2Instance instance)
        {
            var request = new TerminateInstancesRequest
            {
                InstanceIds = new List<string> { instance.Ec2Id }
            };

			throw new InvalidOperationException();
			//await client.TerminateInstancesAsync(request);
        }

        /// <summary>
        /// Возвращает список экземпляров, пренадлежащие текущему аккаунту
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Instance>> GetInstancesAsync()
        {
			throw new InvalidOperationException();
			//var response = await client.DescribeInstancesAsync();

            //return response.Reservations.SelectMany(p => p.Instances);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
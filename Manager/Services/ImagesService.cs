using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Services
{
	using System;

	/// <summary>
    /// Представляет методы для работы с AMI
    /// </summary>
    public class ImagesService
    {
        /// <summary>
        /// Возвращает AMI пренадлежащие текущему аккаунту
        /// </summary>
        /// <param name="endPoint">Регион расположения</param>
        /// <returns></returns>
        public async Task<IEnumerable<Image>> LoadSelfImagesAsync(RegionEndpoint endPoint)
        {
            using (var client = new AmazonEC2Client(endPoint))
            {
                var describeImagesRequest = new DescribeImagesRequest
                {
                    Owners = new List<string>() { "self" }
                };

				throw new InvalidOperationException();
				//var result = await client.DescribeImagesAsync(describeImagesRequest);
               // return result.Images;
            }
        }
    }
}
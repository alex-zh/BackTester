using Manager.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.Pricing;
using Amazon.Pricing.Model;

namespace Manager.Services
{
    /// <summary>
    /// Представляет методы для работы с API цен
    /// </summary>
    public class PricingService
    {
        private AmazonPricingClient client;

        public PricingService(RegionEndpoint endPoint)
        {            
            client = new AmazonPricingClient(endPoint);
        }

        private readonly string serviceUrl = "http://info.awsstream.com/instances.json?top=100&region={0}&os=mswin";

        public async Task<IEnumerable<Ec2InstancePrice>> ReceiveEc2PricesAsync(string region)
        {
            try
            {


                var products = await client.GetProductsAsync(new GetProductsRequest()
                {
                    MaxResults = 1,
                    ServiceCode = "AmazonEC2",
                    Filters = new List<Filter>()
                    {                        
                        new Filter {Field = "operatingSystem", Type = "TERM_MATCH", Value = "Windows"}
                    }
                });

            }
            catch (Exception exception)
            {
                
            }

            return new[] { new Ec2InstancePrice() };
        }

        /// <summary>
        /// Возвращает первые 100 цен для заданного региона 
        /// </summary>
        /// <param name="region">Регион</param>
        /// <returns></returns>
        /// 
        public async Task<IEnumerable<Ec2InstancePrice>> ReceiveEc2PricesAsyncx(string region)
        {
            using (var client = new WebClient())
            {
                var json = await client.DownloadStringTaskAsync(string.Format(serviceUrl, region));
                dynamic jObject = JsonConvert.DeserializeObject(json);

                var result = new List<Ec2InstancePrice>();

                foreach (var price in jObject)
                {
                    result.Add(new Ec2InstancePrice
                    {
                        Model = price.model.ToString(),
                        Hourly = Convert.ToDouble(price.hourly.ToString())
                    });
                }

                return result;
            }
        }
    }
}
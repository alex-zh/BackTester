using Manager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Services
{
    /// <summary>
    /// Представляет методы для слежения за экземплярами vm
    /// </summary>
    public class InstancesWatcher
    {
        public AlgoStorageService AlgoStorageService { get; set; }

        /// <summary>
        /// Возникает после окончания работы экземпляра
        /// </summary>
        public event Action<Ec2Instance, string> TaskCompleted;
        /// <summary>
        /// Возникает после перерасхода доступного баланса средств
        /// </summary>
        public event Action<Ec2Instance> ExceedMoneyQuota;

        private bool stopped;
        private int instanceCheckInterval;
        private List<string> existingOutputObjects = new List<string>();

        public InstancesWatcher(AlgoStorageService algoStorageService)
        {
            AlgoStorageService = algoStorageService;
            instanceCheckInterval = Convert.ToInt32(ConfigurationManager.AppSettings["InstanceCheckInterval"]);
        }

        /// <summary>
        /// Начинает слежение за экземплярами vm
        /// Раз в InstanceCheckInterval (задается в App.config) проверяет поступление
        /// результата работы vm в s3 хранилище. В случае поступления генерирует событие TaskCompleted.
        /// Следит за потраченной суммой. В случае превышения лимита генерирует событие ExceedMoneyQuota
        /// </summary>
        /// <param name="instances">Экземпляры vm</param>
        public async void StartWatchAsync(IEnumerable<Ec2Instance> instances)
        {
            await Task.Factory.StartNew(async () =>
            {
                var algoName = instances.First().AlgoName;
                var outputObjects = await AlgoStorageService.GetOutputObjectsListAsync(algoName);
                existingOutputObjects = outputObjects.Select(p => p.Key).ToList();

                while (!stopped)
                {
                    CheckMoneyQuota(instances);
                    await CheckCompletedTasksAsync(instances, algoName);

                    await Task.Delay(instanceCheckInterval);
                }
            });
        }

        /// <summary>
        /// Проверяет поступление результата работы vm в s3 хранилище.
        /// В случае поступления генерирует событие TaskCompleted
        /// </summary>
        /// <param name="instances">Экземпляры vm</param>
        /// <param name="algoName">Название алгоритма</param>
        /// <returns></returns>
        private async Task CheckCompletedTasksAsync(IEnumerable<Ec2Instance> instances, string algoName)
        {
            var outputObjects = await AlgoStorageService.GetOutputObjectsListAsync(algoName);

            foreach (var instance in instances)
            {
                var instanceResult = outputObjects.Where(p => Path.GetFileNameWithoutExtension(p.Key).Contains(instance.Ec2Id))
                    .OrderBy(p => p.LastModified)
                    .LastOrDefault();

                if (instanceResult == null)
                    continue;

                if (!existingOutputObjects.Any(p => p == instanceResult.Key))
                {
                    existingOutputObjects.Add(instanceResult.Key);
                    
                    var outputBody = await AlgoStorageService.GetOutputObjectBodyAsync(instanceResult.Key);
                    OnTaskCompleted(instance, outputBody);
                }
            }
        }

        /// <summary>
        /// Проверяет перерасход средств. В случае наступления генерирует событие ExceedMoneyQuota
        /// </summary>
        /// <param name="instances">Экземпляры vm</param>
        private void CheckMoneyQuota(IEnumerable<Ec2Instance> instances)
        {
            foreach (var instance in instances)
            {
                if (instance.LaunchTime == null)
                    continue;

                double secondsElapsed = (DateTime.Now - instance.LaunchTime.GetValueOrDefault()).TotalSeconds;
                double moneySpent = (instance.Price * secondsElapsed) / 3600;

                if (moneySpent >= instance.MaxMoneyAmount)
                    OnExceedMoneyQuota(instance);
            }
        }

        /// <summary>
        /// Останавливает наблюдение
        /// </summary>
        public void StopWatch()
        {
            stopped = true;
        }

        private void OnTaskCompleted(Ec2Instance instance, string text)
        {
            if (TaskCompleted != null)
                TaskCompleted(instance, text);
        }

        private void OnExceedMoneyQuota(Ec2Instance instance)
        {
            if (ExceedMoneyQuota != null)
                ExceedMoneyQuota(instance);
        }
    }
}
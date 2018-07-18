using Manager.Models;
using Manager.Tools;
using System;
using System.Collections.Generic;

namespace Manager.Data
{
    /// <summary>
    /// Хранилище экземпляров vm в памяти
    /// </summary>
    public class InstancesStore
    {
        private static InstancesStore instance;
        public static InstancesStore Instance 
        { 
            get 
            {
                return instance;
            }
        }

        private Dictionary<Guid, Ec2Instance> ec2instances;

        static InstancesStore()
        {
            instance = new InstancesStore();
        }

        private InstancesStore()
        {
            ec2instances = new Dictionary<Guid, Ec2Instance>();
        }

        /// <summary>
        /// Добавляет объект в хранилище
        /// </summary>
        /// <param name="instance">Экземпляр vm</param>
        public void Add(Ec2Instance instance)
        {
            if (ec2instances.ContainsKey(instance.Id))
            {
                ec2instances[instance.Id] = instance;
            }
            else
            {
                ec2instances.Add(instance.Id, instance);
            }
        }

        /// <summary>
        /// Удаляет объект из хранилища
        /// </summary>
        /// <param name="instance">Экземпляр vm</param>
        public void Remove(Ec2Instance instance)
        {
            if (ec2instances.ContainsKey(instance.Id))
            {
                ec2instances.Remove(instance.Id);
            }
        }

        /// <summary>
        /// Возвращает полные копии экземпляров из хранилища
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Ec2Instance> Get()
        {
            var result = new List<Ec2Instance>();

            foreach (var instance in ec2instances)
            {
                result.Add(ObjectCopier.Clone<Ec2Instance>(instance.Value));
            }

            return result;
        }
    }
}
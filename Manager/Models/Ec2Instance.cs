using System;

namespace Manager.Models
{
    /// <summary>
    /// Экземпляр vm
    /// </summary>
    [Serializable]
    public class Ec2Instance
    {
        /// <summary>
        /// Внутренний идентификатор экземпляра
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Идентификатор экземпляра Amazon
        /// </summary>
        public string Ec2Id { get; set; }
        /// <summary>
        /// Тип машины (t2.micro, t2.nano ...)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Цена в час
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Входные параметры для алгоритма
        /// </summary>
        public string Parameters { get; set; }
        /// <summary>
        /// Результат работы алгоритма
        /// </summary>
        public string Output { get; set; }
        /// <summary>
        /// Название алгоритма
        /// </summary>
        public string AlgoName { get; set; }
        /// <summary>
        /// Максимально доступная сумма средст
        /// </summary>
        public double MaxMoneyAmount { get; set; }
        /// <summary>
        /// Состояние
        /// </summary>
        public Ec2InstanceState State { get; set; }
        /// <summary>
        /// Время запуска
        /// </summary>
        public DateTime? LaunchTime { get; set; }
        /// <summary>
        /// Образ ОС, который будет использован для запуска алгоритма
        /// </summary>
        public Ec2InstanceImage Image { get; set; }
    }
}
using System;

namespace Manager.Models
{
    /// <summary>
    /// Образ ОС
    /// </summary>
    [Serializable]
    public class Ec2InstanceImage
    {
        /// <summary>
        /// Идентификатор образа
        /// </summary>
        public string ImageId { get; set; }
        /// <summary>
        /// Имя образа
        /// </summary>
        public string Name { get; set; }
    }
}

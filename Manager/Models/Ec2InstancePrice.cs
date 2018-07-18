
namespace Manager.Models
{
    /// <summary>
    /// Цена экземпляра vm в час
    /// </summary>
    public class Ec2InstancePrice
    {
        /// <summary>
        /// Модель компьютера
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Цена в час
        /// </summary>
        public double Hourly { get; set; }
    }
}
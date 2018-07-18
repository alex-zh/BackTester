namespace Launcher
{
	using System.Net;

	/// <summary>
    /// Представляет метод для получения текущего id экземпляра
    /// </summary>
    public class IdService
    {
        private const string _serviceUrl = "http://169.254.169.254/latest/meta-data/instance-id";

        /// <summary>
        /// Возвращает id текущего экземпляра ec2
        /// </summary>
        /// <returns></returns>
        public string GetCurrentInstanceId()
        {
            using (var client = new WebClient())
                return client.DownloadString(_serviceUrl);
        }
    }
}
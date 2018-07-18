namespace Launcher
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.ServiceProcess;

	using Amazon;

	public partial class LauncherService : ServiceBase
	{
		public LauncherService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			string instanceId = new IdService().GetCurrentInstanceId();
			string bucketName = ConfigurationManager.AppSettings["BucketName"];
			int checkInputParamsInterval = Convert.ToInt32(ConfigurationManager.AppSettings["CheckInputParamsInterval"]);
			var regionEndpoint = RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["RegionEndpoint"]);

			using (var storage = new AlgoStorage(bucketName, instanceId, regionEndpoint))
			{
				string algoName = null;

				try
				{
					var result = storage.GetInputParams(checkInputParamsInterval);
					storage.DeleteInputParams(result.FileName);
					algoName = result.AlgoName;
					var parameters = AlgoParams.Parse(result.Parameters);

					var launcherRootDirectory = ConfigurationManager.AppSettings["LauncherRootDirectory"];
					var algoRootDirectory = Path.Combine(launcherRootDirectory, algoName);

					storage.Download(algoRootDirectory, algoName);

					var activator = new AlgoActivator();
					var algoResult = activator.Activate(algoRootDirectory, parameters);
					storage.UploadSuccessResult(algoResult, algoName);
				}
				catch (Exception e)
				{
					storage.UploadErrorResult(e, algoName);
				}
			}
		}
	}
}
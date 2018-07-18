using System.ServiceProcess;

namespace Launcher
{
	static class Program
	{
		static void Main()
		{
			ServiceBase.Run(new ServiceBase[]
			{
				new LauncherService()
			});
		}
	}
}
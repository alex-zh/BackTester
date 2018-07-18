using Amazon;
using Manager.Data;
using Manager.Services;
using Manager.ViewModels;
using System.Configuration;
using System.Windows.Controls;

namespace Manager.Views
{
    public partial class Ec2InstancesView : Page
    {
        public Ec2InstancesView()
        {
            InitializeComponent();
            
            string regionName = ConfigurationManager.AppSettings["RegionEndpoint"];
            string algoStorageName = ConfigurationManager.AppSettings["AlgoStorage"];
            var algoStorageService = new AlgoStorageService(RegionEndpoint.GetBySystemName(regionName), algoStorageName);

            DataContext = new Ec2InstancesViewModel(
                InstancesStore.Instance,
                new InstancesService(RegionEndpoint.GetBySystemName(regionName)),
                algoStorageService,
                new InstancesWatcher(algoStorageService));
        }
    }
}
using System.Configuration;
using Manager.Data;
using Manager.Services;
using Manager.ViewModels;
using System.Windows.Controls;
using Amazon;

namespace Manager.Views
{
    public partial class CreateEc2InstancesView : Page
    {
        private CreateEc2InstancesViewModel viewModel;

        public CreateEc2InstancesView()
        {
            string regionName = "us-east-1"; //for pricing only us-east-1 or ap-south-1

            InitializeComponent();
            viewModel = new CreateEc2InstancesViewModel(new PricingService(RegionEndpoint.GetBySystemName(regionName)), new ImagesService(), InstancesStore.Instance);
            DataContext = viewModel;
        }
    }
}   
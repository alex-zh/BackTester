using Apex.MVVM;
using Manager.Data;
using Manager.Models;
using Manager.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Manager.ViewModels
{
    public class Ec2InstancesViewModel : ViewModelBase
    {
        public ObservableCollection<Ec2InstanceViewModel> Instances { get; set; }

        public Command NavigateToCreateInstancesCommand { get; set; }
        public Command StartInstancesCommand { get; set; }

        public InstancesStore InstancesStore { get; set; }
        public InstancesService InstancesService { get; set; }
        public AlgoStorageService AlgoStorageService { get; set; }
        public InstancesWatcher InstancesWatcher { get; set; }

        public Ec2InstancesViewModel(InstancesStore instancesStore, InstancesService instancesService, 
            AlgoStorageService algoStorageService, InstancesWatcher instancesWatcher)
        {
            Instances = new ObservableCollection<Ec2InstanceViewModel>();

            NavigateToCreateInstancesCommand = new Command(NavigateToCreateInstances);
            StartInstancesCommand = new Command(StartInstances);

            InstancesStore = instancesStore;
            InstancesService = instancesService;
            AlgoStorageService = algoStorageService;
            InstancesWatcher = instancesWatcher;
        }

        /// <summary>
        /// Переходит на страницу создания экземпляров
        /// </summary>
        private void NavigateToCreateInstances()
        {
            var navigationWindow = Application.Current.MainWindow as NavigationWindow;
            navigationWindow.Navigate(new Uri("Views/CreateEc2InstancesView.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Запускает ранее созданные экземпляры vm
        /// </summary>
        private async void StartInstances()
        {
            LoadInstances();

            if (Instances.Count == 0)
                return;

            await CreateInstancesAsync();
            await UploadInputParamsAsync();

            InstancesWatcher.TaskCompleted += async (instance, output) =>
            {
                var viewModel = Instances.FirstOrDefault(p => p.Ec2Instance == instance);
                viewModel.Output = output;

                await TerminateInsatanceAsync(viewModel);
            };

            InstancesWatcher.ExceedMoneyQuota += async (instance) =>
            {
                var viewModel = Instances.FirstOrDefault(p => p.Ec2Instance == instance);

                await TerminateInsatanceAsync(viewModel);
            };

            InstancesWatcher.StartWatchAsync(Instances.Select(p => p.Ec2Instance));
        }

        /// <summary>
        /// Загружает входные параметры для vm в s3 хранилище
        /// </summary>
        /// <returns></returns>
        private async Task UploadInputParamsAsync()
        {
            foreach (var viewModel in Instances)
            {
                if (viewModel.Ec2Instance.LaunchTime != null)
                    continue;

                await AlgoStorageService.UploadInputParamsAsync(viewModel.Ec2Instance);
            }
        }

        /// <summary>
        /// Создает экземпляры vm
        /// </summary>
        /// <returns></returns>
        private async Task CreateInstancesAsync()
        {
            foreach (var viewModel in Instances)
            {
                if (viewModel.Ec2Instance.LaunchTime != null)
                    continue;

                var instanceId = await InstancesService.CreateInstanceAsync(viewModel.Ec2Instance);
                viewModel.Ec2Id = instanceId;
                viewModel.State = Ec2InstanceState.Work;
                viewModel.Ec2Instance.LaunchTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Завершает работу экземпляра vm
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task TerminateInsatanceAsync(Ec2InstanceViewModel viewModel)
        {
            if (viewModel.State == Ec2InstanceState.Terminated)
                return;

            await InstancesService.TerminateInstanceAsync(viewModel.Ec2Instance);
            viewModel.State = Ec2InstanceState.Terminated;
        }

        /// <summary>
        /// Загружает экземпляры vm из хранилища
        /// </summary>
        private void LoadInstances()
        {
            var instances = InstancesStore.Get();

            foreach (var instance in instances)
            {
                Instances.Add(new Ec2InstanceViewModel(instance));
            }
        }
    }
}
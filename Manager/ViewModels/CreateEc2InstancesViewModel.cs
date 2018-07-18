using Amazon;
using Apex.MVVM;
using Manager.Data;
using Manager.Models;
using Manager.Services;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace Manager.ViewModels
{
    public class CreateEc2InstancesViewModel : ViewModelBase
    {
        public ObservableCollection<Ec2InstancePriceViewModel> Prices { get; set; }
        public ObservableCollection<CreateEc2InstanceViewModel> Instances { get; set; }
        public ObservableCollection<Ec2InstanceImageViewModel> Images { get; set; }

        public Command AppendInstanceCommand { get; set; }
        public Command CreateInstancesCommand { get; set; }
        public Command LoadPricesCommand { get; set; }

        public PricingService PricingService { get; set; }
        public ImagesService ImagesService { get; set; }
        public InstancesStore InstancesStore { get; set; }

        private readonly string regionName = ConfigurationManager.AppSettings["RegionEndpoint"];

        private bool inProgress;
        public bool InProgress
        {
            get { return inProgress; }
            set 
            {
                inProgress = value;
                OnPropertyChanged();
            }
        }

        private string algoName;
        public string AlgoName
        {
            get { return algoName; }
            set
            {
                algoName = value;
                OnPropertyChanged();
            }
        }

        public CreateEc2InstancesViewModel(PricingService pricingService, ImagesService imagesService, InstancesStore instancesStore)
        {
            Prices = new ObservableCollection<Ec2InstancePriceViewModel>();
            Instances = new ObservableCollection<CreateEc2InstanceViewModel>();
            Instances.Add(new CreateEc2InstanceViewModel());

            Images = new ObservableCollection<Ec2InstanceImageViewModel>();

            AppendInstanceCommand = new Command(AppendInstance);
            CreateInstancesCommand = new Command(CreateInstances);
            LoadPricesCommand = new Command(LoadPrices);

            PricingService = pricingService;
            ImagesService = imagesService;
            InstancesStore = instancesStore;
        }

        private void AppendInstance()
        {
            Instances.Add(new CreateEc2InstanceViewModel());
        }

        private void CreateInstances()
        {
            if (!ValidateForm())
                return;

            var result = MessageBox.Show("Создать количество экземпляров: " + Instances.Count
                 + Environment.NewLine
                 + "Общая стоимость в час: " + Math.Round(Instances.Sum(p => p.Price.Hourly), 3) + " $"
                , "", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            foreach (var instance in Instances)
            {
                InstancesStore.Add(new Ec2Instance
                {
                    Id = Guid.NewGuid(),
                    Parameters = instance.Parameters,
                    AlgoName = AlgoName,
                    Type = instance.Price.Model,
                    Price = instance.Price.Hourly,
                    Image = instance.Image.Image,
                    MaxMoneyAmount = instance.MaxAmount
                });
            }

            NavigateToInstances();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(AlgoName))
            {
                MessageBox.Show("Необходимо указать название алгоритма");
                return false;
            }

            if (Instances.Any(p => p.MaxAmount == 0))
            {
                MessageBox.Show("Максимальная сумма должна быть больше 0");
                return false;
            }

            return true;
        }

        private void NavigateToInstances()
        {
            var navigationWindow = Application.Current.MainWindow as NavigationWindow;
            navigationWindow.Navigate(new Uri("Views/Ec2InstancesView.xaml", UriKind.Relative));
        }

        private async void LoadPrices()
        {
            try
            {
                ToggleProgress();

                var prices = await PricingService.ReceiveEc2PricesAsync(regionName);
                var images = await ImagesService.LoadSelfImagesAsync(RegionEndpoint.GetBySystemName(regionName));

                foreach (var price in prices)
                {
                    Prices.Add(new Ec2InstancePriceViewModel(price));
                }

                foreach (var image in images)
                {
                    Images.Add(new Ec2InstanceImageViewModel(new Ec2InstanceImage
                    {
                        ImageId = image.ImageId,
                        Name = image.Name
                    }));
                }
            }
            catch
            {
                MessageBox.Show("Не удалось загрузить цены. Попробуйте позже");
                NavigateToInstances();
            }
            finally
            {
                ToggleProgress();
            }
        }

        private void ToggleProgress()
        {
            InProgress = !InProgress;
        }
    }
}
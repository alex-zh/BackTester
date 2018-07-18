using Manager.Models;

namespace Manager.ViewModels
{
    public class Ec2InstancePriceViewModel : ViewModelBase
    {
        public Ec2InstancePrice Ec2InstancePrice { get; set; }

        public Ec2InstancePriceViewModel(Ec2InstancePrice ec2InstancePrice)
        {
            Ec2InstancePrice = ec2InstancePrice;
        }

        public string Model
        {
            get { return Ec2InstancePrice.Model; }
            set
            {
                Ec2InstancePrice.Model = value;
                OnPropertyChanged();
            }
        }

        public double Hourly
        {
            get { return Ec2InstancePrice.Hourly; }
            set
            {
                Ec2InstancePrice.Hourly = value;
                OnPropertyChanged();
            }
        }
    }
}
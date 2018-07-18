
namespace Manager.ViewModels
{
    public class CreateEc2InstanceViewModel : ViewModelBase
    {
        private string instanceType;
        public string InstanceType 
        {
            get { return instanceType; }
            set
            {
                instanceType = value;
                OnPropertyChanged();
            }
        }

        private string parameters;
        public string Parameters 
        {
            get { return parameters; }
            set
            {
                parameters = value;
                OnPropertyChanged();
            }
        }

        private double maxAmount;
        public double MaxAmount 
        {
            get { return maxAmount; }
            set
            {
                maxAmount = value;
                OnPropertyChanged();
            }
        }

        private Ec2InstanceImageViewModel image;
        public Ec2InstanceImageViewModel Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }

        private Ec2InstancePriceViewModel price;

        public Ec2InstancePriceViewModel Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }
    }
}
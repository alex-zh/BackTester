using Manager.Models;

namespace Manager.ViewModels
{
    public class Ec2InstanceImageViewModel : ViewModelBase
    {
        public Ec2InstanceImage Image { get; set; }

        public Ec2InstanceImageViewModel(Ec2InstanceImage image)
        {
            Image = image;
        }

        public string ImageId
        {
            get { return Image.ImageId; }
            set 
            {
                Image.ImageId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return Image.Name; }
            set 
            {
                Image.Name = value;
                OnPropertyChanged();
            }
        }
    }
}

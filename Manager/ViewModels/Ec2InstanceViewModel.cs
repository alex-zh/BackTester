using Manager.Models;

namespace Manager.ViewModels
{
    public class Ec2InstanceViewModel : ViewModelBase
    {
        public Ec2Instance Ec2Instance { get; set; }

        public Ec2InstanceViewModel(Ec2Instance instance)
        {
            Ec2Instance = instance;
        }

        public string Ec2Id
        {
            get { return Ec2Instance.Ec2Id; }
            set
            {
                Ec2Instance.Ec2Id = value;
                OnPropertyChanged();
            }
        }

        public Ec2InstanceState State
        {
            get { return Ec2Instance.State; }
            set
            {
                Ec2Instance.State = value;
                OnPropertyChanged();
                OnPropertyChanged("StateString");
            }
        }

        public string StateString
        {
            get
            {
                switch(Ec2Instance.State)
                {
                    case Ec2InstanceState.Wait: return "Ожидает запуска";
                    case Ec2InstanceState.Work: return "Работает";
                    case Ec2InstanceState.Terminated: return "Завершен";
                    default: return null;
                };
            }
        }

        public string Output
        {
            get { return Ec2Instance.Output; }
            set 
            {
                Ec2Instance.Output = value;
                OnPropertyChanged();
            }
        }
    }
}
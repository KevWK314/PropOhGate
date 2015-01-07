using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace HousingData.Client.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                var vm = ServiceLocator.Current.GetInstance<MainViewModel>();
                return vm;
            }
        }
        
        public static void Cleanup()
        {
            ServiceLocator.Current
                .GetAllInstances<MainViewModel>()
                .ToList()
                .ForEach(v => v.Cleanup());
        }
    }
}
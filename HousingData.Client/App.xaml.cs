using System.Windows;
using HousingData.Client.ViewModel;

namespace HousingData.Client
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ViewModelLocator.Cleanup();
        }
    }
}

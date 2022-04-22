using Rg.Plugins.Popup.Services;
using SSDIWMS_android.Services.MainServices;
using SSDIWMS_android.Services.NotificationServices;
using SSDIWMS_android.Services.ServerDbServices.Users;
using SSDIWMS_android.ViewModels;
using SSDIWMS_android.ViewModels.PopUpVMs;
using SSDIWMS_android.Views.PopUpPages;
using SSDIWMS_android.Views.StockMovementPages;
using SSDIWMS_android.Views.StockMovementPages.IncomingPages;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSDIWMS_android
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        IMainServices mainService;
        IToastNotifService notifService;
        IServerUserServices serverDbUserService;

        string _fullname, _role;
        bool _stockMovementVisible;
        public bool StockMovementVisible
        {
            get => _stockMovementVisible;
            set
            {

                if (value == _stockMovementVisible)
                    return;
                _stockMovementVisible = value;
                OnPropertyChanged();
            }
        }
        public string Fullname
        {
            get => _fullname;
            set
            {

                if (value == _fullname)
                    return;
                _fullname = value;
                OnPropertyChanged();
            }
        }
        public string Role
        {
            get => _role;
            set
            {

                if (value == _role)
                    return;
                _role = value;
                OnPropertyChanged();
            }
        }
        public AppShell()
        {
            InitializeComponent();
            
            BindingContext = this;
            serverDbUserService = DependencyService.Get<IServerUserServices>();
            notifService = DependencyService.Get<IToastNotifService>();
            mainService = DependencyService.Get<IMainServices>();

            Routing.RegisterRoute(nameof(IncomingHeaderPage), typeof(IncomingHeaderPage));
            Routing.RegisterRoute(nameof(IncomingDetailListPage), typeof(IncomingDetailListPage));
            Fullname = Preferences.Get("PrefUserFullname", string.Empty);
            Role = Preferences.Get("PrefUserRole", string.Empty);
            SetView(Role);

            MessagingCenter.Subscribe<LoginVM, string>(this, "FromLoginToShell", async (page, e) =>
            {
                await Task.Delay(1);
                Fullname = Preferences.Get("PrefUserFullname", string.Empty);
                Role = Preferences.Get("PrefUserRole", string.Empty);
                SetView(Role);
            });
        }
        private void SetView(string role)
        {
            if(role == "Pick" || role == "Check")
            {
                StockMovementVisible = true;
            }
            else
            {
                StockMovementVisible = false;
            }
        }
        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            var con = new LoadingPopupVM();
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage("Logout"));
            try
            {
                var userId = Preferences.Get("PrefUserId", 0);
                int[] array = { userId };
                var user = await serverDbUserService.ReturnModel("Id", null, array);
                if(user != null)
                {
                    await serverDbUserService.Update("Logout", null, array, user);
                    await mainService.RemovePreferences();
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                
            }
            catch
            {
                await notifService.StaticToastNotif("Error", "Cannot connect to server.");
            }
            await con.CloseAll();
          
        }

    }
}

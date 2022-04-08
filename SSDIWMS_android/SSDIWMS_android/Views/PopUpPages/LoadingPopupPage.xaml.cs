using SSDIWMS_android.ViewModels.PopUpVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSDIWMS_android.Views.PopUpPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public LoadingPopupPage(string type)
        {
            
            InitializeComponent();
            var con = BindingContext as LoadingPopupVM;
            con.TaskType = type;
        }
    }
}
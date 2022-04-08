
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
    public partial class FormPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public FormPopupPage(string type)
        {
            InitializeComponent();
            var con = BindingContext as FormPopupVM;
            con.TaskType = type;

        }
    }
}
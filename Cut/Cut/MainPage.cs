using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Cut
{
    public class MainPage : MasterDetailPage
    {
        MasterPage masterPage;
        public MainPage()
        {
            Navigation.PushModalAsync(new MainLoadingPage(this));
            masterPage = new MasterPage();
            Master = masterPage;
            Detail = new NavigationPage(new HomePage());

            masterPage.ListView.ItemSelected += OnItemSelected;
            if (Device.OS == TargetPlatform.Windows)
            {
                Master.Icon = "swap.png";
            }
        }
        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
        bool navi_zero = false;
        protected override bool OnBackButtonPressed()
        {
            if (Detail.Navigation.NavigationStack.Count > 1) {
                navi_zero = false;
                return base.OnBackButtonPressed();
            }
            else {
                if (!navi_zero)
                    navi_zero = true;
                else
                    return base.OnBackButtonPressed();
            }
            return true;
        }
    }
}

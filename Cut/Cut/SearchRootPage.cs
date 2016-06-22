using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Cut
{
    public class SearchRootPage : ContentPage
    {
        SearchBar input_voc;
        ListView VocList;
        ObservableCollection<string> VocList_Items;
        public SearchRootPage()
        {
            Title = "部首查詢";
            input_voc = new SearchBar();
            VocList = new ListView();
            VocList_Items = new ObservableCollection<string>();
            VocList.ItemsSource = VocList_Items;
            VocList.ItemSelected += Lis_ItemSelected;
            input_voc.TextChanged += (sender, args)=>{
                VocList_Items.Clear();
                foreach (var voc in Voc.match_rot("^" + input_voc.Text + "*"))
                    VocList_Items.Add(voc);
            };
            /*input_voc.SearchButtonPressed += async (sender, args) => {
                if (input_voc.Text == "") return;
                await Navigation.PushAsync(new SingleVocPage(input_voc.Text));
            };*/
            VocList_Items.Clear();
            foreach (var voc in Voc.match_rot("^" + input_voc.Text + "*"))
                VocList_Items.Add(voc);
            Content = new StackLayout
            {
                Children = {
                    input_voc,
                    VocList
                }
            };
        }

        private async void Lis_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as string;
            if (item != null)
            {
                await Navigation.PushAsync(new SingleRootPage(item));
                ((Content as StackLayout).Children.ElementAt(1) as ListView).SelectedItem = null;
            }
        }
    }
}

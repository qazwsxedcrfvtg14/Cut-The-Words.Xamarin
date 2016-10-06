using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Cut
{
    public class FavoritePage : ContentPage
    {
        
        ListView VocList;
        ObservableCollection<wod> VocList_Items;
        public FavoritePage()
        {
            Title = "我的最愛";
            VocList = new ListView();
            VocList_Items = new ObservableCollection<wod>();
            VocList.ItemsSource = VocList_Items;
            var customCell = new DataTemplate(typeof(stcell));
            customCell.SetBinding(stcell.vocProperty, "voc");
            customCell.SetBinding(stcell.expProperty, "exp");
            VocList.ItemTemplate = customCell;
            VocList.ItemSelected += VocList_ItemSelected; ;
            VocList.HasUnevenRows = true;
            VocList_Items.Clear();
            foreach (var voc in Voc.favorite.data)
                if(Voc.words.exists(voc.Key))
                    VocList_Items.Add(new wod(voc.Key, Voc.GetExpSimple(Voc.words.val(voc.Key))));
            Content = VocList;
        }

        private async void VocList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (e.SelectedItem as wod);
            if (item != null)
            {
                await Navigation.PushAsync(new SingleVocPage(item.voc));
                VocList.SelectedItem = null;
            }
        }
    }
}
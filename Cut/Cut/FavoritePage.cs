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
        public class stcell : ViewCell
        {
            Label _voc, _exp;
            public static readonly BindableProperty vocProperty = BindableProperty.Create("voc", typeof(string), typeof(stcell), "voc");
            public static readonly BindableProperty expProperty = BindableProperty.Create("exp", typeof(string), typeof(stcell), "exp");
            public string voc
            {
                get { return (string)GetValue(vocProperty); }
                set { SetValue(vocProperty, value); }
            }

            public string exp
            {
                get { return (string)GetValue(expProperty); }
                set { SetValue(expProperty, value); }
            }

            public stcell()
            {
                _voc = new Label { };
                _exp = new Label { };
                View = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { _voc, _exp }
                };
            }

            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();

                if (BindingContext != null)
                {
                    _voc.Text = voc;
                    _exp.Text = exp;
                }
            }
        }
        public class wod
        {
            public string voc { get; private set; }
            public string exp { get; private set; }
            public wod(string _voc, string _exp)
            {
                voc = _voc;
                exp = _exp;
            }
        }
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
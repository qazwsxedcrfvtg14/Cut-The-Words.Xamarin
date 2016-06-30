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
    public class SearchVocPage : ContentPage
    {
        public class stcell:ViewCell
        {
            Label _voc,_exp;
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
                _voc = new Label {};
                _exp = new Label {};
                View = new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                    Children = { _voc, _exp}
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
        SearchBar input_voc;
        ListView VocList;
        ObservableCollection<wod> VocList_Items;
        bool islading = false;
        string target = "";
        public SearchVocPage()
        {
            Title = "單字查詢";
            input_voc = new SearchBar();
            VocList = new ListView();
            VocList_Items = new ObservableCollection<wod>();
            VocList.ItemsSource = VocList_Items;
            var customCell = new DataTemplate(typeof(stcell));
            customCell.SetBinding(stcell.vocProperty, "voc");
            customCell.SetBinding(stcell.expProperty, "exp");
            VocList.ItemTemplate = customCell;
            VocList.ItemSelected += Lis_ItemSelected;
            VocList.HasUnevenRows = true;

            Device.StartTimer(new TimeSpan(1000000), () => {
                if (this == null)
                    return false;
                var q = input_voc.Text;
                if (q == null) q = "";
                if (target == q)
                    return true;
                islading = true;
                target = q;
                Task.Run(() =>
                {
                    var ve = Voc.match(q + "*");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (target != input_voc.Text||q!=target) return;
                        VocList_Items.Clear();
                        foreach (var voc in ve)
                            VocList_Items.Add(new wod(voc, Voc.GetExpSimple(Voc.words.val(voc))));
                        islading = false;
                    });
                });
                return true;
            });
            input_voc.SearchButtonPressed += async (sender, args) => {
                if (input_voc.Text == "") return;
                await Navigation.PushAsync(new SingleVocPage(input_voc.Text));
            };
            VocList_Items.Clear();
            foreach (var voc in Voc.match(input_voc.Text + "*"))
                VocList_Items.Add(new wod(voc, Voc.GetExpSimple(Voc.words.val(voc))));
            VocList.ItemAppearing += (sender, e) =>{
                if (islading || VocList_Items.Count == 0)
                    return;
                if (e.Item!=null&&(e.Item as wod).voc == VocList_Items[VocList_Items.Count - 1].voc){
                    LoadMoreItems();
                }
            };
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
            var item = (e.SelectedItem as wod);
            if (item != null)
            {
                await Navigation.PushAsync(new SingleVocPage(item.voc));
                VocList.SelectedItem = null;
            }
        }
        private async void LoadMoreItems()
        {
            islading = true;
            int cnt = VocList_Items.Count;
            string st = VocList_Items[cnt - 1].voc;
            var q = input_voc.Text;
            var ve=await Task.Run(() =>
            {
                return Voc.match(input_voc.Text + "*", cnt,st);
            });
            if (q != input_voc.Text) return;
            foreach (var voc in ve)
                VocList_Items.Add(new wod(voc, Voc.GetExpSimple(Voc.words.val(voc))));
            islading = false;
        }
    }
}

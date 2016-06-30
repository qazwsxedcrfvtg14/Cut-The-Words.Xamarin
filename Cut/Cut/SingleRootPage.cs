using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cut
{
    public class SingleRootPage : ContentPage
    {

        internal StackLayout
            voc_root = new StackLayout { Orientation = StackOrientation.Horizontal},
            alias_list = new StackLayout(),
            VocList = new StackLayout();
        Grid grid = new Grid
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    //new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) }
                },
        };
        string _voc, _exp;
        public SingleRootPage(string param)
        {
            //param.ToLower();
            param = param.Trim();
            //NavigationPage.SetHasNavigationBar(this, false);
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "todo.png",
                Text = "修改",
                Command = new Command(async () => {
                    var page = new MyPopupPage(this);
                    await PopupNavigation.PushAsync(page);
                    //await Navigation.PushPopupAsync(new MyPopupPage());
                })
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "reminders.png",
                Text = "刪除",
                Command = new Command(async () =>
                {
                    var answer = await DisplayAlert("刪除", "真的要刪除嗎?", "是", "否");
                    if (answer == true)
                    {
                        Voc.words.remove(Title);
                        await Navigation.PopAsync();
                    }
                })
            });


            Title = param;
            _voc = param;
            _exp = Voc.GetRootExp(param);
            voc_root.Children.Clear();
            foreach (var x in Voc.CutExp(_exp))
            {
                voc_root.Children.Add(new Label
                {
                    Text = x,
                    Margin = 15,
                    //FontSize=20,
                });
            }

            alias_list.Children.Clear();
            if (_voc[0] == '-' && _voc[_voc.Length - 1] == '-')
            {
                foreach (var x in Voc.root.data)
                    if ("-" + x.Key + "-" != _voc && Voc.GetRootExp("-" + x.Key + "-") == _exp)
                    {
                        var tmp = new Label { Text = x.Key, Margin = 5 };
                        var tgr = new TapGestureRecognizer();
                        tgr.Tapped += Tgr_Tapped;
                        tmp.GestureRecognizers.Add(tgr);
                        alias_list.Children.Add(tmp);
                    }
            }
            else if (_voc[0] != '-' && _voc[_voc.Length - 1] == '-')
            {
                foreach (var x in Voc.prefix.data)
                    if (x.Key + "-" != _voc && Voc.GetRootExp(x.Key + "-") == _exp)
                    {
                        var tmp = new Label { Text = x.Key, Margin = 5 };
                        var tgr = new TapGestureRecognizer();
                        tgr.Tapped += Tgr_Tapped;
                        tmp.GestureRecognizers.Add(tgr);
                        alias_list.Children.Add(tmp);
                    }
            }
            else if (_voc[0] == '-' && _voc[_voc.Length - 1] != '-')
            {
                foreach (var x in Voc.suffix.data)
                    if ("-" + x.Key != _voc && Voc.GetRootExp("-" + x.Key) == _exp)
                    {
                        var tmp = new Label { Text = x.Key, Margin = 5 };
                        var tgr = new TapGestureRecognizer();
                        tgr.Tapped += Tgr_Tapped;
                        tmp.GestureRecognizers.Add(tgr);
                        alias_list.Children.Add(tmp);
                    }
            }
            var tsk=Task.Run(() =>
            {
                string data = "";
                string reg_string = "";

                if (_voc[0] == '-' && _voc[_voc.Length - 1] == '-')
                    data = _voc.Substring(1, _voc.Length - 2);
                else if (_voc[0] != '-' && _voc[_voc.Length - 1] == '-')
                    data = _voc.Substring(0, _voc.Length - 1);
                else if (_voc[0] == '-' && _voc[_voc.Length - 1] != '-')
                    data = _voc.Substring(1, _voc.Length - 1);

                reg_string = ".*" + data + ".*";
                if (data.Length == 1)
                {
                    if (_voc[0] == '-')
                        reg_string = ".*" + data;
                    else
                        reg_string = data + ".*";

                }

                Regex reg = new Regex(reg_string, RegexOptions.IgnoreCase);
                int cnt = 0;
                bool brk = false;
                List<string> lis=new List<string>();
                foreach (var x in Voc.words.data)
                {
                    if (brk) break;
                    if (!reg.IsMatch(x.Key) || x.Key == data) continue;
                    var s2 = Voc.Show2(x.Key);
                    foreach (var y in s2)
                    {
                        if (y == _voc)
                        {
                            lis.Add(x.Key);
                            
                            if (++cnt == 30) brk = true;
                            break;
                        }
                    }
                }
                return lis;
            });
            VocList.Children.Add(new Label {Text="讀取中..." });
            Device.StartTimer(new TimeSpan(1000000), () => {
                if (tsk.IsCompleted)
                {
                    var lis = tsk.Result;
                    VocList.Children.Clear();
                    foreach (var x in lis) {
                        var stp = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = {
                                    new Label{Text = x},
                                    new Label {
                                        Text = Voc.GetExpSimple(Voc.words.val(x)).Trim(),
                                        Margin = new Thickness(20, 0, 0, 0)
                                    },
                                }
                        };
                        var tgr2 = new TapGestureRecognizer();
                        tgr2.Tapped += Tgr2_Tapped;
                        stp.GestureRecognizers.Add(tgr2);
                        VocList.Children.Add(stp);
                    }
                    return false;
                }
                return true;
            });
            Content = new ScrollView { Orientation = ScrollOrientation.Vertical, Content = grid, Margin = new Thickness(10, 10, 10, 5) };
            grid.Children.Add(alias_list, 0, 0);
            grid.Children.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = voc_root }, 0, 1);
            grid.Children.Add(VocList, 0, 2);
            
        }

        private async void Tgr2_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SingleVocPage(((sender as StackLayout).Children[0] as Label).Text));
        }

        private async void Tgr_Tapped(object sender, EventArgs e)
        {
            var senderTextBlock = sender as Label;
            if (senderTextBlock == null) return;
            var s = senderTextBlock.Text;
            if (Title[0] == '-')
                s = "-" + s;
            if (Title[Title.Length-1] == '-')
                s = s + "-";
            await Navigation.PushAsync(new SingleRootPage(s));
        }

        public class MyPopupPage : PopupPage
        {
            ContentPage back_page;
            public MyPopupPage(SingleRootPage page)
            {
                back_page = page;
                var input1 = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, };
                input1.Text = Voc.GetRootExpOrg(page.Title);
                var st = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = 30,
                    Children = {
                    new Label { Text = "\n修改解釋" },
                    new StackLayout {
                        Orientation = StackOrientation.Vertical,Children= {
                            input1,
                            new Button {
                                Text ="確認",
                                Command = new Command(async()=> {
                                    string _voc=page._voc;
                                    if (_voc[0] == '-' && _voc[_voc.Length - 1] == '-')
                                        Voc.root.add(_voc.Substring(1, _voc.Length - 2),input1.Text);
                                    else if (_voc[0] != '-' && _voc[_voc.Length - 1] == '-')
                                        Voc.prefix.add(_voc.Substring(0, _voc.Length - 1),input1.Text);
                                    else if (_voc[0] == '-' && _voc[_voc.Length - 1] != '-')
                                        Voc.suffix.add(_voc.Substring(1, _voc.Length - 1),input1.Text);
                                    await PopupNavigation.PopAsync();
                                }),
                            }
                        }
                    },
                }
                };
                Content = st;
            }

            protected override void OnAppearing()
            {
                back_page.Content.IsVisible = false;
                base.OnAppearing();
            }

            protected override void OnDisappearing()
            {
                base.OnDisappearing();
                back_page.Content.IsVisible = true;
            }

            protected override bool OnBackButtonPressed()
            {
                // Prevent hide popup
                //return base.OnBackButtonPressed();
                return true;
            }
        }
    }

}

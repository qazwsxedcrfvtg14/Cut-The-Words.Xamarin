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
    public class SingleVocPage : ContentPage
    {

        internal StackLayout
            voc_root = new StackLayout { Orientation = StackOrientation.Horizontal },
            voc_croot = new StackLayout { Orientation = StackOrientation.Horizontal },
            //expst = new StackLayout(),
            pics = new StackLayout(),
            alias_list = new StackLayout(),
            VocList = new StackLayout();
        internal ContentView
            expst = new ContentView();
        internal ScrollView
            note_view = new ScrollView();
        internal Tuple<string, List<int>> wds;
        internal Label tivoc= new Label();
        Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    //new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) }
                },
            };
        Label kk = new Label();
        string media_uri,_voc;
        internal void Init(string data,bool first=true)
        {
            tivoc.Text = data;
            wds = Voc.GetExp(Voc.words.val(data));
            expst.Content = Voc.ExpStack(wds.Item1);
            if (first)
            {
                this.Appearing += async (sender, e) =>
                {
                    var s = await Voc.GetAsync(Voc.setting.val("sound_url") + data);
                    if (s == null) return;
                    var be = s.IndexOf(Voc.setting.val("sound_url2"));
                    if (be == -1) return;
                    s = s.Substring(be);
                    var ed = s.IndexOf(Voc.setting.val("sound_type"));
                    if (ed == -1) return;
                    s = s.Substring(0, ed + 4);
                    while (true)
                    {
                        var pos = s.Substring(1).IndexOf(Voc.setting.val("sound_url2"));
                        if (pos == -1) break;
                        s = s.Substring(pos + 1);
                    }
                    media_uri = s;
                    ToolbarItems[4].Icon = "volume.png";
                    if (Voc.setting["auto_play"] == "On")
                        DependencyService.Get<IAudio>().PlayMp3File(media_uri);
                };
                this.Appearing += async (sender, e) =>
                {
                    var s = await Voc.GetAsync("https://www.bing.com/images/search?q=" + data);
                    if (s == null) return;
                    pics.Children.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        var be = s.IndexOf(".mm.bing.net/");
                        if (be == -1 || be - 25 < 0) return;
                        s = s.Substring(be - 25);
                        be = s.IndexOf("http");
                        if (be == -1) return;
                        s = s.Substring(be);
                        var ed = s.IndexOf("\"");
                        if (ed == -1) return;
                        pics.Children.Add(new Image
                        {
                            Source = s.Substring(0, ed),
                            Margin = new Thickness(10, 0, 0, 0),
                            HeightRequest = 150,
                        });
                        s = s.Substring(ed - 1);
                    }
                };
                this.Appearing += async (sender, e) =>
                {
                    var web = await Voc.GetAsync("http://tw.dictionary.search.yahoo.com/search?p=" + data);
                    if (web == null) return;
                    int len = web.Length;
                    var beg = web.IndexOf(">KK[");
                    if (beg == -1) return;
                    web = web.Substring(beg);
                    beg = web.IndexOf("[");
                    if (beg == -1) return;
                    web = web.Substring(beg);
                    beg = web.IndexOf("]");
                    if (beg == -1) return;
                    web = web.Substring(0, beg + 1);
                    kk.Text = web;
                };
            }
            //Web Data Begin
            //Web Data End
            var ve = Voc.Show(data);
            //var tp = new List<Picker>();
            //var tmp = new List<Label>();
            voc_root.Children.Clear();
            voc_croot.Children.Clear();
            foreach (var x in ve)
            {
                var tmp = new Label {
                    Text=x.Item2,
                    Margin=new Thickness(10,5,10,5),
                    //FontSize=18,
                };
                var tgr = new TapGestureRecognizer();
                tgr.Tapped += Tgr_Tapped ;
                tmp.GestureRecognizers.Add(tgr);
                voc_root.Children.Add(tmp);
            }
            int id = 0;
            foreach (var x in ve)
            {
                var tmp = new Picker {
                    Margin = new Thickness(10, 5, 10, 5),
                };
                var vw = Voc.CutExp(x.Item1);
                foreach (var y in vw)
                    tmp.Items.Add(y);
                if (wds.Item2.Count == id)
                    wds.Item2.Add(0);
                if (wds.Item2[id] >= tmp.Items.Count)
                    wds.Item2[id] = tmp.Items.Count - 1;
                tmp.SelectedIndex = wds.Item2[id];
                tmp.SelectedIndexChanged += Tmp_SelectedIndexChanged;
                voc_croot.Children.Add(tmp);
                id++;
            }
            if (Voc.favorite.exists(data))
                ToolbarItems[0].Icon = "unfavorite.png";
            else
                ToolbarItems[0].Icon = "favorite.png";
            string _voc = data, _exp = Voc.GetExpSimple(Voc.words.val(_voc));

            var task = Task.Run(() =>
            {
                List<string> lis = new List<string>();
                foreach (var x in Voc.words.data)
                    if (x.Key != _voc && Voc.GetExpSimple(x.Value) == _exp)
                        lis.Add(x.Key);
                return lis;
            });

            var tsk = Task.Run(() =>
            {
                if (data.Length == 1 || (data.Split(' ').Length) != 1)
                    return new List<string>();
                string reg_string = "";
                if (data.Length > 2 && data[data.Length - 1] == 'e')
                    reg_string = ".*" + data.Substring(0, data.Length - 1) + ".*";
                else
                    reg_string = ".*" + data + ".*";
                Regex reg = new Regex(reg_string, RegexOptions.IgnoreCase);
                List<string> lis = new List<string>();
                foreach (var x in Voc.words.data)
                {
                    if (!reg.IsMatch(x.Key) || x.Key == data) continue;
                    var s2 = Voc.Show2(x.Key);
                    foreach (var y in s2)
                        if (y == data)
                        {
                            lis.Add(x.Key);
                            break;
                        }
                }
                return lis;
            });
            if (first)
                VocList.Children.Add(new Label { Text = "讀取中..." });
            Device.StartTimer(new TimeSpan(1000000), () => {
                bool tsk_fin = false, task_fin = false;
                if (tsk.IsCompleted && !tsk_fin)
                {
                    var lis = tsk.Result;
                    VocList.Children.Clear();
                    foreach (var x in lis)
                    {
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
                        tgr2.Tapped += Tgr2_Tapped; ;
                        stp.GestureRecognizers.Add(tgr2);
                        VocList.Children.Add(stp);
                    }
                    tsk_fin = true;
                }
                if (task.IsCompleted && !task_fin)
                {
                    var lis = task.Result;
                    alias_list.Children.Clear();
                    foreach (var x in lis)
                    {
                        var tmp = new Label { Text = x, Margin = 5 };
                        var tgr = new TapGestureRecognizer();
                        tgr.Tapped += Tgr_Tapped;
                        tmp.GestureRecognizers.Add(tgr);
                        alias_list.Children.Add(tmp);
                    }
                    task_fin = true;
                }
                if (tsk_fin && task_fin)
                    return false;
                return true;
            });
            /*Device.StartTimer(new TimeSpan(5000000), () => {
                if (Voc.rtp != null)
                {
                    VocList.Children.Clear();
                    SortedSet<string> tmp;
                    if (Voc.rtp.TryGetValue(data,out tmp))
                        foreach (var x in tmp)
                        {
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
                            tgr2.Tapped += Tgr2_Tapped; ;
                            stp.GestureRecognizers.Add(tgr2);

                            VocList.Children.Add(stp);
                        }
                    return false;
                }
                return true;
            });*/
            var tb = new Label {
                Margin=10,
                //FontSize=18,
            };
            if (Voc.note.exists(data))
                tb.Text = Voc.note.val(data);
	        else
                tb.Text = "備註(點擊修改)";
            var tb_tgr = new TapGestureRecognizer();
            tb_tgr.Tapped += Tb_tgr_Tapped; ;
            tb.GestureRecognizers.Add(tb_tgr);
            note_view.Content = tb;
            pics.Orientation = StackOrientation.Horizontal;

        }

        private async void  Tgr2_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SingleVocPage(((sender as StackLayout).Children[0] as Label).Text));
        }

        private void Tb_tgr_Tapped(object sender, EventArgs e)
        {
            var tb = new Entry {
                Text = Voc.note.exists(_voc) ? Voc.note.val(_voc) : "",
                Margin = 20,
                //FontSize = 18,
            };
            tb.Unfocused += Tb_Unfocused;
            note_view.Content = tb;
        }

        private void Tb_Unfocused(object sender, FocusEventArgs e)
        {
            var tmp = note_view.Content as Entry;
            if (tmp.Text != Voc.note.val(_voc))
                Voc.note.add(_voc, tmp.Text);
            else if (tmp.Text==""&&Voc.note.exists(_voc))
                Voc.note.remove(_voc);
            var tb = new Label
            {
                Margin = 10,
                //FontSize=18,
            };
            if (Voc.note.exists(_voc))
                tb.Text = Voc.note.val(_voc);
            else
                tb.Text = "備註(點擊修改)";
            var tb_tgr = new TapGestureRecognizer();
            tb_tgr.Tapped += Tb_tgr_Tapped; ;
            tb.GestureRecognizers.Add(tb_tgr);
            note_view.Content = tb;

        }

        private void Tmp_SelectedIndexChanged(object sender, EventArgs e)
        {
            var senderComboBox = sender as Picker;
            if (senderComboBox == null) return;
            for (int i = 0; i < voc_croot.Children.Count; i++)
                if (voc_croot.Children[i] == senderComboBox)
                {
                    wds.Item2[i] = senderComboBox.SelectedIndex;
                    Voc.words.add(_voc,Voc.MakeExp(wds));
                    break;
                }
        }

        private async void GoRootPage(object sender)
        {
            var senderTextBlock = sender as Label;
            if (senderTextBlock == null) return;
            for (int i = 0; i < voc_root.Children.Count; i++)
                if (voc_root.Children[i] == senderTextBlock)
                {
                    var s = Voc.Show2(_voc)[i];
                    if (s[0] != '-' && s[s.Length - 1] != '-')
                    {
                        s = Voc.WordRotToExp(s).Item2;
                        if (Voc.words.exists(s))
                            await Navigation.PushAsync(new SingleVocPage(s));
                    }
                    else
                        await Navigation.PushAsync(new SingleRootPage(s));
                }
            return;
        }
        private void Tgr_Tapped(object sender, EventArgs e)
        {
            GoRootPage(sender);
        }
        public SingleVocPage(string param)
        {
            //param.ToLower();
            param = param.Trim();
            //NavigationPage.SetHasNavigationBar(this, false);
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "favorite.png",
                Text = "最愛",
                Command = new Command(() => {
                    if (Voc.favorite.exists(_voc))
                        Voc.favorite.remove(_voc);
                    else
                        Voc.favorite.add(_voc, "");
                    Init(_voc, false);
                })
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "refresh.png",
                Text = "重整",
                Command = new Command(async() => {
                    await Refresh();
                })
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "edit.png",
                Text = "修改",
                Command = new Command(async () => {
                    var page = new MyPopupPage(this);
                    await PopupNavigation.PushAsync(page);
                    //await Navigation.PushPopupAsync(new MyPopupPage());
                })
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "delete.png",
                Text = "刪除",
                Command = new Command(async () =>
                {
                    var answer = await DisplayAlert("刪除", "真的要刪除嗎?", "是", "否");
                    if (answer == true)
                    {
                        Voc.words.remove(_voc);
                        await Navigation.PopAsync();
                    }
                })
            });
            ToolbarItems.Add(new ToolbarItem
            {
                Icon = "mute.png",
                Text = "發音",
                Command = new Command(() => { if (media_uri != null) DependencyService.Get<IAudio>().PlayMp3File(media_uri); })

            });
            Content = new ScrollView { Orientation = ScrollOrientation.Vertical, Content = grid, Margin = new Thickness(10, 10, 10, 5) };
            grid.Children.Add(tivoc, 0, 0);
            grid.Children.Add(kk, 0, 1);
            grid.Children.Add(expst, 0, 2);
            grid.Children.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = voc_root }, 0, 3);
            grid.Children.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = voc_croot }, 0, 4);
            grid.Children.Add(note_view, 0, 5);
            grid.Children.Add(alias_list, 0, 6);
            grid.Children.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = pics }, 0, 7);
            grid.Children.Add(VocList, 0, 8);
            if (Voc.words.exists(param))
            {
                if (_voc != null)
                    Init(param, false);
                else
                {
                    _voc = param;
                    Init(param);
                }
            }
            else
            {
                _voc = param;
                this.Appearing += async (s,e)=> { await Refresh(true); };
            }
        }
        async Task Refresh(bool first=false) {
            var web = await Voc.GetAsync("http://cn.bing.com/dict/search?mkt=zh-cn&q=" + _voc);
            if (web == null || web=="") {
                await DisplayAlert("錯誤", "網路錯誤", "了解");
                return;
            }
            string npa = _voc;
            var pb = web.IndexOf("<h1><strong>");
            if (pb != -1)
            {
                var tit = web.Substring(pb + 12);
                var pe = tit.IndexOf("<");
                if (pe != -1)
                {
                    npa = tit.Substring(0, pe);
                    if (npa.ToLower()!=_voc.ToLower()) npa = _voc;
                    if (npa != _voc)
                    {
                        Voc.note.remove(_voc);
                        Voc.words.remove(_voc);
                        _voc = npa;
                    }
                }
            }
            string disc = "", nt = "";
            var betip = web.IndexOf("<div class=\"in_tip\">");
            if (betip != -1)
            {
                web = web.Substring(betip + 20);
                var edtip = web.IndexOf("</div>");
                if (edtip != -1)
                {
                    string od = web.Substring(0, edtip);
                    foreach (var x in od)
                        if (x == ',')
                            nt += "，";
                        else if (x == '/')
                            nt += "／";
                        else
                            nt += x;
                }
            }
            while (true)
            {
                var be = web.IndexOf("<span class=\"pos\">");
                if (be == -1 || be + 18 >= web.Length) break;
                web = web.Substring(be + 18);
                var ed = web.IndexOf("</span><span class=\"def\"><span>");
                if (ed == -1 || ed + 31 >= web.Length) break;
                disc += "[" + web.Substring(0, ed) + "]";
                web = web.Substring(ed + 31);
                if (ed == -1) break;
                ed = web.IndexOf("</span></span>");
                string tmp = web.Substring(0, ed), str = "";
                bool inhtml = false;
                foreach (var x in tmp)
                    if (x == '[') str += '(';
                    else if (x == ']') str += ')';
                    else if (x == '<') inhtml = true;
                    else if (x == '>') inhtml = false;
                    else if (!inhtml) str += x;
                disc += str + " ";
            }
            while (true)
            {
                var be = web.IndexOf("<span class=\"pos web\">");
                if (be == -1 || be + 22 >= web.Length) break;
                web = web.Substring(be + 22);
                var ed = web.IndexOf("</span><span class=\"def\"><span>");
                if (ed == -1 || ed + 31 >= web.Length) break;
                disc += "[" + web.Substring(0, ed) + "]";
                web = web.Substring(ed + 31);
                if (ed == -1) break;
                ed = web.IndexOf("</span></span>");
                string tmp = web.Substring(0, ed), str = "";
                bool inhtml = false;
                foreach (var x in tmp)
                    if (x == '[') str += '(';
                    else if (x == ']') str += ')';
                    else if (x == '<') inhtml = true;
                    else if (x == '>') inhtml = false;
                    else if (!inhtml) str += x;
                disc += str + " ";
            }
            //ShowMsg(disc);
            //Explanation = ref new String(disc.c_str());
            string disc2 = "";
            foreach (var x in disc)
                if (x == ',')
                    disc2 += '，';
                else if (x == '/')
                    disc2 += '／';
                else
                    disc2 += x;
            if (disc2 == "")
            {
                await DisplayAlert("錯誤", "查無此字", "了解");
                return;
            }
            disc = Voc.s2t(disc2);
            Voc.words.add(_voc, disc);
            if (nt != "")
                Voc.note.add(_voc, Voc.s2t(nt));
            Init(_voc,first);
        }
        public class MyPopupPage : PopupPage
        {
            ContentPage back_page;
            public MyPopupPage(SingleVocPage page)
            {
                back_page = page;
                var input1 = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, };
                var input2 = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, };
                input1.Text = "";
                bool nf = false;
                for (int i = 0; i < page.voc_root.Children.Count; i++)
                {
                    if (nf) input1.Text += " "; nf = true;
                    input1.Text += (page.voc_root.Children[i] as Label).Text;
                }
                input2.Text = Voc.GetExpSimpleOrg(Voc.words.val(page._voc));
                var st = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = 30,
                    Children = {
                    new Label { Text = "應該如何拆解?" },
                    new StackLayout {
                        Orientation = StackOrientation.Vertical,Children= {
                            input1,
                            new Button {
                                Text ="確認",
                                Command = new Command(async()=> {
                                    string a=page._voc, b=input1.Text, c="";
                                    var tmp=b.Split(' ');
                                    foreach(var x in tmp)
                                        c+=x;
                                    if (c != a) {
                                        await DisplayAlert ("錯誤", "格式錯誤", "了解");
                                        return;
                                    }
                                    Voc.words.add_ok(a,b);
                                    page.Init(page._voc,false);
                                    await PopupNavigation.PopAsync();
                                }),
                            }
                        }
                    },
                    new Label { Text = "\n修改解釋" },
                    new StackLayout {
                        Orientation = StackOrientation.Vertical,Children= {
                            input2,
                            new Button {
                                Text ="確認",
                                Command = new Command(async()=> {
                                    string a=page._voc, b="",c=input2.Text;
                                    foreach (var x in c)
                                        if (x == ',')
                                            b += "，";
                                        else
                                            b += x;
                                    page.wds=Tuple.Create(b,page.wds.Item2);
                                    Voc.words.add(a,Voc.MakeExp(page.wds));
                                    b = Voc.GetExpSimple(Voc.words.val(a));
                                    page.Init(page._voc,false);
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

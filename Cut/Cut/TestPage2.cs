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
    public class TestPage2 : ContentPage
    {
        Grid grid = new Grid
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            RowDefinitions ={
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            },
        };
        Label title;
        StackLayout test_stp;
        int ac = 0, wa = 0;
        bool lck=false;
        string ans=null;
        ListView lis;
        ObservableCollection<string> lis_Items;
        void Init()
        {
            int cnt = 0;
            foreach (var x in Voc.favorite.data)
            {
                if (x.Value.Trim() == "")
                    cnt += 64;
                else
                    cnt += int.Parse(x.Value);
            }
            int rd = new Random().Next(cnt);
            cnt = 0;
            foreach (var x in Voc.favorite.data)
            {
                if (x.Value.Trim() == "")
                    cnt += 64;
                else
                    cnt += int.Parse(x.Value);
                if (cnt > rd)
                {
                    ans = x.Key;
                    break;
                }
            }
            if (ans == null)
            {
                title.Text = "請在我的最愛中加入單字";
                return;
            }
            //ve.RemoveRange(1, ve.Count - 1);
            //random_shuffle(ve.begin(), ve.end());

            List<string> words_ve= new List<string>();
            foreach (var x in Voc.words.data) words_ve.Add(x.Key);
            List<string> prob = new List<string>();
            prob.Add(ans);
            title.Text = "正確：" + ac.ToString() + " 錯誤：" + wa.ToString();
            test_stp.Children.Clear();
            var rand = new Random();
            int pbs;
            if (Voc.setting["sellect_prob_cnt"] == "")
			    pbs = 5;
		    else
			    pbs = Math.Max(2, Math.Min(100, int.Parse(Voc.setting["sellect_prob_cnt"])));
            for (int i = 0; i < pbs-1; i++)
            {
                for (int j = 0; ; j++)
                {
                    int p = rand.Next((int)words_ve.Count);
                    int dis = Voc.LevenshteinDistance(words_ve[p], ans);
                    foreach (var w in prob)
                        if (w == words_ve[p])
                            dis = 0;
                    if (dis >= 1 && dis <= 4 + j / 1000)
                    {
                        prob.Add(words_ve[p]);
                        break;
                    }
                }
            }
            int n = prob.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                var value = prob[k];
                prob[k] = prob[n];
                prob[n] = value;
            }
            //while (!event_token_queue.empty()) event_token_queue.pop();
            test_stp.Children.Clear();
            test_stp.Children.Add(Voc.ExpStack(Voc.GetExpSimple(Voc.words.val(ans))));
            /*
            var block = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = {
                    Voc.ExpStack(Voc.GetExpSimple(Voc.words.val(ans)))
                }
            };*/
            lis = new ListView();
            lis_Items = new ObservableCollection<string>();
            lis.ItemsSource = lis_Items;
            lis.ItemTapped += Lis_ItemTapped;
            foreach (var x in prob)
            {
                lis_Items.Add(x);
            }
            test_stp.Children.Add(lis);
            //block.Margin = new Thickness(0, 15, 0, 0);
            //test_stp.Children.Add(block);
        }

        private void Lis_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tmp = lis.SelectedItem as string;
            
            if (tmp == null||lck) return;
            lck = true;
            if (tmp == ans)
            {
                ac++;
                var tb = new Label
                {
                    Text = "正確",
                    TextColor = Color.Green,
                    Margin = 10
                };
                test_stp.Children.Add(tb);
                if (Voc.favorite.exists(ans))
                {
                    int x;
                    if (Voc.favorite.val(ans).Trim() == "")
                        x = 64;
                    else
                        x = int.Parse(Voc.favorite.val(ans));
                    Voc.favorite.add(ans, Math.Max(1, (int)(x / 1.2)).ToString());
                }
            }
            else
            {
                wa++;
                var tb = new Label
                {
                    Text = "錯誤",
                    TextColor = Color.Red,
                    Margin = 10
                };
                test_stp.Children.Add(tb);
                if (Voc.favorite.exists(ans))
                {
                    int x;
                    if (Voc.favorite.val(ans).Trim() == "")
                        x = 64;
                    else
                        x = int.Parse(Voc.favorite.val(ans));
                    Voc.favorite.add(ans, Math.Max(1, (int)(x + 32)).ToString());
                }
            }
            for (int i = 0; i < lis_Items.Count; i++)
                if (lis_Items[i] == ans)
                    lis_Items[i] = "> " + ans;
                else
                    lis_Items[i] = "  " + ans;
            lis.SelectedItem = null;
            Device.StartTimer(new TimeSpan(10000000), () => {
                Navigation.PopAsync();
                Navigation.PushAsync(new TestPage2(ac, wa));
                return false;
            });
        }
        

        public TestPage2(int _ac, int _wa)
        {
            ac = _ac;
            wa = _wa;
            Title = "單字測驗";
            
            title = new Label { Text = "正確：0 錯誤：0" }; ;
            //var tb1 = new Label { Text = "這份考卷在關掉此應用程式或是按右上重整前都不會消失喔!", HorizontalTextAlignment = TextAlignment.Center };
            //var tb2 = new Label { Text = "想看小抄請對【正確】/【錯誤】字樣上連點兩下", HorizontalTextAlignment = TextAlignment.Center };
            test_stp = new StackLayout { };
            grid.Children.Add(title, 0, 0);
            //grid.Children.Add(tb1, 0, 1);
            //grid.Children.Add(tb2, 0, 2);
            grid.Children.Add(test_stp, 0, 3);
            Content = grid;
            Init();
        }

    }
}
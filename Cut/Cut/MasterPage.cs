using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Cut
{
    public class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        
        ListView listView;

        public MasterPage()
        {
            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "家",
                //IconSource = "contacts.png",
                TargetType = typeof(HomePage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "單字查詢",
                //IconSource = "todo.png",
                TargetType = typeof(SearchVocPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "部首查詢",
                //IconSource = "reminders.png",
                TargetType = typeof(SearchRootPage)
            });
            /*masterPageItems.Add(new MasterPageItem
            {
                Title = "單字搜尋",
                TargetType = typeof(SearchVocPage)
            });*/
            masterPageItems.Add(new MasterPageItem
            {
                Title = "新增資料",
                TargetType = typeof(AddVocPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "我的最愛",
                TargetType = typeof(FavoritePage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "單字測驗",
                TargetType = typeof(TestPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "圖片掃描",
                TargetType = typeof(PhotoPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "設定",
                TargetType = typeof(SettingPage)
            });
            listView = new ListView
            {
                ItemsSource = masterPageItems,
                ItemTemplate = new DataTemplate(() => {
                    //var imageCell = new ImageCell();
                    //imageCell.SetBinding(TextCell.TextProperty, "Title");
                    //return imageCell;
                    //imageCell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");
                    var Cell = new TextCell();
                    Cell.SetBinding(TextCell.TextProperty, "Title");
                    //Cell.TextColor = Color.Aqua;
                    return Cell;
                }),
                VerticalOptions = LayoutOptions.FillAndExpand,
                SeparatorVisibility = SeparatorVisibility.None
            };

            Padding = new Thickness(0, 40, 0, 0);
            Icon = "hamburger.png";
            Title = "Personal Organiser";
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    listView
                }
            };
        }
    }
}

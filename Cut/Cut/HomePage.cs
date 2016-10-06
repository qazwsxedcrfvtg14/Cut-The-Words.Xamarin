using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace Cut
{
    public class HomePage : ContentPage
    {
        public HomePage()
        {
            Title = "Home";
            int fontsize = 16;
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Margin=10,
                    HorizontalOptions = LayoutOptions.Center,
                    Children = {
                    new Label {
                        Text = "請按左上角的漢堡鍵(三條橫線)開啟選單",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "0.家：使用說明",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "1.單字查詢：查詢單字(連網時可查詢不在單字庫的單字)",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "2.部首查詢：查詢部首",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "3.新增資料：增加新的單字/部首到本機資料庫中",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "4.我的最愛：查看自訂單字表",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "5.單字測驗：練習自訂單字表中的單字",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "6.設定：設定自動發音開關、更新單字庫、測驗選項數量",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "  (字根字首部分不保證完全正確)",
                        FontSize=fontsize
                    },
                    new Label {
                        Text = "建議第一次使用前先更新單字庫",
                        FontSize=fontsize
                    },
                    new Button {
                        Text = "資料格式說明",
                        HorizontalOptions = LayoutOptions.Center,
                        Command = new Command(async()=> {
                            await DisplayAlert("資料格式說明",
                                "單字解釋的格式：\n"+
                                "  [] 中填寫詞性且須寫在開頭，當遇到下組[]時會自動分隔\n"+
                                "  整段解釋內均不可出現\"/\"字元\n"+
                                "  可使用\"=XXX\"來代表此單字跟XXX相等\n"+
                                "\n"+
                                "部首解釋的格式：\n"+
                                "  請以\"/\"字元分段，其餘部分隨意填寫\n"+
                                "  可使用\"=OOO\"來代表此部首跟OOO相等\n",
                                "了解");
                        })
                    }
                }
                }
            };
        }
    }
}

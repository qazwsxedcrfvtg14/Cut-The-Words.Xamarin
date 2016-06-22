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
            Button btn = new Button { Text = "Test" };
            var fileService = DependencyService.Get<ISaveAndLoad>();
            btn.Clicked += async (sender, e) => {
                bool x=fileService.FileExists("root.txt");
                if (x)
                    btn.Text = await fileService.LoadTextAsync("root.txt");
                else
                    btn.Text = "404";
            };
            Button btn2 = new Button { Text = "Test2" };
            btn2.Clicked += async (sender, e) => {
                btn.IsEnabled = false;
                await fileService.SaveTextAsync("a.txt","gg");
                btn.IsEnabled = true;
            };
            Button btn3 = new Button { Text = "Test3" };
            btn3.Clicked += async (sender, e) => {
                await Navigation.PushAsync(new SearchVocPage());
            };
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var tmp=assembly.GetManifestResourceNames();
            Stream stream = assembly.GetManifestResourceStream("Cut.data.tmp.txt");
            using (var reader = new System.IO.StreamReader(stream))
            {
                btn3.Text = reader.ReadToEnd();
            }


            Content = new StackLayout
            {
                Children = {
                    new Label {
                        Text = "家",
                        HorizontalOptions = LayoutOptions.Center,
                        //VerticalOptions = LayoutOptions.CenterAndExpand
                    },
                    btn,btn2,btn3
                }
            };
        }
    }
}

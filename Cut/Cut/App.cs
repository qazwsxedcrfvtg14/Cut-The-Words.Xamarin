using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Cut
{
    
    public class App : Application
    {
        public App()
        {


            // The root page of your application
            MainPage = new MainPage();

        }
        protected override void OnStart()
        {
            // Handle when your app starts
            
            /*
            Voc.rtp = new SortedDictionary<string, SortedSet<string>>();
            foreach(var x in Voc.words.data)
            {
                var ve = Voc.Show2(x.Key);
                foreach (var y in ve) {
                    string z = y;
                    if (z[0] != '-' && z[y.Length - 1] != '-')
                        z = Voc.WordRotToExp(z).Item2;
                    if (z != x.Key)
                    {
                        SortedSet<string> tmp;
                        if (!Voc.rtp.TryGetValue(z, out tmp))
                            Voc.rtp.Add(z,tmp = new SortedSet<string>());
                        tmp.Add(x.Key);
                    }
                }
            }
            */
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

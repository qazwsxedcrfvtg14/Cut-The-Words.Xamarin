using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Cut
{
    /*class Map
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        public void add(string key, string value = "")
        {
            data.Add(key, value);
        }
        public void remove(string key)
        {
            data.Remove(key);
        }
        public bool exists(string key)
        {
            string value;
            if (data.TryGetValue(key, out value)) return true;
            else return false;
        }
        public string val(string key)
        {
            string value;
            if (data.TryGetValue(key, out value)) return value;
            else return null;
        }
    }*/
    public class Maps
    {
        public SortedDictionary<string, string> data = new SortedDictionary<string, string>();
        public SortedDictionary<string, string> ok = new SortedDictionary<string, string>();
        public string file_name;
        ISaveAndLoad fileService = DependencyService.Get<ISaveAndLoad>();
        public void add(string key, string value = "")
        {
            data[key] = value;
            if (file_name != null) {
                if(exists_ok(key))
                    fileService.AppendTextAsync(file_name, "*" + ok[key] + "," + data[key] + "\n");
                else
                    fileService.AppendTextAsync(file_name, key + "," + data[key] + "\n");
            }
        }
        public void add_ok(string key, string value = "")
        {
            ok[key] = value;
            if (file_name != null)
            {
                if (exists_ok(key))
                    fileService.AppendTextAsync(file_name, "*" + ok[key] + "," + data[key] + "\n");
                else
                    fileService.AppendTextAsync(file_name, key + "," + data[key] + "\n");
            }
        }
        public void remove(string key)
        {
            data.Remove(key);
            ok.Remove(key);
            if (file_name != null)
            {
                    fileService.AppendTextAsync("$" + file_name, key + "\n");
            }
        }
        public bool exists(string key)
        {
            string value;
            if (data.TryGetValue(key, out value)) return true;
            else return false;
        }
        public bool exists_ok(string key)
        {
            string value;
            if (ok.TryGetValue(key, out value)) return true;
            else return false;
        }
        public string val(string key)
        {
            string value;
            if (data.TryGetValue(key, out value)) return value;
            else return null;
        }
        public string val_ok(string key)
        {
            string value;
            if (ok.TryGetValue(key, out value)) return value;
            else return null;
        }
        public void clear()
        {
            data.Clear();
            ok.Clear();
        }
    }
}

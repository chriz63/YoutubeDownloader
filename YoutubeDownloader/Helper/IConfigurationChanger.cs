using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Helper
{
    public interface IConfigurationChanger
    {
        public bool Change(string key, string value);
    }
}

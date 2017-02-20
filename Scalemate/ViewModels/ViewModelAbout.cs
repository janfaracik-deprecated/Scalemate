using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Scalemate.ViewModels
{
    public class ViewModelAbout
    {

        public String AppName { get; set; }
        public String Version { get; set; }

        public ViewModelAbout()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            AppName = "Scalemate";
            Version = string.Format("{0}.{1}", version.Major, version.Minor);
        }

    }
}

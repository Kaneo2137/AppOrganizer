using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOrganizer.Models
{
    public class ProfileModel
    {
        public string Name { get; set; } = string.Empty;

        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();

        public override string ToString()
        {
            return Name;
        }
    }

    public class Program
    {
        public string Executable { get; set; } = string.Empty;

        public string Arguments { get; set; } = string.Empty;

        public override string ToString()
        {
            return Executable;
        }
    }
}

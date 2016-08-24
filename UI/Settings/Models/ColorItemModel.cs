using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace UI.Settings.Models
{
    public class ColorItemModel : ItemModel
    {
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }
        private string _Code = string.Empty;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        private string _Name = string.Empty;

        public string CodeAid
        {
            get { return _CodeAid; }
            set { _CodeAid = value; OnPropertyChanged("CodeAid"); }
        }
        private string _CodeAid = string.Empty;

        public string Amount
        {
            get { return _Amount; }
            set { _Amount = value; OnPropertyChanged("Amount"); }
        }
        private string _Amount = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace UI.Settings.Models
{
    public class CustomerItemModel : ItemModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }
}

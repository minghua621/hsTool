using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public abstract class ListModel<TItemModel> : ObservableCollection<TItemModel>
        where TItemModel : ItemModel
    {
        public ListModel()
        {
            Load();
        }
        public abstract void Load();
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using UI.Localization.Messages;

namespace UI.Localization
{
    public sealed class ResourceWrapper : NotificationBase
    {
        public ApplicationStrings ApplicationString
        {
            get { return _ApplicationString; }
            set { _ApplicationString = value; OnPropertyChanged("ApplicationString"); }
        }
        private ApplicationStrings _ApplicationString;

        public MessageStrings MessageStrings
        {
            get { return _MessageStrings; }
            set { _MessageStrings = value; OnPropertyChanged("MessageStrings"); }
        }
        private MessageStrings _MessageStrings;

        public static string CurrentCultrue
        {
            get { return _currentCulture.Name; }
            set
            {
                _currentCulture = new CultureInfo(value);
                System.Threading.Thread.CurrentThread.CurrentUICulture = _currentCulture;
                ResourceWrapper rm = System.Windows.Application.Current.Resources["ResourceWrapper"] as ResourceWrapper;
                rm.ApplicationString = new ApplicationStrings();
                rm.MessageStrings = new MessageStrings();
            }
        }
        private static CultureInfo _currentCulture;
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Common;
using Common.Command;
using UI.Settings.Models;
using UI.Settings.ViewModels;
using UI.Product.Models;
using UI.Main;
using UI.Localization.Messages;
using ClosedXML.Excel;

namespace UI.Product.ViewModels
{
    public class InvoiceVM : NotificationBase
    {
        #region fields

        public ObservableCollection<CustomerItemModel> Customers
        {
            get { return CustomerSettinigsVM.CustomerSettinigs.Items; }
        }

        public CustomerItemModel SelectedCustomer
        {
            get { return _SelectedCustomer; }
            set 
            { 
                _SelectedCustomer = value; 
                OnPropertyChanged("SelectedCustomer");
                OnPropertyChanged("Items");
                OnPropertyChanged("ShipmentTotal");
                OnPropertyChanged("PersentTax");
                OnPropertyChanged("Total");
            }
        }
        private CustomerItemModel _SelectedCustomer;

        public DateTime SelectedMonth 
        {
            get { return _SelectedMonth; }
            set
            {
                _SelectedMonth = value;
                OnPropertyChanged("SelectedMonth");
                OnPropertyChanged("Items");
                OnPropertyChanged("ShipmentTotal");
                OnPropertyChanged("PersentTax");
                OnPropertyChanged("Total");
            }
        }
        private DateTime _SelectedMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public List<InvoiceItemModel> Items
        {
            get { return (_SelectedCustomer != null && SelectedMonth != null) ? Dao.InvoiceDao.GetInvoiceList(_SelectedCustomer.Code, SelectedMonth) : null; }
        }

        public int ShipmentTotal
        {
            get
            {
                double rlt = 0;
                if (Items != null)
                {
                    foreach (InvoiceItemModel item in Items)
                    {
                        rlt += item.SubTotal;
                    }
                }
                return Convert.ToInt32(rlt);
            }
        }

        public bool PersentTaxChecked
        {
            get { return _PersentTaxChecked; }
            set { _PersentTaxChecked = value; OnPropertyChanged("PersentTaxChecked"); OnPropertyChanged("Total"); }
        }
        private bool _PersentTaxChecked = true;

        public int PersentTax
        {
            get
            {
                double rlt = Math.Round(ShipmentTotal * 0.05, MidpointRounding.AwayFromZero);
                return Convert.ToInt32(rlt);
            }
        }

        public bool ManualTaxChecked
        {
            get { return _ManualTaxChecked; }
            set { _ManualTaxChecked = value; OnPropertyChanged("ManualTaxChecked"); OnPropertyChanged("Total"); }
        }
        private bool _ManualTaxChecked = false;

        public int ManualTax
        {
            get { return _ManualTax; }
            set { _ManualTax = value; OnPropertyChanged("_ManualTax"); OnPropertyChanged("Total"); }
        }
        private int _ManualTax = 0;

        public int Total
        {
            get
            {
                return ShipmentTotal + (PersentTaxChecked == true ? PersentTax : ManualTax);
            }
        }
        #endregion

        #region commands

        public ICommand ExportCommand
        {
            get
            {
                return new ActiveDelegateCommand<InvoiceVM>(this,
                    (p) => { Export(); },
                    (p) => { return (Items == null ? Items != null : Items.Count != 0) && Total != 0; });
            }
        }
        #endregion

        #region methods

        private void Output(IXLWorksheet ws, int startCell)
        {
            int cell = startCell;
            //title
            ws.Cell(string.Format("A{0}", cell - 4)).Value = SelectedCustomer.FullName;
            ws.Cell(string.Format("A{0}", cell - 3)).Value = string.Format(ApplicationStrings.invoice_phone_number, SelectedCustomer.Phone);
            ws.Cell(string.Format("D{0}", cell - 3)).Value = string.Format(ApplicationStrings.invoice_month, SelectedMonth.Year - 1911, SelectedMonth.Month);

            //shipment
            foreach (InvoiceItemModel item in Items)
            {
                ws.Cell(string.Format("A{0}", cell)).Value = item.ProductName;
                ws.Cell(string.Format("B{0}", cell)).Value = item.Price;
                ws.Cell(string.Format("C{0}", cell)).Value = item.Qty;
                ws.Cell(string.Format("D{0}", cell)).Value = item.SubTotal;
                cell++;
            }

            //total
            ws.Cell(string.Format("A{0}", ++cell)).Value = ApplicationStrings.header_invoice_shipment_total;
            ws.Cell(string.Format("D{0}", cell)).Value = ShipmentTotal;
            ws.Cell(string.Format("A{0}", ++cell)).Value = ApplicationStrings.header_tax;
            if (PersentTaxChecked)
            {
                ws.Cell(string.Format("D{0}", cell)).Value = PersentTax;
            }
            else
            {
                ws.Cell(string.Format("D{0}", cell)).Value = ManualTax;
            }
            ws.Cell(string.Format("A{0}", ++cell)).Value = ApplicationStrings.header_invoice_total;
            ws.Cell(string.Format("D{0}", cell)).Value = Total;

            //set border
            var rngTable = ws.Range(string.Format("A{0}:D{1}", startCell, cell++));
            rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rngTable.Style.Border.OutsideBorderColor = XLColor.Black;
            rngTable.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rngTable.Style.Border.InsideBorderColor = XLColor.Black;

            //footer            
            ws.Cell(string.Format("A{0}", ++cell)).Value = AppSettings.Company.FullName;
            ws.Cell(string.Format("D{0}", cell)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell(string.Format("D{0}", cell)).Value = string.Format(ApplicationStrings.invoice_date, DateTime.Now.Year - 1911, DateTime.Now.Month, DateTime.Now.Day);
            ws.Cell(string.Format("A{0}", ++cell)).Value = string.Format(ApplicationStrings.invoice_phone_number, AppSettings.Company.Phone);
        }
        private void Export()
        {
            string fileName = AppSettings.InvoiceTemplate;
            XLWorkbook workbook = new XLWorkbook(fileName);
            IXLWorksheet ws = workbook.Worksheet(SelectedCustomer.InvoiceFormat);

            Output(ws, 7);
            if (SelectedCustomer.InvoiceFormat == 2)
            {
                Output(ws, 32);
            }
            
            //delete other sheets            
            for (int i = workbook.Worksheets.Count; i >= 1; i--)
            {
                if (i != SelectedCustomer.InvoiceFormat)
                {
                    workbook.Worksheet(i).Delete();
                }
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = string.Format("{0}-{1}", SelectedCustomer.Name, SelectedMonth.ToString("yyyyMM")); // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                workbook.SaveAs(filename);
            }
        }
        #endregion
    }
}
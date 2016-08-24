using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Theme.Controls
{
    public class MonthUpDown : UserControl
    {
        private TextBox valueTextBox;
        private string monthFormat = "yyyy/MM";
        private string monthSeperator = "/";

        public MonthUpDown()
        {
            this.Loaded += MonthUpDown_Loaded;
        }

        private void MonthUpDown_Loaded(object sender, RoutedEventArgs e)
        {
            if (valueTextBox != null)
            {
                this.Loaded -= MonthUpDown_Loaded;
                Value = DateTime.Now;
                Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                valueTextBox.SelectionStart = valueTextBox.Text.IndexOf(monthSeperator) + 1;
                valueTextBox.SelectionLength = 2;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RepeatButton upBtn = GetTemplateChild("upButton") as RepeatButton;
            RepeatButton downBtn = GetTemplateChild("downButton") as RepeatButton;
            valueTextBox = GetTemplateChild("valueTextBox") as TextBox;

            upBtn.Click += upBtn_Click;
            downBtn.Click += downBtn_Click;
        }

        private void upBtn_Click(object sender, RoutedEventArgs e)
        {
            if(isYearSelected())
            {
                Value = Value.AddYears(Increment);
                FocusYear();
            }
            else 
            {
                Value = Value.AddMonths(Increment);
                FocusMonth();
            }
        }

        private void downBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isYearSelected())
            {
                Value = Value.AddYears(-Increment);
                FocusYear();
            }
            else
            {
                Value = Value.AddMonths(-Increment);
                FocusMonth();
            }
        }

        private void ValueChanged()
        {
            if (valueTextBox != null)
            {
                valueTextBox.Text = Value.ToString(monthFormat);
            }
        }

        private bool isYearSelected()
        {
            int cur = valueTextBox.SelectionStart;
            int slash = valueTextBox.Text.IndexOf(monthSeperator);
            return cur <= slash ? true : false;
        }

        private void FocusYear()
        {     
            valueTextBox.SelectionStart = 0;
            valueTextBox.SelectionLength = 4;
            valueTextBox.Focus();
        }

        private void FocusMonth()
        {            
            valueTextBox.SelectionStart = valueTextBox.Text.IndexOf(monthSeperator) + 1;
            valueTextBox.SelectionLength = 2;
            valueTextBox.Focus();            
        }

        #region dependency properties

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime), typeof(MonthUpDown), new PropertyMetadata((d, e) => { ((MonthUpDown)d).ValueChanged(); }));
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(int), typeof(MonthUpDown), new PropertyMetadata(1));
        public int Increment
        {
            get { return (int)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        #endregion
    }
}

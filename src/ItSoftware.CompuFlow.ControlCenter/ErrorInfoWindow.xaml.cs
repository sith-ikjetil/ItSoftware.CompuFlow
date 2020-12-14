using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ItSoftware.CompuFlow.ControlCenter
{
    /// <summary>
    /// Interaction logic for WindowErrorInfo.xaml
    /// </summary>
    public partial class ErrorInfoWindow : Window
    {
        public ErrorInfoWindow()
        {
            InitializeComponent();
        }

        public string ErrorInformation
        {
            get
            {
                return this.textBoxErrorInfo.Text;
            }
            set
            {
                this.textBoxErrorInfo.Text = value;
            }
        }

        private void LabelLinkCopy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(this.textBoxErrorInfo.Text);
        }

        private void LabelLinkExit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}

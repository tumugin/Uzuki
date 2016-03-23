using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Uzuki.Dialogs.MainWindow;
using Uzuki.Settings;

namespace Uzuki.Dialogs
{
    /// <summary>
    /// SettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingDialog : MetroWindow
    {
        public SettingDialog()
        {
            InitializeComponent();
            //Bind to settings object
            BBSMenuPathTextBox.SetBinding(TextBox.TextProperty, new Binding("BBSMenuPath") { Source = SingletonManager.MainWindowSingleton.SetMannage });
            UseBlackThemeCheckBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("UseBlackTheme") { Source = SingletonManager.MainWindowSingleton.SetMannage });
            NGListViewContainer.NGListView.ItemsSource = SingletonManager.MainWindowSingleton.SetMannage.NGIDCollection;
            BuildVersionLabel.Content += Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private void DeleteSelectedOnClick(object sender, RoutedEventArgs e)
        {
            if(NGListViewContainer.NGListView.SelectedItems.Count != 0) SingletonManager.MainWindowSingleton.SetMannage.NGIDCollection.RemoveAt(NGListViewContainer.NGListView.SelectedIndex);
        }
    }
}

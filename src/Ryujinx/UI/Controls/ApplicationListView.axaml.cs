using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Controls;
using Ryujinx.Ava.UI.Helpers;
using Ryujinx.Ava.UI.ViewModels;
using Ryujinx.UI.App.Common;
using System;
using System.Linq;

namespace Ryujinx.Ava.UI.Controls
{
    public partial class ApplicationListView : UserControl
    {
        public static readonly RoutedEvent<ApplicationOpenedEventArgs> ApplicationOpenedEvent =
            RoutedEvent.Register<ApplicationListView, ApplicationOpenedEventArgs>(nameof(ApplicationOpened), RoutingStrategies.Bubble);

        public event EventHandler<ApplicationOpenedEventArgs> ApplicationOpened
        {
            add { AddHandler(ApplicationOpenedEvent, value); }
            remove { RemoveHandler(ApplicationOpenedEvent, value); }
        }

        public ApplicationListView()
        {
            InitializeComponent();
        }

        public void GameList_DoubleTapped(object sender, TappedEventArgs args)
        {
            if (sender is ListBox listBox)
            {
                if (listBox.SelectedItem is ApplicationData selected)
                {
                    RaiseEvent(new ApplicationOpenedEventArgs(selected, ApplicationOpenedEvent));
                }
            }
        }

        public void GameList_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ListBox listBox)
            {
                (DataContext as MainWindowViewModel).ListSelectedApplication = listBox.SelectedItem as ApplicationData;
            }
        }

        private void SearchBox_OnKeyUp(object sender, KeyEventArgs args)
        {
            (DataContext as MainWindowViewModel).SearchText = (sender as TextBox).Text;
        }

        private async void IdString_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainWindowViewModel mwvm)
                return;
            
            if (sender is not Button { Content: TextBlock idText })
                return;
            
            if (App.MainWindow.Clipboard is { } clipboard)
            {
                var appData = mwvm.Applications.FirstOrDefault(it => it.IdString == idText.Text);
                if (appData is null)
                    return;
                
                await clipboard.SetTextAsync(appData.IdString);
                
                NotificationHelper.Show("Copied Title ID", $"{appData.Name} ({appData.IdString})", NotificationType.Information);
            }
        }
    }
}

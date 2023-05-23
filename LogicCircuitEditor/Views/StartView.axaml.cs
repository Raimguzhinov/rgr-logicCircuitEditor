using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using LogicCircuitEditor.ViewModels;

namespace LogicCircuitEditor.Views
{
    public partial class StartView : UserControl
    {
        public StartView()
        {
            InitializeComponent();
        }

        private void CreateProjectButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.GetLogicalParent() is MainWindow mw)
            {
                if (mw.DataContext is MainWindowViewModel mainWindow) mainWindow.CreateNewProject();
            }
        }

        private void OpenProjectButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (DataContext is StartViewModel start)
            {
                if (this.GetLogicalParent() is MainWindow mw)
                {
                    if (start.Index > -1) if (mw.DataContext is MainWindowViewModel mainWindow) mainWindow.OpenProject(start.Projects[start.Index].Path);
                }
            }
        }
        private void CloseWindowButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.GetLogicalParent() is MainWindow mw) mw.Close();
        }
    }
}
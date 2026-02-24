using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel(
            closeWindow: () => Close(),
            filePicker: async () =>
            {
                var result = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "エディタ実行ファイルを選択",
                    AllowMultiple = false
                });
                return result.Count > 0 ? result[0].Path.LocalPath : null;
            },
            showError: async (message) =>
            {
                var dialog = new Window
                {
                    Title = "エラー",
                    Width = 400,
                    Height = 150,
                    Content = new Avalonia.Controls.TextBlock
                    {
                        Text = message,
                        Margin = new Avalonia.Thickness(20),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                };
                await dialog.ShowDialog(this);
            });
    }
}

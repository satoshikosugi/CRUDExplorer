using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class MakeCrudWindow : Window
{
    public MakeCrudWindow()
    {
        InitializeComponent();

        // StorageProvider と Close() をコールバックとして渡すことで
        // ViewModel がウィンドウを直接参照せずにダイアログを開ける
        DataContext = new MakeCrudViewModel(
            folderPicker: async (title) =>
            {
                var result = await StorageProvider.OpenFolderPickerAsync(
                    new FolderPickerOpenOptions { Title = title, AllowMultiple = false });
                return result.Count > 0 ? result[0].Path.LocalPath : null;
            },
            closeWindow: () => Close()
        );
    }
}

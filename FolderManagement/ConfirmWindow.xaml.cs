using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace FolderManagement
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public bool IsCreatingFile { get; private set; }
        public string Path { get; private set; }
        public List<MainWindow.Item> ItemList { get; private set; }
        public ConfirmWindow(List<MainWindow.Item> itemList, string path)
        {
            InitializeComponent();
            ItemList = itemList;
            Path = path;
        }

        private void file_btn_Click(object sender, RoutedEventArgs e)
        {
            file_btn.Visibility = Visibility.Hidden;
            file_name_label.Visibility = Visibility.Visible;
            file_name_txt.Visibility = Visibility.Visible;
            ok_btn.Visibility = Visibility.Visible;
            folder_btn.Visibility= Visibility.Hidden;
            IsCreatingFile = true;
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            switch (IsCreatingFile)
            {
                case true:
                    createFile(file_name_txt.Text, Path);
                    break;
                case false:
                    createFolder(folder_name_txt.Text, Path);
                    break;
            }
        }

        private void folder_btn_Click(object sender, RoutedEventArgs e)
        {
            folder_btn.Visibility = Visibility.Hidden;
            folder_name_label.Visibility = Visibility.Visible;
            folder_name_txt.Visibility = Visibility.Visible;
            ok_btn.Visibility = Visibility.Visible;
            file_btn.Visibility = Visibility.Hidden;
            IsCreatingFile = false;
        }

        private void createFile(string filename, string path)
        {
            if (!filename.Contains("."))
            {
                filename += ".txt";
            }
            try
            {
                string filePath = System.IO.Path.Combine(path, filename);
                if (File.Exists(filePath)){
                    MessageBox.Show("Existed file! Please try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                File.Create(filePath);
                MessageBox.Show("Create new file successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                ItemList.Add(new MainWindow.Item
                {
                    Name = filename,
                    Path = filePath
                });
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.scanFilesAndSubfolders(Path);
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private void createFolder(string foldername, string path)
        {
            try
            {
                string folderPath = System.IO.Path.Combine(path, foldername);
                if (Directory.Exists(folderPath))
                {
                    MessageBox.Show("Existed folder! Please try again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Directory.CreateDirectory(folderPath);
                MessageBox.Show("Create new folder successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                ItemList.Add(new MainWindow.Item
                {
                    Name = foldername,
                    Path = folderPath
                });
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.scanFilesAndSubfolders(Path);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

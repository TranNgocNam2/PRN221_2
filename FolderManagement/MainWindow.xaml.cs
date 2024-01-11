using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinForm = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using Path = System.IO.Path;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Diagnostics;

namespace FolderManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            filesListView.SelectionChanged += filesListView_SelectionChanged;
            //filesListView.MouseDoubleClick += filesListView_MouseDoubleClick;
        }
        private string rootFolderPath = "";
        public class Item
        {
            public ImageSource? Type { get; set; }
            public string? Name { get; set; }
            public string? Path { get; set; }
        }
        private void browse_file_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WinForm.FolderBrowserDialog dialog = new WinForm.FolderBrowserDialog();
                dialog.InitialDirectory = "";
                WinForm.DialogResult result = dialog.ShowDialog();
                if (result == WinForm.DialogResult.OK)
                {
                    rootFolderPath = dialog.SelectedPath;
                    folder_path.Text = rootFolderPath;
                    scanFilesAndSubfolders(rootFolderPath);
                }
            }catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ImageSource? GetApplicationIcon(string path)
        {
            ImageSource? appIcon = null;
            try { 
            
                if (File.Exists(path))
                {
                    System.Drawing.Icon? fileIcon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                    appIcon = Imaging.CreateBitmapSourceFromHIcon(fileIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                else if(Directory.Exists(path))
                {
                    System.Drawing.Icon? folderIcon = Image.GetFolderIcon();
                    appIcon = Imaging.CreateBitmapSourceFromHIcon(folderIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting application icon: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return appIcon;
        }

        public void scanFilesAndSubfolders(string path)
        {

            var list = new List<Item>();
            list.Clear();
            try
            {
                var files = Directory.GetFiles(path);
                var folders = Directory.GetDirectories(path);
                
                foreach (var file in files)
                {
                    string filePath = Path.GetFullPath(file);
                    list.Add(new Item 
                    { 
                        Type = GetApplicationIcon(filePath), 
                        Name = Path.GetFileName(file), 
                        Path = Path.GetFullPath(file)
                    });
                } 

                foreach (var folder in folders)
                {
                    list.Add(new Item 
                    { 
                        Type = GetApplicationIcon(path), 
                        Name = Path.GetFileName(folder), 
                        Path = Path.GetFullPath(folder) 
                    });
                }

                // Bind the items to the ListView
                filesListView.ItemsSource = list;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error displaying files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void create_btn_Click(object sender, RoutedEventArgs e)
        {
            if(rootFolderPath == null || rootFolderPath.Length == 0)
            {
                MessageBox.Show("Folder name cannot empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var confirmWindow = new ConfirmWindow((List<Item>) filesListView.ItemsSource, rootFolderPath);
            confirmWindow.ShowDialog();
            folder_path.Text = rootFolderPath;
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(folder_path.Text))
            {
                string pathToDelete = folder_path.Text;
                if (File.Exists(pathToDelete))
                {
                    MessageBoxResult result = MessageBox.Show($"Do you want to delete file: {Path.GetFileName(pathToDelete)}?", "Notification", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    deleteFile(pathToDelete);
                    return;
                }else if (Directory.Exists(pathToDelete)){
                    MessageBoxResult result = MessageBox.Show($"Do you want to delete folder: {Path.GetFileName(pathToDelete)}?", "Notification", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    deleteFolder(pathToDelete); 
                    return;
                }
                else
                {
                    MessageBox.Show("The specified path does not exist!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a file or folder to delete!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void deleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                MessageBox.Show($"File '{Path.GetFileName(filePath)}' deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                folder_path.Text = rootFolderPath;
                scanFilesAndSubfolders(rootFolderPath);
            }
            catch(Exception ex) {
                MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void deleteFolder(string folderPath)
        {
            try
            {
                Directory.Delete(folderPath, true);
                MessageBox.Show($"Folder '{Path.GetFileName(folderPath)}' deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                folder_path.Text = rootFolderPath;
                scanFilesAndSubfolders(rootFolderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void filesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(filesListView.SelectedItems != null && filesListView.SelectedItems.Count > 0)
            {
                MainWindow.Item selectedFile = (MainWindow.Item)filesListView.SelectedItem;
                if (selectedFile != null && !string.IsNullOrEmpty(selectedFile.Path))
                {
                    folder_path.Text = selectedFile.Path;
                }
            }
        }

        private void filesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (filesListView.SelectedItems != null && filesListView.SelectedItems.Count > 0)
            {
                MainWindow.Item selectedItem = (MainWindow.Item)filesListView.SelectedItem;
                if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.Path))
                {
                    if (Directory.Exists(selectedItem.Path)) {
                        Process.Start("explorer.exe", selectedItem.Path);
                    }
                    else if (File.Exists(selectedItem.Path))
                    {
                        try 
                        {
                            System.Diagnostics.Process.Start(new ProcessStartInfo(selectedItem.Path) { UseShellExecute = true });
                        }
                        catch (Exception ex) 
                        {
                            MessageBox.Show($"Error: {ex.Message}");
                        }
                        
                    }
                }
            }
        }

        private void previous_folder_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(rootFolderPath))
            {
                DirectoryInfo currentDirectory = new DirectoryInfo(rootFolderPath);
                if (currentDirectory.Parent != null)
                {
                    rootFolderPath = currentDirectory.Parent.FullName;
                    folder_path.Text = rootFolderPath;
                    scanFilesAndSubfolders(rootFolderPath);
                }
            }
        }
    }     
}
using InfoServerDataModels;
using Microsoft.Win32;
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

namespace ClientGUI
{
    /// <summary>
    /// Логика взаимодействия для CreationPage.xaml
    /// </summary>
    public partial class CreationPage : Window
    {
        string curImagePath;

        bool editing;

        int id;
        string name;
        byte[] image;

        MainWindow mainWindow;

        public CreationPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
        public CreationPage(MainWindow mainWindow, Record record)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            InitializeRecord(record);
        }

        void InitializeRecord(Record record)
        {
            editing = true;
            id = record.Id;
            name = record.Name;
            image = (byte[])record.Image.Clone();

            ShowCurRecord();
        }

        void ShowCurRecord()
        {
            nameBox.Text = name;
            ShowImage();
        }
        
        private void selectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                curImagePath = openFileDialog.FileName;
                imagePath.Text = curImagePath;

                ConvertImage(curImagePath);
                ShowImage();
            }
        }

        /*void ShowImage(string path)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(curImagePath);
            image.EndInit();

            imagePreview.Source = image;
            imagePath.Text = curImagePath;
        }*/

        void ConvertImage(string path)
        {
            image = File.ReadAllBytes(path);
        }

        void ShowImage()
        {
            using (var ms = new System.IO.MemoryStream(image))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();

                imagePreview.Source = image;
                imagePath.Text = curImagePath;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (editing)
                mainWindow.EditRecord(id, name, image);
            else
                mainWindow.AddRecord(name, image);
            this.Close();
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (editing && mainWindow.ContainsRecord(id))
            {
                mainWindow.DeleteRecord(id);
            }
            this.Close();
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            name = nameBox.Text;
        }
    }
}

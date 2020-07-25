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
    public partial class RecordPage : Window
    {
        string curImagePath;

        bool editing;

        public int recordId;
        string name;
        byte[] image;

        MainWindow mainWindow;

        public RecordPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }
        public RecordPage(MainWindow mainWindow, Record record)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            InitializeRecord(record);
        }

        void InitializeRecord(Record record)
        {
            editing = true;
            recordId = record.Id;
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
            if (!VerifyRecord())
            {
                return;
            }

            if (editing)
                mainWindow.EditRecord(recordId, name, image);
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
            if (editing && mainWindow.ContainsRecord(recordId))
            {
                mainWindow.DeleteRecord(recordId);
            }
            this.Close();
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            name = nameBox.Text;
        }

        bool VerifyRecord()
        {
            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Name isn't specified!");
                return false;
            }
            if (image == null || image.Length == 0)
            {
                MessageBox.Show("Image isn't specified!");
                return false;
            }
            return true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainWindow.CloseRecordPage(this);
        }
    }
}

using InfoServerDataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client = new Client();

        List<Record> Records = new List<Record>();
        ObservableCollection<RecordViewModel> ViewData = new ObservableCollection<RecordViewModel>();
        int curId = 0;

        public MainWindow()
        {
            InitializeComponent();
            recordsList.ItemsSource = ViewData;
        }

        public void ShowRecords(List<Record> records)
        {
            Records = records.ToList();
            InitializeViewData();
        }

        public void InitializeViewData()
        {
            ViewData.Clear();

            foreach (Record record in Records)
            {
                ViewData.Add(new RecordViewModel(record));
            }
            recordsList.Items.Refresh();
        }

        private void NewRecord_click(object sender, RoutedEventArgs e)
        {
            CreationPage creationPage = new CreationPage(this);
            creationPage.Show();
        }

        public bool ContainsRecord(int id)
        {
            if (Records.Any(record => record.Id == id))
                return true;
            return false;
        }

        public void DeleteRecord(int id)
        {
            var record = Records.First(r => r.Id == id);

            try
            {
                client.DeleteRecord(id);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);
            }
            finally
            {
                Records.Remove(record);
                ViewData.Remove(ViewData.First(r => r.Id == id));
                recordsList.Items.Refresh();
            }
        }

        public async void AddRecord(string name, byte[] image)
        {
            Record record = new Record(curId++, name, image);

            try
            {
                record.Id = await client.UploadRecord(record);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);                
            }
            finally
            {
                Records.Add(record);
                ViewData.Add(new RecordViewModel(record));
            }
        }

        public void EditRecord(int id, string name, byte[] image)
        {
            Record record = Records.First(r => r.Id == id);

            string _name = record.Name;
            byte[] _image = record.Image;

            record.Name = name;
            record.Image = image;

            try
            {
                client.UpdateRecord(record);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);

                record.Name = _name;
                record.Image = _image;
            }
            finally
            {
                recordsList.Items.Refresh();
            }
        }

        public void EditRecord(int id)
        {
            CreationPage creationPage = new CreationPage(this, Records.First(r => r.Id == id));
            creationPage.Show();
        }

        private void StackPanel_KeyDown(object sender, MouseButtonEventArgs e)
        {
            var item = (FrameworkElement)e.OriginalSource;
            RecordViewModel record = (RecordViewModel)item.DataContext;
            EditRecord(record.Id);
        }

        private async void loadData_Click(object sender, RoutedEventArgs e)
        {
            List<Record> records = new List<Record>();
            try
            {
                records = await client.GetRecords();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);
            }
            finally
            {
                ShowRecords(records);
            }
            
        }
    }
}

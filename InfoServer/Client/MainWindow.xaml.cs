using InfoServerDataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        List<RecordPage> openedRecords = new List<RecordPage>();

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
            RecordPage creationPage = new RecordPage(this);
            ShowRecord(creationPage);
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

                Records.Remove(record);
                ViewData.Remove(ViewData.First(r => r.Id == id));
                recordsList.Items.Refresh();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);
            }
        }

        public async void AddRecord(string name, byte[] image)
        {
            Record record = new Record(-1, name, image);

            try
            {
                record.Id = await client.UploadRecord(record);

                Records.Add(record);
                ViewData.Add(new RecordViewModel(record));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);
            }
        }

        public void EditRecord(int id, string name, byte[] image)
        {
            Record record = Records.First(r => r.Id == id);

            try
            {
                client.UpdateRecord(record);

                record.Name = name;
                record.Image = image;

                recordsList.Items.Refresh();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);
            }
        }

        public void EditRecord(int id)
        {
            RecordPage recordPage = new RecordPage(this, Records.First(r => r.Id == id));
            ShowRecord(recordPage);
        }

        public void ShowRecord(RecordPage recordPage)
        {
            if (openedRecords.Any(rp => rp.recordId == recordPage.recordId))
            {
                openedRecords.First(rp => rp.recordId == recordPage.recordId).Activate();
                return;
            }

            recordPage.Show();
            openedRecords.Add(recordPage);
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

                ShowRecords(records);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error occurred while connectiong to server.\n\r" + exception.Message);
            }
        }

        public void CloseRecordPage(RecordPage page)
        {
            openedRecords.Remove(page);
        }

        private void sortByName_Click(object sender, RoutedEventArgs e)
        {
            recordsList.Items.SortDescriptions.Clear();
            recordsList.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            recordsList.Items.Refresh();
        }
        private void sortByDefault_Click(object sender, RoutedEventArgs e)
        {
            recordsList.Items.SortDescriptions.Clear();
            recordsList.Items.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            recordsList.Items.Refresh();
        }
    }
}

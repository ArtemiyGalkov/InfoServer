using InfoServerDataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI
{
    class RecordViewModel : INotifyPropertyChanged
    {
        private Record record;

        public RecordViewModel(Record r)
        {
            record = r;
        }

        public int Id
        {
            get { return record.Id; }
            set
            {
                record.Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return record.Name; }
            set
            {
                record.Name = value;
                OnPropertyChanged("Name");
            }
        }
        public byte[] Image
        {
            get { return record.Image; }
            set
            {
                record.Image = value;
                OnPropertyChanged("Image");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

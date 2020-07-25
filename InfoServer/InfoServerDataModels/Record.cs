using System;
using System.Collections.Generic;
using System.Text;

namespace InfoServerDataModels
{
    public class Record
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }

        public Record(int id, string name, byte[] image)
        {
            Id = id;
            Name = name;
            Image = image;
        }
    }
}

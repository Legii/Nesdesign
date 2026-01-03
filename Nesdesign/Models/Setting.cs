using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesdesign
{

    public class Setting {
        public string Key { get; set; }
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string Description { get; set; }
        public Setting() { }
        public Setting(string key, string value) {
            Key = key;
            Value = value;
        }

    }


}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nesdesign.Models
{
    public class Part : INotifyPropertyChanged
    {
        private string _name;
        private bool sub = false;

        private bool _checked;
        public int? Quantity { get; set; }

        public Offer? Parent { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                    if(sub && Parent != null)
                        Parent.NewPartsText();
                }
            }
        }

        
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnPropertyChanged();
                    if (sub && Parent != null)
                        Parent.NewPartsText();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Part() { }
        public Part(string name, bool _checked, Offer parent)
        {
            this.Parent = parent;
            this.Name = name;
            this.Checked = _checked;
            sub = true;
        }
        public void Delete()
        {
          
            Parent.DeletePart(this);
        }


    }


}

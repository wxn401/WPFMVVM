using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WpfCore.ViewModel
{
    public class NetUnitViewModel : MvxViewModel
    {
        public ObservableCollection<string> list { get; set; }
        public NetUnitViewModel()
        {
            list = new ObservableCollection<string>();
        }
    }
}

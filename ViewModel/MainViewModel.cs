using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using WpfCore.Helper;

namespace WpfCore.ViewModel
{
    public class MainViewModel : MvxViewModel
    {

        public MainViewModel(IConfiguration Configuration, IServiceProvider ServiceProviderFactory, ITestService testService)
        {
            net = new ObservableCollection<NetUnitViewModel>();
            NetUnitViewModel model = new NetUnitViewModel();
            model.list.Add(testService.SayHello("kkk1111kkk"));
            net.Add(model);

            NetUnitViewModel model2 = new NetUnitViewModel();
            model2.list.Add("kkk2222kkk");
            net.Add(model2);

            var model3 = ServiceProviderFactory.GetRequiredService<NetUnitViewModel>();
            model3.list.Add(testService.SayHello("kkk3333333kkk"));
            net.Add(model3);

        }

        private string _bgColor = "";
        public string BgColor
        {
            get
            {
                return _bgColor;
            }
            set
            {
                _bgColor = value;
                RaisePropertyChanged(() => BgColor);
            }
        }

        public ObservableCollection<NetUnitViewModel> net { get; set; }

        private string _tText = "tttt";

        /// <summary>
        /// 绑定到界面上TextBox的Text属性上
        /// </summary>
        public string TTest
        {
            get
            {
                return _tText;
            }
            set
            {
                _tText = value;
                RaisePropertyChanged(() => TTest);
            }
        }

        private string _tText1 = "1111111111";

        /// <summary>
        /// 绑定到界面上TextBox的Text属性上
        /// </summary>
        public string TTest1
        {
            get
            {
                return _tText1;
            }
            set
            {
                _tText1 = value;
                RaisePropertyChanged(() => TTest1);
            }
        }

        //public ICommand ChangeColorCMD
        public ICommand ChangeColorCMD = new RelayCommand(() => {
            BgColor = "Green";
        });
    }
}

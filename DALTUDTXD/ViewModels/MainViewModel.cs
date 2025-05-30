using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome.Sharp;
using System.Windows.Input;
using DALTUDTXD.Models;
using DALTUDTXD.Repositories;

namespace DALTUDTXD.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private UserAccountModel _currentUserAccount;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        private IUserRepository userRepository;

        //Properties
        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }

            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }

            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get
            {
                return _icon;
            }

            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        //--> Commands
        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowCal1ViewCommand { get; }
        public ICommand ShowCal2ViewCommand { get; }
        public ICommand ShowCal3ViewCommand { get; }
        public ICommand ShowCal4ViewCommand { get; }
        public ICommand ShowCal5ViewCommand { get; }

        public MainViewModel()
        {
            // Khởi tạo command, mỗi khi Execute sẽ đổi CurrentChildView + Caption + Icon
            ShowHomeViewCommand = new ViewModelCommand(_ => ExecuteShowHomeView());
            ShowCal1ViewCommand = new ViewModelCommand(_ => ExecuteShowCal1View());
            ShowCal2ViewCommand = new ViewModelCommand(_ => ExecuteShowCal2View());
            ShowCal3ViewCommand = new ViewModelCommand(_ => ExecuteShowCal3View());
            ShowCal4ViewCommand = new ViewModelCommand(_ => ExecuteShowCal4View());
            ShowCal5ViewCommand = new ViewModelCommand(_ => ExecuteShowCal5View());

            // Khi khởi động, mặc định hiển thị Home
            ExecuteShowHomeView();
        }

        private void ExecuteShowHomeView()
        {
            CurrentChildView = new HomeViewModel();   
            Caption = "Trang chủ";
            Icon = IconChar.Home;
        }

        private void ExecuteShowCal1View()
        {
            CurrentChildView = new Page1ViewModel();
            Caption = "Thông số móng cơ bản";
            Icon = IconChar.Ruler;
        }

        private void ExecuteShowCal2View()
        {
            CurrentChildView = new Page2ViewModel();
            Caption = "Thông số cốt thép";
            Icon = IconChar.TrowelBricks;
        }

        private void ExecuteShowCal3View()
        {
            CurrentChildView = new Page3ViewModel();
            Caption = "Số liệu nền đất";
            Icon = IconChar.HouseFloodWater;
        }

        private void ExecuteShowCal4View()
        {
            CurrentChildView = new Page4ViewModel();
            Caption = "Tải trọng";
            Icon = IconChar.ArrowAltCircleDown;
        }

        private void ExecuteShowCal5View()
        {
            CurrentChildView = new Page5ViewModel();
            Caption = "Tính toán móng";
            Icon = IconChar.Calculator;
        }
    }
}

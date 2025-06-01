using DALTUDTXD.Models;
using DALTUDTXD.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;

namespace DALTUDTXD.ViewModels
{
    public class Page4ViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private ForceInputEntry _forceInput;
        private string _selectedElementName;

        public string SelectedElementName
        {
            get => _selectedElementName;
            set
            {
                _selectedElementName = value;
                OnPropertyChanged(nameof(SelectedElementName));
            }
        }

        public ForceInputEntry ForceInput
        {
            get => _forceInput;
            set
            {
                _forceInput = value;
                OnPropertyChanged(nameof(ForceInput));
            }
        }

        public ConstructionEntry SelectedMong
        {
            get => ForceInput.Mong;
            set
            {
                ForceInput.Mong = value;
                OnPropertyChanged(nameof(SelectedMong));
            }
        }
        // Danh sách móng từ Page2
        public ObservableCollection<ConstructionEntry> DanhSachMong => _mainViewModel.ConstructionList;

        public ICommand SaveCommand { get; }
        public ICommand ShowCalculatorViewCommand { get; }
        public ICommand ImportEtabsDataCommand { get; }

        public Page4ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _forceInput = new ForceInputEntry();
            SaveCommand = new RelayCommand(SaveData);
            ShowCalculatorViewCommand = new RelayCommand(ExecuteShowCalculatorView);
        }

        private void ExecuteShowCalculatorView()
        {
            _mainViewModel.ExecuteShowCal5View();
        }

        public ObservableCollection<ForceInputEntry> ForceInputList { get; set; } = new ObservableCollection<ForceInputEntry>();



        private void SaveData()
        {
            if (ForceInput.Mong == null)
            {
                MessageBox.Show("Vui lòng chọn móng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra giá trị nhập
            if (ForceInput.Moment == 0 && ForceInput.AxialForce == 0 && ForceInput.ShearForce == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một giá trị nội lực!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lưu dữ liệu vào danh sách
            var newForceInput = new ForceInputEntry
            {
                Mong = ForceInput.Mong,
                Moment = ForceInput.Moment,
                AxialForce = ForceInput.AxialForce,
                ShearForce = ForceInput.ShearForce
            };

            ForceInputList.Add(newForceInput);

            MessageBox.Show($"Đã lưu thành công:\nTên móng: {ForceInput.Mong.TenMong}\nMoment (M): {ForceInput.Moment}\nLực dọc (N): {ForceInput.AxialForce}\nLực cắt (Q): {ForceInput.ShearForce}",
                          "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset form
            ForceInput = new ForceInputEntry();
        }
    }
}

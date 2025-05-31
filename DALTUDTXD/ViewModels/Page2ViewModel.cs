using DALTUDTXD.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DALTUDTXD.ViewModels
{
    public class Page2ViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        // --- Dữ liệu công trình ---
        private ConstructionEntry _newConstructionEntry = new ConstructionEntry();
        public ConstructionEntry NewConstructionEntry
        {
            get => _newConstructionEntry;
            set { _newConstructionEntry = value; OnPropertyChanged(nameof(NewConstructionEntry)); }
        }

        // --- Selected để xóa ---
        private ConstructionEntry _selectedConstructionEntry;
        public ConstructionEntry SelectedConstructionEntry
        {
            get => _selectedConstructionEntry;
            set { _selectedConstructionEntry = value; OnPropertyChanged(nameof(SelectedConstructionEntry)); }
        }

        private FoundationEntry _selectedSoilLayer;
        public FoundationEntry SelectedSoilLayer
        {
            get => _selectedSoilLayer;
            set
            {
                _selectedSoilLayer = value;
                OnPropertyChanged(nameof(SelectedSoilLayer));
            }
        }

        // Cập nhật khi chọn tên móng
        public string SelectedFoundationName
        {
            get => NewConstructionEntry.TenMong;
            set
            {
                NewConstructionEntry.TenMong = value;
                OnPropertyChanged(nameof(SelectedFoundationName));

                // Tìm lớp đất tương ứng
                SelectedSoilLayer = FindSoilLayerByName(value);
            }
        }

        private FoundationEntry FindSoilLayerByName(string name)
        {
            foreach (var axis in _mainViewModel.Page1ViewModel.GeologicalAxes)
            {
                var layer = axis.Entries.FirstOrDefault(e => e.Vitrimong == name);
                if (layer != null) return layer;
            }
            return null;
        }

        // --- Danh sách tên móng từ Page1 ---
        public ObservableCollection<string> FoundationNames
        {
            get
            {
                var names = new ObservableCollection<string>();
                foreach (var axis in _mainViewModel.Page1ViewModel.GeologicalAxes)
                {
                    foreach (var entry in axis.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Vitrimong) && !names.Contains(entry.Vitrimong))
                        {
                            names.Add(entry.Vitrimong);
                        }
                    }
                }
                return names;
            }
        }

        // Danh sách cấp độ bê tông
        public List<string> DanhSachCapDoBeTong { get; } = new List<string>
        {
            "B12.5",
            "B15",
            "B20",
            "B25",
            "B30",
            "B35",
            "B40"
        };

        // Danh sách loại thép
        public List<string> DanhSachLoaiThep { get; } = new List<string>
        {
            "CB240-T",
            "CB300-V",
            "CB400-V",
            "CB500-V"
        };

        public ICommand AddConstructionCommand { get; }
        public ICommand DeleteConstructionCommand { get; }

        public Page2ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            AddConstructionCommand = new RelayCommand(AddConstructionEntry);
            DeleteConstructionCommand = new RelayCommand(DeleteSelectedConstructionEntry);

            // Thiết lập giá trị mặc định
            NewConstructionEntry.CapDoBeTong = DanhSachCapDoBeTong[2]; // B20
            NewConstructionEntry.LoaiThep = DanhSachLoaiThep[2]; // CB400-V
            NewConstructionEntry.ChieuDayLopBaoVe = 25; // 25mm
            NewConstructionEntry.BeDayTuong = 0.22; // 0.22m
            NewConstructionEntry.ChieuCaoDai = 0.6; // 0.6m
        }

        private void AddConstructionEntry()
        {
            if (ValidateEntry())
            {
                // Tìm tất cả lớp đất cho vị trí móng này
                var soilLayers = FindSoilLayersByName(NewConstructionEntry.TenMong);
                // Tìm lớp đất tại độ sâu chôn móng
                var soilLayerAtDepth = FindSoilLayerAtDepth(soilLayers, NewConstructionEntry.ChieuSauChonMong);

                ConstructionList.Add(new ConstructionEntry
                {
                    TenMong = NewConstructionEntry.TenMong,
                    ChieuSauChonMong = NewConstructionEntry.ChieuSauChonMong,
                    ChieuRongMong = NewConstructionEntry.ChieuRongMong,
                    CapDoBeTong = NewConstructionEntry.CapDoBeTong,
                    LoaiThep = NewConstructionEntry.LoaiThep,
                    ChieuDayLopBaoVe = NewConstructionEntry.ChieuDayLopBaoVe,
                    BeDayTuong = NewConstructionEntry.BeDayTuong,
                    ChieuCaoDai = NewConstructionEntry.ChieuCaoDai,
                    SoilLayer = soilLayerAtDepth // Lưu lớp đất tại độ sâu chôn móng
                });

                // Reset form nhưng giữ lại các giá trị mặc định
                var capDoBeTong = NewConstructionEntry.CapDoBeTong;
                var loaiThep = NewConstructionEntry.LoaiThep;
                var chieuDayBaoVe = NewConstructionEntry.ChieuDayLopBaoVe;
                var beDayTuong = NewConstructionEntry.BeDayTuong;
                var chieuCaoDai = NewConstructionEntry.ChieuCaoDai;

                NewConstructionEntry = new ConstructionEntry
                {
                    CapDoBeTong = capDoBeTong,
                    LoaiThep = loaiThep,
                    ChieuDayLopBaoVe = chieuDayBaoVe,
                    BeDayTuong = beDayTuong,
                    ChieuCaoDai = chieuCaoDai
                };
            }
        }
        // Tìm tất cả lớp đất cho một vị trí móng
        private List<FoundationEntry> FindSoilLayersByName(string name)
        {
            var layers = new List<FoundationEntry>();
            foreach (var axis in _mainViewModel.Page1ViewModel.GeologicalAxes)
            {
                foreach (var entry in axis.Entries)
                {
                    if (entry.Vitrimong == name)
                    {
                        layers.Add(entry);
                    }
                }
            }
            // Sắp xếp theo thứ tự lớp
            return layers.OrderBy(l => l.Sothutulopdat).ToList();
        }

        // Tìm lớp đất tại độ sâu hm (chiều sâu chôn móng)
        private FoundationEntry FindSoilLayerAtDepth(List<FoundationEntry> layers, double depth)
        {
            double cumulativeDepth = 0;
            foreach (var layer in layers)
            {
                cumulativeDepth += layer.Chieudaylopdat;
                if (depth <= cumulativeDepth)
                {
                    return layer;
                }
            }
            // Nếu độ sâu vượt quá tổng chiều dày các lớp thì trả về lớp cuối cùng
            return layers.LastOrDefault();
        }
        private bool ValidateEntry()
        {
            if (string.IsNullOrWhiteSpace(NewConstructionEntry.TenMong))
            {
                System.Windows.MessageBox.Show("Vui lòng chọn tên móng!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (NewConstructionEntry.ChieuSauChonMong <= 0)
            {
                System.Windows.MessageBox.Show("Chiều sâu chôn móng phải lớn hơn 0!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (NewConstructionEntry.ChieuRongMong <= 0)
            {
                System.Windows.MessageBox.Show("Chiều rộng móng phải lớn hơn 0!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (NewConstructionEntry.ChieuDayLopBaoVe <= 0)
            {
                System.Windows.MessageBox.Show("Chiều dày lớp bảo vệ phải lớn hơn 0!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (NewConstructionEntry.BeDayTuong <= 0)
            {
                System.Windows.MessageBox.Show("Bề dày tường phải lớn hơn 0!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            if (NewConstructionEntry.ChieuCaoDai <= 0)
            {
                System.Windows.MessageBox.Show("Chiều cao đài phải lớn hơn 0!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void DeleteSelectedConstructionEntry()
        {
            if (SelectedConstructionEntry != null)
            {
                ConstructionList.Remove(SelectedConstructionEntry);
            }
        }

        public ObservableCollection<ConstructionEntry> ConstructionList => _mainViewModel.ConstructionList;
    }
}

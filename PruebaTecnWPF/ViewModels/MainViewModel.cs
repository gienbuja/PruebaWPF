using Microsoft.EntityFrameworkCore;
using PruebaTecnWPF.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PruebaTecnWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Product _selectedProduct;
        private string _name;
        private string _description;
        private decimal _price;
        private int _stock;

        public ObservableCollection<Product> Products { get; set; } = new();

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }
        public int Stock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(); }
        }


        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if (value != null)
                {
                    Name = value.Name;
                    Description = value.Description;
                    Price = value.Price;
                    Stock = value.Stock;
                }
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            AddCommand = new RelayCommand(AddProduct);
            UpdateCommand = new RelayCommand(UpdateProduct, () => SelectedProduct != null);
            DeleteCommand = new RelayCommand(() => DeleteProduct(SelectedProduct));

            LoadProduct();
            defaultValue();
        }

        private void LoadProduct()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            Products.Clear();
            foreach (var product in db.Products)
                Products.Add(product);
        }

        private void AddProduct()
        {
            if (!validateData()) return;
            using var db = new AppDbContext();
            var product = new Product {
                Name = Name, 
                Description = Description,
                Price = Price,
                Stock = Stock
            };
            db.Products.Add(product);
            db.SaveChanges();
            Products.Add(product);
            defaultValue();
        }

        private void UpdateProduct()
        {
            if (!validateData()) return;
            using var db = new AppDbContext();
            var product = db.Products.Find(SelectedProduct.Id);
            if (product != null)
            {
                product.Name = Name;
                product.Description = Description;
                product.Price = Price;
                product.Stock = Stock;
                db.Products.Update(product);
                db.SaveChanges();
                var index = Products.IndexOf(SelectedProduct);
                if (index >= 0)
                {
                    Products[index] = product;
                    SelectedProduct = product;
                }
                defaultValue();

                OnPropertyChanged(nameof(Product));
            }
        }

        private void DeleteProduct(Product product)
        {
            if (product == null) return;
            using var db = new AppDbContext();
            var prod = db.Products.Find(product.Id);
            if (prod != null)
            {
                db.Products.Remove(prod);
                db.SaveChanges();
                Products.Remove(product);
                defaultValue();
            }
        }

        private void defaultValue()
        {
            Name = string.Empty;
            Description = string.Empty;
            Price = 0;
            Stock = 0;
            SelectedProduct = null;
        }

        private Boolean validateData()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("El nombre no puede ir vacío.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                MessageBox.Show("La descripción no puede ir vacía.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (Price < 0)
            {
                MessageBox.Show("El precio no puede ser negativo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (Stock < 0)
            {
                MessageBox.Show("El stock no puede ser negativo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
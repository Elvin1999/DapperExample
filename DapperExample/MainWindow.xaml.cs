using Dapper;
using DapperExample.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DapperExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Product> GetAll()
        {
            List<Product> products = new List<Product>();
            using (var connection=new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString))
            {
                connection.Open();
                products = connection.Query<Product>("SELECT Id, Name, Price from Products").ToList();
               
            }
            return products;
        }
        private Product GetById(int id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString))
            {
                connection.Open();
                var product=connection.QueryFirstOrDefault("SELECT * FROM Products where Id=@PId"
                    , new { PId = id });

                return new Product {
                Id=product.Id,
                Name=product.Name,
                Price=product.Price
                };
            }
        }
        private void Update(Product product)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString))
            {
                connection.Open();
                connection.Execute("UPDATE Products SET Name=@PName,Price=@PPrice WHERE Id=@PId",
                    new { PId = product.Id, PName = product.Name, PPrice = product.Price });
            }
        }
        private void InsertProduct(Product product)
        {
            using (var connection= new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString))
            {
                connection.Open();
                connection.Execute("INSERT INTO Products(Name,Price) VALUES(@ProductName,@ProductPrice)"
                    , new { ProductName = product.Name, ProductPrice = product.Price });

            }
        }
        private void Delete(int id)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString))
            {
                connection.Open();
                connection.Execute("DELETE FROM Products WHERE Id=@PId", new { PId = id });
                MessageBox.Show("Product Deleted Successfully");
            }
        }
        private void CallSP()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString))
            {
                DynamicParameters parameter = new DynamicParameters();

                connection.Open();

                parameter.Add("@P_Price", DbType.Decimal);
                var data=connection.Query<Product>("ShowGreaterThan",
    parameter,
    commandType: CommandType.StoredProcedure);

                mydatagrid.ItemsSource = data.ToList();

            }
        }
        public MainWindow()
        {
            InitializeComponent();
            //InsertProduct(new Product { Name = "ASUS Rog", Price = 4300 });
          //  mydatagrid.ItemsSource = GetAll();

            //var product = GetById(1);
            //product.Name = "ACER PREDATOR";
            //product.Price = 10001;
            //Update(product);
            //Delete(1);
            CallSP();
//            mydatagrid.ItemsSource = GetAll();
        }
    }
}

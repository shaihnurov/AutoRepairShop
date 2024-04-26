using Diplom1.MVVM.Model.Cars;
using Diplom1.MVVM.Model.Interface;
using Diplom1.MVVM.Model.WorkShop;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace Diplom1.Repository
{
    public class SelectionSparesRepository : RepositoryBase, ISelectionSparesRepository
    {
        public ObservableCollection<WorkShopSparesModel> GetAvailableSpares(string nameModel, string make)
        {
            ObservableCollection<WorkShopSparesModel> availableSpares = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = @"SELECT s.*, w.AmountSpares, w.WorkShopId FROM dbo.CarModelArticul c JOIN dbo.WorkShopSpares w ON c.Articul = w.Articul JOIN dbo.Spares s ON w.SparesId = s.Id WHERE c.NameModel = @NameModel AND c.Make = @Make";
                command.Parameters.AddWithValue("@NameModel", nameModel);
                command.Parameters.AddWithValue("@Make", make);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var amountSpares = Convert.ToInt32(reader["AmountSpares"]);
                    if (amountSpares != 0)
                    {
                        var sparesModel = new WorkShopSparesModel
                        {
                            Id = reader["Id"].ToString(),
                            WorkShopId = reader["WorkShopId"].ToString(),
                            Name = reader["Name"].ToString(),
                            Image = (byte[])reader["Image"],
                            Articul = reader["Articul"].ToString(),
                            Make = reader["Make"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Amount = reader["AmountSpares"].ToString()
                        };
                        availableSpares.Add(sparesModel);
                    }
                }
            }

            return availableSpares;
        }
        public ObservableCollection<SparesModel> GetSparesShop(string nameModel, string make)
        {
            ObservableCollection<SparesModel> availableSpares = [];

            using (var connection = GetConnection())
            {
                connection.Open();
                using var command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = @"SELECT s.* FROM dbo.CarModelArticul c JOIN dbo.Spares s ON c.Articul = s.Articul WHERE c.NameModel = @NameModel AND c.Make = @Make AND NOT EXISTS (SELECT 1 FROM dbo.WorkShopSpares w WHERE w.Articul = s.Articul)";
                command.Parameters.AddWithValue("@NameModel", nameModel);
                command.Parameters.AddWithValue("@Make", make);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var sparesModel = new SparesModel
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Image = (byte[])reader["Image"],
                        Articul = reader["Articul"].ToString(),
                        Make = reader["Make"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Amount = reader["Amount"].ToString()
                    };
                    availableSpares.Add(sparesModel);
                }
            }

            return availableSpares;
        }
    }
}
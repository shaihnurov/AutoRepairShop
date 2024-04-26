using Diplom1.MVVM.Model.Cars;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Diplom1.MVVM.Model.Interface;
using System;

namespace Diplom1.Repository
{
    public class CarsRepository : RepositoryBase, ICarsRepository
    {
        public ObservableCollection<Car> GetCars()
        {
            ObservableCollection<Car> car = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT Name, Image FROM dbo.[Car]";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Car carsModel = new()
                    {
                        Name = reader["Name"].ToString(),
                        Image = (byte[])reader["Image"]
                    };
                    car.Add(carsModel);
                }
            }
            return car;
        }
        public ObservableCollection<CarsModel> GetCarsModel(string selectedMake)
        {
            ObservableCollection<CarsModel> car = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT Id, NameModel, Make, Image FROM dbo.[CarsModel] WHERE Make = @selectedMake";
                command.Parameters.AddWithValue("@selectedMake", selectedMake);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CarsModel carsModel = new CarsModel();
                    carsModel.Id = reader["Id"].ToString();
                    carsModel.NameModel = reader["NameModel"].ToString();
                    carsModel.Make = reader["Make"].ToString();

                    if (reader["Image"] != DBNull.Value)
                    {
                        carsModel.Image = (byte[])reader["Image"];
                    }
                    else
                    {
                        carsModel.Image = null;
                    }

                    car.Add(carsModel);
                }
            }
            return car;
        }
    }
}

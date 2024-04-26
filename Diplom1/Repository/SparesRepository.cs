using System.Data.SqlClient;
using Diplom1.MVVM.Model.Cars;
using System.Collections.ObjectModel;
using System;
using Diplom1.MVVM.Model.Interface;

namespace Diplom1.Repository
{
    public class SparesRepository : RepositoryBase, ISparesRepository
    {
        public ObservableCollection<SparesModel> GetSparesByCategory(string nameModel, string make)
        {
            ObservableCollection<SparesModel> spares = [];

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Articul FROM dbo.CarModelArticul WHERE NameModel = @nameModel AND Make = @make";
                command.Parameters.AddWithValue("@nameModel", nameModel);
                command.Parameters.AddWithValue("@make", make);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string articul = reader["Articul"].ToString();
                    var sparesWithArticul = GetSparesByArticul(articul);
                    foreach (var item in sparesWithArticul)
                    {
                        spares.Add(item);
                    }
                }
            }

            return spares;
        }
        public ObservableCollection<SparesModel> GetSparesByArticul(string articul)
        {
            ObservableCollection<SparesModel> spares = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, Name, Image, Articul, Make, Amount, Price FROM dbo.Spares WHERE Articul = @articul";
                command.Parameters.AddWithValue("@articul", articul);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    SparesModel spare = new()
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Image = (byte[])reader["Image"],
                        Articul = reader["Articul"].ToString(),
                        Make = reader["Make"].ToString(),
                        Amount = reader["Amount"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                    spares.Add(spare);
                }
            }
            return spares;
        }
        public int DecreaseSparesAmount(string sparesId)
        {
            int updatedAmount = 0;

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Amount FROM dbo.Spares WHERE Id = @sparesId";
                command.Parameters.AddWithValue("@sparesId", sparesId);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    int currentAmount = Convert.ToInt32(result);

                    if (currentAmount > 0)
                    {
                        command.CommandText = "UPDATE dbo.Spares SET Amount = Amount - 1 WHERE Id = @sparesId";
                        command.ExecuteNonQuery();

                        updatedAmount = currentAmount - 1;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Товар закончился на складе. \t\nНовая партия поступит в течение 5-10 минут");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Товар не найден.");
                }
            }

            return updatedAmount;
        }
        public void IncreaseSparesAmount(string sparesId)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT Amount FROM dbo.Spares WHERE Id = @sparesId";
            command.Parameters.AddWithValue("@sparesId", sparesId);
            int currentAmount = Convert.ToInt32(command.ExecuteScalar());

            if (currentAmount == 0)
            {
                command.CommandText = "UPDATE dbo.Spares SET Amount = Amount + 10 WHERE Id = @sparesId";
                command.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Не удалось пополнить товар на складе");
            }
        }
    }
}

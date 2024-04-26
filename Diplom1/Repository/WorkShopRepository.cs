using Diplom1.MVVM.Model.WorkShop;
using System;
using System.Data.SqlClient;
using Diplom1.MVVM.Model.Interface;

namespace Diplom1.Repository
{
    public class WorkShopRepository : RepositoryBase, IWorkShopRepository
    {
        public WorkShopModel GetByShopInfo()
        {
            WorkShopModel shop = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, Name, Balance, Image, Adress FROM [WorkShop]";
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    shop = new WorkShopModel()
                    {
                        Id = reader[0].ToString(),
                        Name = reader["Name"].ToString(),
                        Balance = Convert.ToDecimal(reader["Balance"]),
                        Image = (byte[])reader["Image"],
                        Adress = reader["Adress"].ToString()
                    };
                }
            }
            return shop;
        }
        public void UpdateWorkShopImage(string workShopId, byte[] imageBytes)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand("UPDATE dbo.WorkShop SET Image = @Image WHERE Id = @Id", connection);
            connection.Open();
            command.Parameters.AddWithValue("@Id", workShopId);
            command.Parameters.AddWithValue("@Image", imageBytes);

            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
            {
                Console.WriteLine("No rows affected. Check if the ID exists in the database.");
            }
        }
        public void UpdateWorkShopAdress(string workShopId, string adress)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand("UPDATE dbo.WorkShop SET Adress = @Adress WHERE Id = @Id", connection);
            connection.Open();
            command.Parameters.AddWithValue("@Id", workShopId);
            command.Parameters.AddWithValue("@Adress", adress);

            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
            {
                Console.WriteLine("No rows affected. Check if the ID exists in the database.");
            }
        }
        public void UpdateWorkShopName(string workShopId, string name)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand("UPDATE dbo.WorkShop SET Name = @Name WHERE Id = @Id", connection);
            connection.Open();
            command.Parameters.AddWithValue("@Id", workShopId);
            command.Parameters.AddWithValue("@Name", name);

            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows == 0)
            {
                Console.WriteLine("No rows affected. Check if the ID exists in the database.");
            }
        }
        public void DecreaseBalance(string workShopId, decimal priceSpares)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT Balance FROM dbo.WorkShop WHERE Id = @workShopId";
            command.Parameters.AddWithValue("@workShopId", workShopId);
            int currentBalance = Convert.ToInt32(command.ExecuteScalar());

            if (currentBalance >= priceSpares)
            {
                command.CommandText = "UPDATE dbo.WorkShop SET Balance = Balance - @priceSpares WHERE Id = @workShopId";
                command.Parameters.AddWithValue("@priceSpares", priceSpares);
                command.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Недостаточно средств");
            }
        }
        public void IncreaseBalance(string workShopId, decimal priceSpares)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT Balance FROM dbo.WorkShop WHERE Id = @workShopId";
            command.Parameters.AddWithValue("@workShopId", workShopId);
            int currentBalance = Convert.ToInt32(command.ExecuteScalar());

            if (currentBalance >= priceSpares)
            {
                command.CommandText = "UPDATE dbo.WorkShop SET Balance = Balance + @priceSpares WHERE Id = @workShopId";
                command.Parameters.AddWithValue("@priceSpares", priceSpares);
                command.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Ошибка в пополнении баланса");
            }
        }
    }
}

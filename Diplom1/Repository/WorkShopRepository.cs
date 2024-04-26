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
                command.CommandText = "select *from [WorkShop]";
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    shop = new WorkShopModel()
                    {
                        Id = reader[0].ToString(),
                        Name = reader[1].ToString(),
                        Balance = Convert.ToDecimal(reader[2]),
                    };
                }
            }
            return shop;
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
    }
}

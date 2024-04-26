using Diplom1.MVVM.Model;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using Diplom1.MVVM.Model.Interface;

namespace Diplom1.Repository
{
    public class HistoryPayRepository : RepositoryBase, IHistoryPayRepository
    {
        public void AddHistoryPay(HistoryPayModel historyModel)
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [HistoryPay] (Name, Amount, DateTime, WorkShopId, WorkShopName, SparesId, Price) VALUES (@Name, @Amount, @DateTime, @WorkShopId, @WorkShopName, @SparesId, @Price)";
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = historyModel.Name;
                command.Parameters.Add("@Amount", SqlDbType.NVarChar).Value = historyModel.Amount;
                command.Parameters.Add("@DateTime", SqlDbType.NVarChar).Value = historyModel.DateTime;
                command.Parameters.Add("@WorkShopId", SqlDbType.NVarChar).Value = historyModel.WorkShopId;
                command.Parameters.Add("@WorkShopName", SqlDbType.NVarChar).Value = historyModel.WorkShopName;
                command.Parameters.Add("@SparesId", SqlDbType.NVarChar).Value = historyModel.SparesId;
                command.Parameters.Add("@Price", SqlDbType.Decimal).Value = historyModel.Price;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"* Ошибка при составлении истории: {ex.Message}");
            }
        }
        public ObservableCollection<HistoryPayModel> GetAllHistory(string workShopId)
        {
            ObservableCollection<HistoryPayModel> history = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT Id, Name, Amount, DateTime, WorkShopId, WorkShopName, SparesId, Price FROM dbo.[HistoryPay] WHERE WorkShopId = @workShopId";
                command.Parameters.AddWithValue("@workShopId", workShopId);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    HistoryPayModel historyModel = new()
                    {
                        Id = reader["Id"].ToString(),
                        Name = reader["Name"].ToString(),
                        Amount = reader["Amount"].ToString(),
                        DateTime = Convert.ToDateTime(reader["DateTime"]),
                        WorkShopId = reader["WorkShopId"].ToString(),
                        WorkShopName = reader["WorkShopName"].ToString(),
                        SparesId = reader["SparesId"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                    history.Add(historyModel);
                }
            }
            return history;
        }
    }
}

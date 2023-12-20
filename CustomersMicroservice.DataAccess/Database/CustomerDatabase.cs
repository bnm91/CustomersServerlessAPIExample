using Microsoft.Data.Sqlite;

namespace CustomersMicroservice.DataAccess.Database
{
    public class CustomerDatabase
    {
        private SqliteConnection? _connection;

        // NOTE:
        //      This is for this in-memory toy example
        //      Creating a singleton connection would be a very bad idea in most cases
        //      In this instance it allows us to keep an in memory db around and accessible
        //          by multiple calls to multiple azure functions
        public SqliteConnection GetOpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("Data Source=file::memory:?cache=shared");
                _connection.Open();
                ConstructDb();
            }
            return _connection;
        }

        public void CloseConnection()
        {
            if(_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        private void ConstructDb()
        {
            var conn = GetOpenConnection();
            var createCustomersTableSql = "CREATE TABLE Customers (CustomerId text, FullName text, DateOfBirth text);";

            SqliteCommand cmd = new SqliteCommand(createCustomersTableSql, conn);
            cmd.ExecuteNonQuery();
        }
    }
}

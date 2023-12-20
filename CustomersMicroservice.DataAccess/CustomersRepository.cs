using CustomersMicroservice.DataAccess.Database;
using CustomersMicroservice.Models;

namespace CustomersMicroservice.DataAccess
{
    public class CustomersRepository : ICustomersRepository
    {
        private CustomerDatabase _db { get; set; }
        public CustomersRepository(CustomerDatabase db)
        {
            _db = db;
        }

        public Customer? GetById(Guid id)
        {
            var sql = $"SELECT customerId, FullName, DateOfBirth FROM Customers WHERE CustomerId = $id";

            var cmd = _db.GetOpenConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$id", id.ToString());

            var reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                try
                {
                    var customerId = Guid.Parse((string)reader.GetValue(0));
                    var fullName = (string)reader.GetValue(1);
                    var dateOfBirth = DateOnly.Parse((string)reader.GetValue(2));
                    return new Customer()
                    {
                        CustomerId = customerId,
                        FullName = fullName,
                        DateOfBirth = dateOfBirth,
                    };
                }
                catch
                {
                    throw new Exception("Error parsing data from database during Repository.GetAll");
                }
            }
            return null;
        }

        public List<Customer> GetByBirthdate(DateOnly? minDate = null, DateOnly? maxDate = null)
        {
            minDate ??= DateOnly.MinValue;
            maxDate ??= DateOnly.MaxValue;

            var minDateSqlParam = minDate.Value.ToString("yyyy-MM-dd");
            var maxDateSqlParam = maxDate.Value.ToString("yyyy-MM-dd");

            var sql = $"SELECT customerId, FullName, DateOfBirth FROM Customers WHERE DateOfBirth >= $minDateSqlParam and DateOfBirth <= $maxDateSqlParam";

            var cmd = _db.GetOpenConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$minDateSqlParam", minDateSqlParam);
            cmd.Parameters.AddWithValue("$maxDateSqlParam", maxDateSqlParam);

            var reader = cmd.ExecuteReader();

            var result = new List<Customer>();
            while (reader.Read())
            {
                try
                {
                    var customerId = Guid.Parse((string)reader.GetValue(0));
                    var fullName = (string)reader.GetValue(1);
                    var dateOfBirth = DateOnly.Parse((string)reader.GetValue(2));
                    result.Add(new Customer()
                    {
                        CustomerId = customerId,
                        FullName = fullName,
                        DateOfBirth = dateOfBirth,
                    });
                }
                catch
                {
                    throw new Exception("Error parsing data from database during Repository.GetAll");
                }
            }
            return result;
        }

        public IEnumerable<Customer> GetAll()
        {
            var customers = new List<Customer>();
            var sql = $"SELECT customerId, FullName, DateOfBirth FROM Customers";

            var cmd = _db.GetOpenConnection().CreateCommand();
            cmd.CommandText = sql;

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    var id = Guid.Parse((string)reader.GetValue(0));
                    var fullName = (string)reader.GetValue(1);
                    var dateOfBirth = DateOnly.Parse((string)reader.GetValue(2));
                    customers.Add(new Customer()
                    {
                        CustomerId = id,
                        FullName = fullName,
                        DateOfBirth = dateOfBirth,
                    });
                }
                catch 
                {
                    throw new Exception("Error parsing data from database during Repository.GetAll");
                }
            }
            return customers;
        }

        public void Add(Customer customer) 
        {
            var sql = $"INSERT INTO Customers (CustomerId, FullName, DateOfBirth) VALUES ($cusId, $fName, $birthday)";

            var cmd = _db.GetOpenConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$cusId", customer.CustomerId.ToString());
            cmd.Parameters.AddWithValue("$fName", customer.FullName);
            cmd.Parameters.AddWithValue("$birthday", customer.DateOfBirth.ToString("yyyy-MM-dd"));

            cmd.ExecuteNonQuery();
        }

        public void Delete(Guid id)
        {
            var sql = $"DELETE FROM Customers WHERE CustomerId = $id";

            var cmd = _db.GetOpenConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("id", id.ToString());

            cmd.ExecuteNonQuery();
        }

        public void Update(Guid id, Customer customer) 
        {
            var sql = $"UPDATE Customers SET CustomerId = $cusId, FullName = $fName, DateOfBirth = $birthdate WHERE CustomerId = $id";

            var cmd = _db.GetOpenConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$id", id.ToString());
            cmd.Parameters.AddWithValue("$cusId", customer.CustomerId.ToString());
            cmd.Parameters.AddWithValue("$fName", customer.FullName);
            cmd.Parameters.AddWithValue("$birthdate", customer.DateOfBirth.ToString("yyyy-MM-dd"));

            cmd.ExecuteNonQuery();
        }

    }
}

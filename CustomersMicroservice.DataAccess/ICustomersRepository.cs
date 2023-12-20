using CustomersMicroservice.Models;

namespace CustomersMicroservice.DataAccess
{
    public interface ICustomersRepository
    {
        IEnumerable<Customer> GetAll();
        Customer? GetById(Guid id);
        void Add(Customer customer);
        void Delete(Guid id);
        void Update(Guid id, Customer customer);
        List<Customer> GetByBirthdate(DateOnly? minDate = null, DateOnly? maxDate = null);
    }
}
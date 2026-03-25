using System.Numerics;

namespace PortfolioScheduler.Domain.Models
{
    public class Customer
    {
        public long Id { get; }
        public string Name { get; private set; }
        public string Cpf { get; private set; }
        public string Email { get; private set; }
        public decimal MonthlyAmount { get; private set; }
        public bool Active { get; private set; }
        public DateTime JoiningDate { get; }

        protected Customer() { }
        public Customer(string name, string cpf, string email, decimal monthlyAmount)
        {
            Name = name;
            Cpf = cpf;
            Email = email;
            MonthlyAmount = monthlyAmount;
            Active = true;
            JoiningDate = DateTime.Now;
        }
    }
}

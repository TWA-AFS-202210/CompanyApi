using Microsoft.Extensions.Hosting;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
        }

        public string? CompanyId { get; set; }
        public string Name { get; private set; }
    }
}


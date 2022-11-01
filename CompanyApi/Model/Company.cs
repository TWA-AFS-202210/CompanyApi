using Microsoft.Extensions.Hosting;
using System.Drawing;
using System;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
        }

        public string? CompanyId { get; set; }
        public string Name { get; set; }

        public override bool Equals(object? obj)
        {
            var company = obj as Company;
            return company != null &&
                   company.Name == Name;
        }
    }
}


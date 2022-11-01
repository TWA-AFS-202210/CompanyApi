using Microsoft.Extensions.Hosting;
using System.Drawing;
using System;
using System.Collections.Generic;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            Employee = new List<Employee>();
        }

        public string? CompanyId { get; set; }
        public string Name { get; set; }
        public List<Employee> Employee { get; set; }

        public override bool Equals(object? obj)
        {
            var company = obj as Company;
            return company != null &&
                   company.Name == Name;
        }
    }
}


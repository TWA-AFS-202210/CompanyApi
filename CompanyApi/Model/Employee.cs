using Microsoft.Extensions.Hosting;
using System.Drawing;
using System;

namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee(string name, int salary)
        {
            Name = name;
            EmployeeId = Guid.NewGuid().ToString();
        }

        public string? EmployeeId { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }

        public override bool Equals(object? obj)
        {
            var employee = obj as Employee;
            return employee != null &&
                   employee.Name == Name && 
                   employee.Salary == Salary;
        }
    }
}


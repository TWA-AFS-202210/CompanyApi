using System;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            Name = name;
            ID = string.Empty;
        }

        public string Name { get; set; }
        public string ID { get; set; }
    }
}
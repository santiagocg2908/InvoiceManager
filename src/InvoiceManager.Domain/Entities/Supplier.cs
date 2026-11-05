using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceManager.Domain.Entities
{
    public class Supplier
    {
        private Supplier() 
        { 
        }
        public Supplier(string name)
        {
            SetName(name);
        }
        public int Id {get; private set;}
        public string Name {get; private set;}
        public string NormalizedName { get; private set;}
        public ICollection<Invoice> Invoices {get; private set;} = new List<Invoice>();
        public void ChangeName(string name)
        {
            SetName(name);
        }
        private void SetName(string name) 
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("The Supplier name is required",
                    nameof(name));
            }
            if (name.Length > 200)
            {
                throw new ArgumentException("The Supplier name can not exceed 200 characters",
                    nameof(name));
            }

            Name = string.Join("", name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
            NormalizedName = Name.ToUpperInvariant();
        }
    }
}

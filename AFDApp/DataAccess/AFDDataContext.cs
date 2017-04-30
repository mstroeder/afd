using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AFDApp
{
    public class AFDDataContext : DbContext
    {
        public AFDDataContext() : base("AFDConnection")
        {
        }
        public DbSet<CustomerData> Customers { get; set; }
    }
}
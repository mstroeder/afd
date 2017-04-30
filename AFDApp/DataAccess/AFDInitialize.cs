using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AFDApp
{
    public class AFDDbInitialize : CreateDatabaseIfNotExists<AFDDataContext>
    {
    }
}
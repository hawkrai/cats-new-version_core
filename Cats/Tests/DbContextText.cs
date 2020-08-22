using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading;
using LMP.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class DbContextText
    {
        public static void Main(string[] args)
        {
            using var dbContext = new LmPlatformModelsContext();

            var dbSets = dbContext.GetType().GetProperties().Where(prop => prop.PropertyType.Name.Contains("DbSet"));

            var totalSets = dbSets.Count();

            var currentSets = 0;

            var sets = dbSets
                .Select(propertyInfo =>propertyInfo.GetValue(dbContext));
        }
    }

}

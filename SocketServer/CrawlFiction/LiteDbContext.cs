using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlFiction
{
    public class LiteDbContext : IDisposable
    {
        private static LiteDatabase _database;
        private static readonly object _lock = new object();
        private static bool _disposed = false;

        public LiteDbContext(string databasePath)
        {
            if (_database == null)
            {
                lock (_lock)
                {
                    if (_database == null)
                    {
                        _database = new LiteDatabase(databasePath);
                    }
                }
            }
        }

        public LiteCollection<Customer> Customers => (LiteCollection<Customer>)_database.GetCollection<Customer>("customers");

        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_lock)
                {
                    if (!_disposed)
                    {
                        _database.Dispose();
                        _disposed = true;
                    }
                }
            }
        }
    }

}

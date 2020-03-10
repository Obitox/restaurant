using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.UnitOfWork;

namespace Restaurant.Infrastructure.Services
{
    public class HomeService
    {
        private readonly IUnitOfWork _uow;

        public HomeService(IUnitOfWork unit)
        {
            _uow = unit;
        }

        public void GetItems()
        {
            string sql = "SELECT * FROM items";

            _uow.GetRepository<Item>().Query(sql);
            _uow.Commit();
        }
    }
}

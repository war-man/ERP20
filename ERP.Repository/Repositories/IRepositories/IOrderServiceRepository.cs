﻿using ERP.Common.GenericRepository;
using ERP.Common.Models;
using ERP.Data.ModelsERP;
using ERP.Data.ModelsERP.ModelView.OrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Repository.Repositories.IRepositories
{
    public interface IOrderServiceRepository : IGenericRepository<order_service>
    {
        PagedResults<order_service> CreatePagedResults(int pageNumber, int pageSize);
        PagedResults<orderseviceviewmodel> GetAllSearch(int pageNumber, int pageSize, DateTime? start_date, DateTime? end_date, string search_name);
    }
}

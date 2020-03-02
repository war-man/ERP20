﻿using ERP.Common.GenericRepository;
using ERP.Common.Models;
using ERP.Data.DbContext;
using ERP.Repository.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Data.ModelsERP;
using ERP.Data.ModelsERP.ModelView.Transaction;
using AutoMapper;
using ERP.Common.Constants.Enums;
using ERP.Data.ModelsERP.ModelView.Customer;
using ERP.Data.ModelsERP.ModelView;

namespace ERP.Repository.Repositories
{
    public class TransactionRepository : GenericRepository<transaction>, ITransactionRepository
    {
        private readonly IMapper _mapper;
        public TransactionRepository(ERPDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            this._mapper = mapper;
        }
        public PagedResults<transaction> CreatePagedResults(int pageNumber, int pageSize)
        {
            var skipAmount = pageSize * pageNumber;

            var list = _dbContext.transactions.OrderBy(t => t.tra_id).Skip(skipAmount).Take(pageSize);

            var totalNumberOfRecords = _dbContext.transactions.Count();

            var results = list.ToList();

            var mod = totalNumberOfRecords % pageSize;

            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            return new PagedResults<transaction>
            {
                Results = results,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords
            };
        }
        
        public List<dropdown> GetTransactionType()
        {
            List<dropdown> res = new List<dropdown>();
            for( int i = 1; i< 5; i++)
            {
                dropdown dr = new dropdown();
                dr.id = i;
                dr.name = EnumTransaction.tra_type[i-1];
                res.Add(dr);
            }
            return res;
        }
        public List<dropdown> GetTransactionPriority()
        {
            List<dropdown> res = new List<dropdown>();
            for (int i = 1; i < 5; i++)
            {
                dropdown dr = new dropdown();
                dr.id = i;
                dr.name = EnumTransaction.tra_priority[i - 1];
                res.Add(dr);
            }
            return res;
        }
        public  List<dropdown> GetTransactionStatus()
        {
            List<dropdown> res = new List<dropdown>();
            for (int i = 1; i < 4; i++)
            {
                dropdown dr = new dropdown();
                dr.id = i;
                dr.name = EnumTransaction.tra_status[i - 1];
                res.Add(dr);
            }
            return res;
        }
        public PagedResults<transactionviewmodel> GetAllPageSearch(int pageNumber, int pageSize, string search_name)
        {
            List<transactionviewmodel> res = new List<transactionviewmodel>();

            var skipAmount = pageSize * pageNumber;

            var list = _dbContext.transactions.OrderBy(t => t.tra_id).Skip(skipAmount).Take(pageSize);

            var totalNumberOfRecords = _dbContext.transactions.Count();

            var results = list.ToList();
            foreach (transaction i in results)
            {
               
                var transactionview = _mapper.Map<transactionviewmodel>(i);
                //Bat theo Enums
                for (int j = 1; j < 5; j++)
                {
                    if (j == i.tra_type)
                    {
                        transactionview.tra_type_name = EnumTransaction.tra_type[j-1];
                    }
                }
                for (int j = 1; j < 5; j++)
                {
                    if (j == i.tra_priority)
                    {
                        transactionview.tra_priority_name = EnumTransaction.tra_priority[j - 1];
                    }
                }
                for (int j = 1; j < 4; j++)
                {
                    if (j == i.tra_status)
                    {
                        transactionview.tra_status_name = EnumTransaction.tra_status[j - 1];
                    }
                }
                //Bat cac truong tra ve id 
                transactionview.staff_name = _dbContext.staffs.Where(s => s.sta_id == i.staff_id).FirstOrDefault().sta_fullname;
                transactionview.customer_name = _dbContext.customers.Where(s => s.cu_id == i.customer_id).FirstOrDefault().cu_fullname;
                //lay ra thong tin khach hang 
                var cus = _dbContext.customers.Where(c => c.cu_id == i.customer_id).FirstOrDefault();
                var customerview = _mapper.Map<transactioncustomerviewmodel>(cus);
                var sources = _dbContext.sources.FirstOrDefault(x => x.src_id == cus.source_id);
                var customergroup = _dbContext.customer_group.FirstOrDefault(x => x.cg_id == cus.customer_group_id);
                customerview.source_name = sources.src_name;
                customerview.customer_group_name = customergroup.cg_name;
                for (int j = 1; j < 3; j++)
                {
                    if (j == cus.cu_type)
                    {
                        customerview.cu_type_name = EnumCustomer.cu_type[j - 1];
                    }
                }
                //Lay ra lich su mua 
                List<transactionorderproductviewmodel> list_add_order_product = new List<transactionorderproductviewmodel>();
                var list_customer_order = _dbContext.customer_order.Where(cuo => cuo.customer_id == i.customer_id).ToList();
                foreach(customer_order cuo in list_customer_order)
                {
                    var list_order_product = _dbContext.order_product.Where(o => o.customer_order_id == cuo.cuo_id).ToList();
                    foreach(order_product ord in list_order_product)
                    {
                        transactionorderproductviewmodel add = _mapper.Map<transactionorderproductviewmodel>(ord);
                        var prodcut_cur = _dbContext.products.Where(pu => pu.pu_id == add.product_id).FirstOrDefault();
                        add.pu_name = prodcut_cur.pu_name;
                        for (int j = 1; j < 3; j++)
                        {
                            if (j == prodcut_cur.pu_unit)
                            {
                                add.pu_unit_name = EnumProduct.pu_unit[j - 1];
                            }
                        }
                        add.cu_fullname = _dbContext.customers.Where(c => c.cu_id == cus.cu_id).FirstOrDefault().cu_fullname;
                        for (int j = 1; j < 4; j++)
                        {
                            if (j == cuo.cuo_status)
                            {
                                add.cuo_status_name = EnumCustomerOrder.status[j - 1];
                            }
                        }
                        add.cuo_date = cuo.cuo_date;
                        list_add_order_product.Add(add);
                    }

                }
                customerview.list_order_product = list_add_order_product;
                transactionview.customer = customerview;
                res.Add(transactionview);
            }


            var mod = totalNumberOfRecords % pageSize;

            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            return new PagedResults<transactionviewmodel>
            {
                Results = res,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords
            };
        }
    }
}
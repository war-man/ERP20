﻿using ERP.Common.GenericRepository;
using ERP.Common.Models;
using ERP.Data.DbContext;
using ERP.Data.ModelsERP;
using ERP.Repository.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Repository.Repositories
{
    public class FieldTemplateRepository : GenericRepository<field_template>, IFieldTemplateRepository
    {
        public FieldTemplateRepository(ERPDbContext dbContext) : base(dbContext)
        {
        }
        public PagedResults<field_template> CreatePagedResults(int pageNumber, int pageSize)
        {
            var skipAmount = pageSize * pageNumber;

            var list = _dbContext.field_template.OrderBy(t => t.fit_id).Skip(skipAmount).Take(pageSize);

            var totalNumberOfRecords = _dbContext.field_template.Count();

            var results = list.ToList();

            var mod = totalNumberOfRecords % pageSize;

            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            return new PagedResults<field_template>
            {
                Results = results,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords
            };
        }
    }
}
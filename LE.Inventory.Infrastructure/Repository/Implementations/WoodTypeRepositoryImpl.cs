using LE.Common.Repository.Implementations;
using LE.Context.Data;
using LE.Inventory.Entities;
using LE.Inventory.Infrastructure.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LE.Inventory.Infrastructure.Repository.Implementations
{
    public class WoodTypeRepositoryImpl : BaseRepositoryImpl<WoodType>, WoodTypeRepository
    {
        private readonly AppDbContext _appDbContext;

        public WoodTypeRepositoryImpl(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public WoodType getByName(string name)
        {
            return _appDbContext.wood_type.Where(a => a.name == name).SingleOrDefault();
        }
    }
}

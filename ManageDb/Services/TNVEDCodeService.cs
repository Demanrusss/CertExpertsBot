using ManageDb.Data;
using ManageDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Services
{
    public class TNVEDCodeService : ITNVEDCodeService<TNVEDCode>
    {
        private readonly AppDbContext dbContext;

        public TNVEDCodeService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ICollection<TNVEDCode>> GetAllAsync()
        {
            return await dbContext.TNVEDCodes
                .OrderByDescending(c => c.Code)
                .Take(10)
                .ToListAsync();
        }

        public async Task<ICollection<TNVEDCode>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1 || pageSize < 10 || pageSize > 100)
                return await GetAllAsync();

            return await dbContext.TNVEDCodes
                .OrderByDescending(c => c.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task<ICollection<TNVEDCode>> GetAllWithTechRegsAsync()
        {
            return await dbContext.TNVEDCodes
                .Include(c => c.TechRegs)
                .OrderByDescending(c => c.Code)
                .Take(10)
                .Select(c => new TNVEDCode
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    TechRegs = c.TechRegs.Select(tr => new TechReg
                    {
                        Name = tr.Name
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<ICollection<TNVEDCode>> GetAllWithTechRegsAsync(int page, int pageSize)
        {
            if (page < 1 || pageSize < 10 || pageSize > 100)
                return await GetAllWithTechRegsAsync();

            return await dbContext.TNVEDCodes
                .OrderByDescending(c => c.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new TNVEDCode
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    TechRegs = c.TechRegs.Select(tr => new TechReg
                    {
                        Name = tr.Name
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<ICollection<TNVEDCode>> GetByCodeAsync(string searchStr)
        {
            if (String.IsNullOrEmpty(searchStr))
                return await GetAllAsync();

            return await dbContext.TNVEDCodes.Where(c => c.Code.Contains(searchStr)).ToListAsync();
        }

        public async Task<TNVEDCode> GetByIdAsync(int id)
        {
            var tNVEDCode = await dbContext.TNVEDCodes
                .Include(c => c.TechRegs)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return tNVEDCode ?? new TNVEDCode();
        }

        public async Task<int> AddAsync(TNVEDCode entity)
        {
            if (entity == null)
                return 0;

            var tNVEDCode = await GetByIdAsync(entity.Id);
            if (tNVEDCode.Id != 0)
                return 0;

            dbContext.TNVEDCodes.Add(entity);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(TNVEDCode entity)
        {
            if (entity == null)
                return 0;

            var tNVEDCode = await GetByIdAsync(entity.Id);
            if (tNVEDCode.Id == 0)
                return 0;

            tNVEDCode.Code = entity.Code;
            tNVEDCode.Name = entity.Name;
            tNVEDCode.TechRegs = entity.TechRegs;
            dbContext.TNVEDCodes.Update(tNVEDCode);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var tNVEDCode = await GetByIdAsync(id);
            if (tNVEDCode.Id == 0)
                return 0;

            dbContext.TNVEDCodes.Remove(tNVEDCode);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await dbContext.TNVEDCodes.CountAsync();
        }
    }
}

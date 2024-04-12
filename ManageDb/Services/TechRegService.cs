using ManageDb.Data;
using ManageDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Services
{
    public class TechRegService : ITechRegService<TechReg>
    {
        private readonly AppDbContext dbContext;

        public TechRegService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ICollection<TechReg>> GetAllAsync()
        {
            return await dbContext.TechRegs.ToListAsync();
        }

        public async Task<ICollection<TechReg>> GetAllAsync(int page, int pageSize)
        {
            if (page < 1 || pageSize < 10)
                return await dbContext.TechRegs
                    .OrderByDescending(tr => tr.Name)
                    .Take(10)
                    .ToListAsync();

            return await dbContext.TechRegs
                .OrderByDescending(tr => tr.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<TechReg> GetByIdAsync(int id)
        {
            var techReg = await dbContext.TechRegs.FirstOrDefaultAsync(tr => tr.Id == id);

            return techReg ?? new TechReg();
        }

        public async Task<ICollection<TechReg>> GetByNameAsync(string searchStr)
        {
            if (String.IsNullOrWhiteSpace(searchStr))
                return await GetAllAsync();

            return await dbContext.TechRegs.Where(tr => tr.Name.Contains(searchStr)).ToListAsync();
        }

        public async Task<int> AddAsync(TechReg entity)
        {
            if (entity == null)
                return 0;

            var techReg = await GetByIdAsync(entity.Id);
            if (techReg.Id != 0)
                return 0;

            dbContext.TechRegs.Add(entity);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(TechReg entity)
        {
            if (entity == null)
                return 0;

            var techReg = await GetByIdAsync(entity.Id);
            if (techReg.Id == 0)
                return 0;

            techReg.Name = entity.Name;
            techReg.Description = entity.Description;
            dbContext.TechRegs.Update(techReg);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var techReg = await GetByIdAsync(id);
            if (techReg.Id == 0)
                return 0;

            dbContext.TechRegs.Remove(techReg);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await dbContext.TechRegs.CountAsync();
        }
    }
}

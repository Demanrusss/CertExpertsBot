using ManageDb.Data;
using ManageDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Services;

public class TNVEDCodeService(AppDbContext dbContext) : ITNVEDCodeService<TNVEDCodeModel>
{
    public async Task<ICollection<TNVEDCodeModel>> GetAllAsync() => 
        await dbContext.TNVEDCodes
            .OrderByDescending(c => c.Code)
            .Take(10)
            .ToListAsync();

    public async Task<ICollection<TNVEDCodeModel>> GetAllAsync(int page, int pageSize) =>
        page < 1 || pageSize < 10 || pageSize > 100
            ? await GetAllAsync()
            : await dbContext.TNVEDCodes
                .OrderByDescending(c => c.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

    private async Task<ICollection<TNVEDCodeModel>> GetAllWithTechRegsAsync() =>
        await dbContext.TNVEDCodes
            .Include(c => c.TechRegs)
            .OrderByDescending(c => c.Code)
            .Take(10)
            .Select(c => new TNVEDCodeModel
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                TechRegs = c.TechRegs!.Select(tr => new TechRegModel
                {
                    Name = tr.Name
                }).ToList()
            })
            .ToListAsync();

    public async Task<ICollection<TNVEDCodeModel>> GetAllWithTechRegsAsync(int page, int pageSize) =>
        page < 1 || pageSize < 10 || pageSize > 100
            ? await GetAllWithTechRegsAsync()
            : await dbContext.TNVEDCodes
                .OrderByDescending(c => c.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new TNVEDCodeModel
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    TechRegs = c.TechRegs!.Select(tr => new TechRegModel
                    {
                        Name = tr.Name
                    }).ToList()
                })
                .ToListAsync();

    public async Task<ICollection<TNVEDCodeModel>> GetByCodeAsync(string searchStr) =>
        String.IsNullOrEmpty(searchStr)
            ? await GetAllAsync()
            : await dbContext.TNVEDCodes
                .Where(c => c.Code.Contains(searchStr))
                .Take(100)
                .ToListAsync();

    public async Task<ICollection<TNVEDCodeModel>> GetByCodeWithTechRegsAsync(string searchStr) =>
        String.IsNullOrEmpty(searchStr)
            ? await GetAllWithTechRegsAsync()
            : await dbContext.TNVEDCodes
                .Where(c => c.Code.Contains(searchStr))
                .OrderByDescending(c => c.Code)
                .Take(100)
                .Select(c => new TNVEDCodeModel
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    TechRegs = c.TechRegs!.Select(tr => new TechRegModel
                    {
                        Name = tr.Name
                    }).ToList()
                })
                .ToListAsync();

    public async Task<TNVEDCodeModel> GetByIdAsync(int id) =>
        await dbContext.TNVEDCodes
            .Include(c => c.TechRegs)
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync()  ?? new TNVEDCodeModel();

    public async Task<int> AddAsync(TNVEDCodeModel? entity)
    {
        if (entity == null)
            return 0;

        var tNVEDCode = await GetByIdAsync(entity.Id);
        if (tNVEDCode.Id != 0)
            return 0;

        dbContext.TNVEDCodes.Add(entity);

        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(TNVEDCodeModel? entity)
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

    public async Task<int> GetCountAsync() => await dbContext.TNVEDCodes.CountAsync();
}
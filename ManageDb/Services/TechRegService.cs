using ManageDb.Data;
using ManageDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Services;

public class TechRegService(AppDbContext dbContext) : ITechRegService<TechRegModel>
{
    public async Task<ICollection<TechRegModel>> GetAllAsync() => await dbContext.TechRegs.ToListAsync();

    public async Task<ICollection<TechRegModel>> GetAllAsync(int page, int pageSize) =>
        page < 1 || pageSize < 10
            ? await dbContext.TechRegs
                .OrderByDescending(tr => tr.Name)
                .Take(10)
                .ToListAsync()
            : await dbContext.TechRegs
                .OrderByDescending(tr => tr.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

    public async Task<TechRegModel> GetByIdAsync(int id) =>
        await dbContext.TechRegs
            .Include(tr => tr.TNVEDCodes)
            .FirstOrDefaultAsync(tr => tr.Id == id) ?? new TechRegModel();

    public async Task<ICollection<TechRegModel>> GetByNameAsync(string searchStr) =>
        String.IsNullOrWhiteSpace(searchStr)
            ? await GetAllAsync()
            : await dbContext.TechRegs.Where(tr => tr.Name.Contains(searchStr)).ToListAsync();

    public async Task<int> AddAsync(TechRegModel entity)
    {
        var techReg = await GetByIdAsync(entity.Id);
        if (techReg.Id != 0)
            return 0;

        dbContext.TechRegs.Add(entity);

        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(TechRegModel entity)
    {
        var techReg = await GetByIdAsync(entity.Id);
        if (techReg.Id == 0)
            return 0;

        techReg.Name = entity.Name;
        techReg.Description = entity.Description;
        dbContext.TechRegs.Update(techReg);

        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateTNVEDCodesAsync(int techRegId, IReadOnlyCollection<int> tnvedCodeIds)
    {
        var techReg = await dbContext.TechRegs
            .Include(x => x.TNVEDCodes)
            .FirstOrDefaultAsync(x => x.Id == techRegId);

        if (techReg == null)
            return 0;

        var selectedIds = tnvedCodeIds.ToHashSet();

        var codesToRemove = techReg.TNVEDCodes
            .Where(x => !selectedIds.Contains(x.Id))
            .ToList();

        foreach (var code in codesToRemove)
            techReg.TNVEDCodes.Remove(code);

        var existingIds = techReg.TNVEDCodes
            .Select(x => x.Id)
            .ToHashSet();

        var codesToAdd = await dbContext.TNVEDCodes
            .Where(x => selectedIds.Contains(x.Id) && !existingIds.Contains(x.Id))
            .ToListAsync();

        foreach (var code in codesToAdd)
            techReg.TNVEDCodes.Add(code);

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

    public async Task<int> GetCountAsync() => await dbContext.TechRegs.CountAsync();
}
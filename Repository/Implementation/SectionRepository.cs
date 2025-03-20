using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class SectionRepository : ISectionRepository
{
    private readonly PizzaShopContext _context;

    public SectionRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Section>> GetAllSectionsAsync()
    {
        return await _context.Sections.Where(s => s.IsDeleted == false).ToListAsync();
    }

    public async Task<Section?> GetSectionByIdAsync(int id)
    {
        return await _context.Sections.FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == false);
    }

    public async Task<bool> AddSectionAsync(Section section)
    {
        bool isNameUnique = !await _context.Sections
            .AnyAsync(s => s.Name.ToLower() == section.Name.ToLower() && s.IsDeleted == false);
        if (!isNameUnique)
            throw new Exception("Section name must be unique.");

        await _context.Sections.AddAsync(section);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateSectionAsync(Section section)
    {
        bool isNameUnique = !await _context.Sections
            .AnyAsync(s => s.Name.ToLower() == section.Name.ToLower() && s.Id != section.Id && s.IsDeleted == false);
        if (!isNameUnique)
            throw new Exception("Section name must be unique.");

        _context.Sections.Update(section);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeleteSectionAsync(int id, bool softDelete)
    {
        var section = await GetSectionByIdAsync(id);
        if (section != null)
        {
            if (softDelete)
            {
                section.IsDeleted = true;
                _context.Sections.Update(section);

                var tables = await _context.Tables.Where(t => t.SectionId == id && t.IsDeleted == false).ToListAsync();
                foreach (var table in tables)
                {
                    table.IsDeleted = true;
                    _context.Tables.Update(table);
                }
            }
            else
            {
                _context.Sections.Remove(section);
            }

            await _context.SaveChangesAsync();
        }
    }
}
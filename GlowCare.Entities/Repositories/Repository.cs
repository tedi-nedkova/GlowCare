using Microsoft.EntityFrameworkCore;
using GlowCare.Entities;
using GlowCare.Entities.Contracts.Interfaces;

namespace GlowCare.Entities.Repositories;

public class Repository<TType, TId>(
    GlowCareDbContext context)
    : IRepository<TType, TId>
    where TType
    : class
{
    private readonly DbSet<TType> _dbSet
        = context.Set<TType>();

    public TType GetById(
        TId id)
    {
        TType? entity = _dbSet
            .Find(id);

        return entity is null
            ? throw new NullReferenceException("Entity is null.")
            : entity;
    }

    public async Task<TType> GetByIdAsync(
        TId id)
    {
        TType? entity = await _dbSet
            .FindAsync(id);

        return entity is null
            ? throw new NullReferenceException("Entity is null.")
            : entity;
    }

    public ICollection<TType> GetAll()
        => [.. _dbSet];

    public async Task<ICollection<TType>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public IQueryable<TType> GetAllAttached()
        => _dbSet.AsQueryable();

    public void Add(
        TType item)
    {
        _dbSet.Add(item);
        context.SaveChanges();
    }

    public async Task AddAsync(
        TType item)
    {
        await _dbSet.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public bool Delete(
        TId id)
    {
        TType entity = GetById(id);

        if (entity is null)
        {
            return false;
        }

        _dbSet.Remove(entity);
        context.SaveChanges();

        return true;
    }

    public async Task<bool> DeleteAsync(
        TId id)
    {
        TType entity = await GetByIdAsync(id);

        if (entity is null)
        {
            return false;
        }

        _dbSet.Remove(entity);
        await context.SaveChangesAsync();

        return true;
    }

    public bool Update(
        TType item)
    {
        try
        {
            _dbSet.Attach(item);
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(
        TType item)
    {
        try
        {
            _dbSet.Attach(item);
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
﻿

using Data.Contexts;
using Data.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repository;

public class GenericRepository<T>(DataContext context) : IGenericRepository<T> where T : class
{
    private readonly DataContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();
    private IDbContextTransaction? _transaction;

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    //Generisk repository för CRUD-operationer
    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(T entity)
    {
        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            var key = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
            if (key == null) throw new InvalidOperationException("Ingen primärnyckel hittades på entiteten.");

            var keyValues = key.Properties
                .Select(p => p.PropertyInfo?.GetValue(entity))
                .ToArray();

            var attachedEntity = await _dbSet.FindAsync(keyValues);

            if (attachedEntity == null)
            {
                return;
            }

            _dbSet.Remove(attachedEntity);
        }
        else
        {
            _dbSet.Remove(entity);
        }

        await _context.SaveChangesAsync();
    }
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    public async Task RollbackTransactionsAync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    public DataContext Context => _context;
}

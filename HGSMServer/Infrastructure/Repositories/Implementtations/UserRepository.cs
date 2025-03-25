﻿using Domain.Models;
//using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations 
{
    public class UserRepository : IUserRepository
    {
        private readonly HgsdbContext _context;

        public UserRepository(HgsdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.UserId == id); 
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = await _context.Users
                .Include(u => u.Role) 
                .Include(u => u.Parent)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (existingUser == null)
                throw new ArgumentException($"User with ID {user.UserId} not found.");

            _context.Entry(existingUser).CurrentValues.SetValues(user);

            if (user.Role != null)
            {
                existingUser.Role = user.Role;
            }
            if (user.Parent != null)
            {
                existingUser.Parent = user.Parent;
            }
            if (user.Teacher != null)
            {
                existingUser.Teacher = user.Teacher;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new ArgumentException($"User with ID {id} not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
﻿using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementtations
{
    public class TimetableDetailRepository : ITimetableDetailRepository
    {
        private readonly HgsdbContext _context;

        public TimetableDetailRepository(HgsdbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TimetableDetail detail)
        {
            await _context.TimetableDetails.AddAsync(detail);
        }

        //public async Task<bool> IsConflictAsync(int classId, string dayOfWeek, int periodId)
        //{
        //    return await _context.TimetableDetails.AnyAsync(x =>
        //        x.ClassId == classId &&
        //        x.DayOfWeek == dayOfWeek &&
        //        x.PeriodId == periodId);
        //}
        public async Task<TimetableDetail> GetByIdAsync(int timetableDetailId)
        {
            return await _context.TimetableDetails
                .Include(td => td.Timetable)  
                .Include(td => td.Subject)
                .FirstOrDefaultAsync(td => td.TimetableDetailId == timetableDetailId);
        }

        public async Task<TimetableDetail> GetByTeacherAndTimeAsync(int teacherId, string dayOfWeek, int periodId, int timetableId)
        {
            return await _context.TimetableDetails
                .Where(td => td.TeacherId == teacherId
                          && td.DayOfWeek == dayOfWeek
                          && td.PeriodId == periodId
                          && td.TimetableId == timetableId)
                .FirstOrDefaultAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

﻿using Application.Features.Timetables.DTOs;
using Domain.Models;

namespace Application.Features.Timetables.Interfaces
{
    public interface ITimetableService
    {
        //Task<TimetableResponse> GetTimetableAsync(int classId, DateOnly effectiveDate);
        Task<Timetable?> GetTimetableByIdAsync(int timetableId);
        Task<Timetable> AddTimetableAsync(Timetable timetable);
        Task<bool> UpdateTimetableAsync(Timetable timetable);
        Task<bool> DeleteTimetableAsync(int timetableId);
        Task<byte[]> ExportTimetableToExcel(int classId, DateOnly effectiveDate);
        Task<bool> ImportTimetableFromExcel(Stream fileStream);
    }
}

using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using ClosedXML.Excel;
using Domain.Models; // Đảm bảo namespace đúng với model EF sinh ra
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Timetables.Services
{
    public class TimetableService
    {
        private readonly HgsdbContext _context;

        public TimetableService(HgsdbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleResponseDto> GenerateTimetableAsync(TimetableRequest request)
        {
            var constraints = request.Constraints;
            var classes = await _context.Classes
                .Where(c => c.Grade >= 6 && c.Grade <= 9) // THCS
                .ToListAsync();
            var subjects = await _context.Subjects.ToListAsync();
            var teachers = await _context.Teachers.ToListAsync();
            var teacherSubjects = await _context.TeacherSubjects.Include(ts => ts.Subject).ToListAsync();

            // Lấy SemesterID từ request.SchoolYear và request.Semester
            var semester = await _context.Semesters
                .Include(s => s.AcademicYear)
                .FirstOrDefaultAsync(s => s.AcademicYear.YearName == request.SchoolYear && s.SemesterName == $"Học kỳ {request.Semester}");
            if (semester == null)
            {
                throw new InvalidOperationException($"Semester not found for SchoolYear: {request.SchoolYear}, Semester: {request.Semester}");
            }

            var assignments = await _context.TeachingAssignments
                .Where(ta => ta.SemesterId == semester.SemesterId) // Dùng SemesterId thay vì Semester và AcademicYear
                .ToListAsync();

            if (!classes.Any() || !subjects.Any() || !teachers.Any() || !assignments.Any())
            {
                throw new InvalidOperationException("Required data (classes, subjects, teachers, or assignments) is missing.");
            }

            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            var timetable = new List<Timetable>();
            var teacherWeeklyPeriods = new Dictionary<int, int>();

            foreach (var teacher in teachers)
            {
                int maxPeriods = constraints.DefaultTeacherPeriods;
                if (teacher.Position == "Hiệu trưởng") maxPeriods = constraints.PrincipalPeriods;
                else if (teacher.Position == "Hiệu phó") maxPeriods = constraints.VicePrincipalPeriods;
                else if (teacher.IsHeadOfDepartment == true) maxPeriods -= constraints.HeadOfDepartmentReduction;
                else if (teacher.AdditionalDuties?.Contains("Tổ phó") == true) maxPeriods -= constraints.DeputyHeadReduction;
                else if (teacher.AdditionalDuties?.Contains("Tổng phụ trách đội") == true) maxPeriods -= constraints.TeamLeaderReduction;
                teacherWeeklyPeriods[teacher.TeacherId] = 0;
            }

            foreach (var cls in classes)
            {
                var classAssignments = assignments.Where(a => a.ClassId == cls.ClassId).ToList();
                int periodsPerDay = 5;

                foreach (var day in days)
                {
                    if (day == "Thursday" && request.Semester == 1) periodsPerDay = constraints.ThursdayPeriodsSemester1;
                    else periodsPerDay = 5;

                    for (int period = 1; period <= periodsPerDay; period++)
                    {
                        if (constraints.HasFlagCeremony && day == "Monday" && period == 1)
                        {
                            timetable.Add(CreateTimetableEntry(cls.ClassId, "Hoạt động trải nghiệm", teachers.First().TeacherId, day, period, semester.SemesterId, constraints));
                            continue;
                        }
                        if (constraints.HasClassMeeting && day == "Saturday" && period == 5)
                        {
                            timetable.Add(CreateTimetableEntry(cls.ClassId, "Hoạt động trải nghiệm", teachers.First().TeacherId, day, period, semester.SemesterId, constraints));
                            continue;
                        }

                        var assignment = classAssignments
                            .Where(a => CanAssign(a, timetable, day, period, teacherWeeklyPeriods, cls.Grade, constraints))
                            .OrderBy(a => Guid.NewGuid())
                            .FirstOrDefault();

                        if (assignment != null)
                        {
                            var subject = subjects.First(s => s.SubjectId == assignment.SubjectId);
                            if (constraints.NoPEInLastPeriod && subject.SubjectName == "Thể dục" && period == periodsPerDay) continue;

                            timetable.Add(CreateTimetableEntry(cls.ClassId, subject.SubjectName, assignment.TeacherId, day, period, semester.SemesterId, constraints));
                            teacherWeeklyPeriods[assignment.TeacherId]++;

                            if (constraints.DoubleLiteratureDay && subject.SubjectName == "Ngữ văn" && period < periodsPerDay)
                            {
                                timetable.Add(CreateTimetableEntry(cls.ClassId, subject.SubjectName, assignment.TeacherId, day, period + 1, semester.SemesterId, constraints));
                                teacherWeeklyPeriods[assignment.TeacherId]++;
                                period++;
                            }
                        }
                    }
                }
            }

            var scheduleData = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<ScheduleResponseDto.PeriodDto>>>>>();

            foreach (var cls in classes)
            {
                var gradeKey = $"Grade {cls.Grade}";
                if (!scheduleData.ContainsKey(gradeKey))
                {
                    scheduleData[gradeKey] = new Dictionary<string, Dictionary<string, Dictionary<string, List<ScheduleResponseDto.PeriodDto>>>>();
                }

                var classKey = cls.ClassName;
                if (!scheduleData[gradeKey].ContainsKey(classKey))
                {
                    scheduleData[gradeKey][classKey] = new Dictionary<string, Dictionary<string, List<ScheduleResponseDto.PeriodDto>>>
                    {
                        { "Monday", new Dictionary<string, List<ScheduleResponseDto.PeriodDto>> { { "Morning", new List<ScheduleResponseDto.PeriodDto>() }, { "Afternoon", new List<ScheduleResponseDto.PeriodDto>() } } },
                        { "Tuesday", new Dictionary<string, List<ScheduleResponseDto.PeriodDto>> { { "Morning", new List<ScheduleResponseDto.PeriodDto>() }, { "Afternoon", new List<ScheduleResponseDto.PeriodDto>() } } },
                        { "Wednesday", new Dictionary<string, List<ScheduleResponseDto.PeriodDto>> { { "Morning", new List<ScheduleResponseDto.PeriodDto>() }, { "Afternoon", new List<ScheduleResponseDto.PeriodDto>() } } },
                        { "Thursday", new Dictionary<string, List<ScheduleResponseDto.PeriodDto>> { { "Morning", new List<ScheduleResponseDto.PeriodDto>() }, { "Afternoon", new List<ScheduleResponseDto.PeriodDto>() } } },
                        { "Friday", new Dictionary<string, List<ScheduleResponseDto.PeriodDto>> { { "Morning", new List<ScheduleResponseDto.PeriodDto>() }, { "Afternoon", new List<ScheduleResponseDto.PeriodDto>() } } },
                        { "Saturday", new Dictionary<string, List<ScheduleResponseDto.PeriodDto>> { { "Morning", new List<ScheduleResponseDto.PeriodDto>() }, { "Afternoon", new List<ScheduleResponseDto.PeriodDto>() } } }
                    };
                }

                var classTimetable = timetable.Where(t => t.ClassId == cls.ClassId);
                foreach (var entry in classTimetable)
                {
                    var periodDto = new ScheduleResponseDto.PeriodDto
                    {
                        Period = entry.Period,
                        SubjectId = entry.SubjectId.ToString(),
                        TeacherId = entry.TeacherId.ToString()
                    };
                    scheduleData[gradeKey][classKey][entry.DayOfWeek][entry.Shift].Add(periodDto);
                }
            }

            return new ScheduleResponseDto { ScheduleData = scheduleData };
        }

        public async Task SaveGeneratedTimetableAsync(List<ManualTimetableDto> timetableDtos)
        {
            var timetables = timetableDtos.Select(dto => new Timetable
            {
                ClassId = dto.ClassId,
                SubjectId = dto.SubjectId,
                TeacherId = dto.TeacherId,
                DayOfWeek = dto.DayOfWeek,
                Shift = dto.Shift,
                Period = dto.Period,
                SemesterId = GetSemesterId(dto.SchoolYear, dto.Semester), // Lấy SemesterId từ SchoolYear và Semester
                EffectiveDate = dto.EffectiveDate
            }).ToList();

            foreach (var timetable in timetables)
            {
                if (_context.Timetables.Any(t => t.ClassId == timetable.ClassId
                    && t.DayOfWeek == timetable.DayOfWeek
                    && t.Period == timetable.Period
                    && t.SemesterId == timetable.SemesterId
                    && t.EffectiveDate == timetable.EffectiveDate)
                || _context.Timetables.Any(t => t.TeacherId == timetable.TeacherId
                    && t.DayOfWeek == timetable.DayOfWeek
                    && t.Period == timetable.Period
                    && t.SemesterId == timetable.SemesterId
                    && t.EffectiveDate == timetable.EffectiveDate))
                {
                    throw new InvalidOperationException($"Schedule conflict detected for class {timetable.ClassId} or teacher {timetable.TeacherId} on {timetable.DayOfWeek}, period {timetable.Period}.");
                }
            }

            _context.Timetables.AddRange(timetables);
            await _context.SaveChangesAsync();
        }

        public async Task AddManualTimetableAsync(ManualTimetableDto dto)
        {
            var timetable = new Timetable
            {
                ClassId = dto.ClassId,
                SubjectId = dto.SubjectId,
                TeacherId = dto.TeacherId,
                DayOfWeek = dto.DayOfWeek,
                Shift = dto.Shift,
                Period = dto.Period,
                SemesterId = GetSemesterId(dto.SchoolYear, dto.Semester),
                EffectiveDate = dto.EffectiveDate
            };

            if (_context.Timetables.Any(t => t.ClassId == dto.ClassId
                && t.DayOfWeek == dto.DayOfWeek
                && t.Period == dto.Period
                && t.SemesterId == timetable.SemesterId
                && t.EffectiveDate == dto.EffectiveDate)
            || _context.Timetables.Any(t => t.TeacherId == dto.TeacherId
                && t.DayOfWeek == dto.DayOfWeek
                && t.Period == dto.Period
                && t.SemesterId == timetable.SemesterId
                && t.EffectiveDate == dto.EffectiveDate))
            {
                throw new InvalidOperationException("Schedule conflict detected for class or teacher.");
            }

            _context.Timetables.Add(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTimetableAsync(int timetableId, ManualTimetableDto dto)
        {
            var timetable = await _context.Timetables.FindAsync(timetableId);
            if (timetable == null)
            {
                throw new InvalidOperationException("Timetable not found.");
            }

            timetable.ClassId = dto.ClassId;
            timetable.SubjectId = dto.SubjectId;
            timetable.TeacherId = dto.TeacherId;
            timetable.DayOfWeek = dto.DayOfWeek;
            timetable.Shift = dto.Shift;
            timetable.Period = dto.Period;
            timetable.SemesterId = GetSemesterId(dto.SchoolYear, dto.Semester);
            timetable.EffectiveDate = dto.EffectiveDate;

            if (_context.Timetables.Any(t => t.TimetableId != timetableId
                && t.ClassId == dto.ClassId
                && t.DayOfWeek == dto.DayOfWeek
                && t.Period == dto.Period
                && t.SemesterId == timetable.SemesterId
                && t.EffectiveDate == dto.EffectiveDate)
            || _context.Timetables.Any(t => t.TimetableId != timetableId
                && t.TeacherId == dto.TeacherId
                && t.DayOfWeek == dto.DayOfWeek
                && t.Period == dto.Period
                && t.SemesterId == timetable.SemesterId
                && t.EffectiveDate == dto.EffectiveDate))
            {
                throw new InvalidOperationException("Schedule conflict detected for class or teacher.");
            }

            _context.Timetables.Update(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimetableAsync(int timetableId)
        {
            var timetable = await _context.Timetables.FindAsync(timetableId);
            if (timetable == null)
            {
                throw new InvalidOperationException("Timetable not found.");
            }

            _context.Timetables.Remove(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RoleBasedTimetableDto>> GetTimetableForParentAsync(int studentId, string schoolYear, int semester, DateOnly? effectiveDate = null)
        {
            var student = await _context.Students
                .Include(s => s.StudentClasses)
                    .ThenInclude(sc => sc.Class)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }

            // Lấy lớp hiện tại của học sinh trong năm học
            var currentClass = student.StudentClasses
                .FirstOrDefault(sc => sc.AcademicYear.YearName == schoolYear)?.Class;
            if (currentClass == null)
            {
                throw new InvalidOperationException($"No class found for student {studentId} in school year {schoolYear}.");
            }

            var semesterId = GetSemesterId(schoolYear, semester);
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var query = _context.Timetables
                .Where(t => t.ClassId == currentClass.ClassId && t.SemesterId == semesterId);

            if (!effectiveDate.HasValue)
            {
                query = query.Where(t => t.EffectiveDate == currentDate);
            }
            else
            {
                query = query.Where(t => t.EffectiveDate == effectiveDate.Value);
            }

            return await query
                .Select(t => new RoleBasedTimetableDto
                {
                    ClassName = t.Class.ClassName,
                    SubjectName = t.Subject.SubjectName,
                    TeacherName = t.Teacher.FullName,
                    DayOfWeek = t.DayOfWeek,
                    Shift = t.Shift,
                    Period = t.Period
                })
                .OrderBy(t => t.DayOfWeek).ThenBy(t => t.Period)
                .ToListAsync();
        }

        public async Task<List<RoleBasedTimetableDto>> GetTimetableForTeacherAsync(int teacherId, string schoolYear, int semester, DateOnly? effectiveDate = null)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.TeacherId == teacherId);
            if (teacher == null)
            {
                throw new InvalidOperationException("Teacher not found.");
            }

            var semesterId = GetSemesterId(schoolYear, semester);
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var query = _context.Timetables
                .Where(t => t.TeacherId == teacherId && t.SemesterId == semesterId);

            if (!effectiveDate.HasValue)
            {
                query = query.Where(t => t.EffectiveDate == currentDate);
            }
            else
            {
                query = query.Where(t => t.EffectiveDate == effectiveDate.Value);
            }

            return await query
                .Select(t => new RoleBasedTimetableDto
                {
                    ClassName = t.Class.ClassName,
                    SubjectName = t.Subject.SubjectName,
                    TeacherName = t.Teacher.FullName,
                    DayOfWeek = t.DayOfWeek,
                    Shift = t.Shift,
                    Period = t.Period
                })
                .OrderBy(t => t.DayOfWeek).ThenBy(t => t.Period)
                .ToListAsync();
        }

        public async Task<List<RoleBasedTimetableDto>> GetTimetableForPrincipalAsync(string schoolYear, int semester, DateOnly? effectiveDate = null)
        {
            var semesterId = GetSemesterId(schoolYear, semester);
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var query = _context.Timetables
                .Where(t => t.SemesterId == semesterId);

            if (!effectiveDate.HasValue)
            {
                query = query.Where(t => t.EffectiveDate == currentDate);
            }
            else
            {
                query = query.Where(t => t.EffectiveDate == effectiveDate.Value);
            }

            return await query
                .Select(t => new RoleBasedTimetableDto
                {
                    ClassName = t.Class.ClassName,
                    SubjectName = t.Subject.SubjectName,
                    TeacherName = t.Teacher.FullName,
                    DayOfWeek = t.DayOfWeek,
                    Shift = t.Shift,
                    Period = t.Period
                })
                .OrderBy(t => t.ClassName).ThenBy(t => t.DayOfWeek).ThenBy(t => t.Period)
                .ToListAsync();
        }

        private bool CanAssign(TeachingAssignment assignment, List<Timetable> timetable, string day, int period, Dictionary<int, int> teacherWeeklyPeriods, int grade, ExternalConstraints constraints)
        {
            var teacher = _context.Teachers.First(t => t.TeacherId == assignment.TeacherId);
            var subject = _context.Subjects.First(s => s.SubjectId == assignment.SubjectId);

            int maxPeriods = constraints.DefaultTeacherPeriods;
            if (teacher.Position == "Hiệu trưởng") maxPeriods = constraints.PrincipalPeriods;
            else if (teacher.Position == "Hiệu phó") maxPeriods = constraints.VicePrincipalPeriods;
            else if (teacher.IsHeadOfDepartment == true) maxPeriods -= constraints.HeadOfDepartmentReduction;
            else if (teacher.AdditionalDuties?.Contains("Tổ phó") == true) maxPeriods -= constraints.DeputyHeadReduction;
            else if (teacher.AdditionalDuties?.Contains("Chủ tịch công đoàn") == true) maxPeriods -= constraints.UnionChairReduction;
            else if (teacher.AdditionalDuties?.Contains("Tổng phụ trách đội") == true) maxPeriods -= constraints.TeamLeaderReduction;

            if (teacherWeeklyPeriods[assignment.TeacherId] >= maxPeriods) return false;
            if (timetable.Any(t => t.TeacherId == assignment.TeacherId && t.DayOfWeek == day && t.Period == period)) return false;
            if (timetable.Any(t => t.ClassId == assignment.ClassId && t.DayOfWeek == day && t.Period == period)) return false;

            if (constraints.MathLiteratureMaxTwoGrades && (subject.SubjectName == "Toán" || subject.SubjectName == "Ngữ văn"))
            {
                var taughtGrades = _context.TeachingAssignments
                    .Where(ta => ta.TeacherId == assignment.TeacherId && ta.SubjectId == assignment.SubjectId)
                    .Select(ta => _context.Classes.First(c => c.ClassId == ta.ClassId).Grade)
                    .Distinct()
                    .ToList();
                if (taughtGrades.Count >= 2 && !taughtGrades.Contains(grade)) return false;
            }

            return true;
        }

        private Timetable CreateTimetableEntry(int classId, string subjectName, int teacherId, string day, int period, int semesterId, ExternalConstraints constraints)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectName == subjectName);
            if (subject == null)
            {
                throw new InvalidOperationException($"Subject '{subjectName}' not found in the database.");
            }

            return new Timetable
            {
                ClassId = classId,
                SubjectId = subject.SubjectId,
                TeacherId = teacherId,
                DayOfWeek = day,
                Shift = constraints.HasMorningShift && period <= 3 ? "Morning" : "Afternoon",
                Period = period,
                SemesterId = semesterId,
                EffectiveDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        private int GetSemesterId(string schoolYear, int semester)
        {
            var sem = _context.Semesters
                .Include(s => s.AcademicYear)
                .FirstOrDefault(s => s.AcademicYear.YearName == schoolYear && s.SemesterName == $"Học kỳ {semester}");
            if (sem == null)
            {
                throw new InvalidOperationException($"Semester not found for SchoolYear: {schoolYear}, Semester: {semester}");
            }
            return sem.SemesterId;
        }
    }
}
using Application.Features.LessonPlans.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LessonPlans.Interfaces
{
    public interface ILessonPlanService
    {
        Task UploadLessonPlanAsync(LessonPlanUploadDto lessonPlanDto);
        Task ReviewLessonPlanAsync(LessonPlanReviewDto reviewDto);
    }
}

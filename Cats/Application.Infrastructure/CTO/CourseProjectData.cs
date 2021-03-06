﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Infrastructure.CTO
{
    public class CourseProjectData
    {
        public int? Id { get; set; }

        [Required]
        public string Theme { get; set; }

        public string Lecturer { get; set; }

        public string Student { get; set; }

        public int? StudentId { get; set; }

        public string Group { get; set; }

        public DateTime? ApproveDate { get; set; }

        public string ApproveDateString => ApproveDate?.ToString("dd-MM-yyyy");

        public int? LecturerId { get; set; }

        public string Subject { get; set; }

        public int? SubjectId { get; set; }

        public IEnumerable<int> SelectedGroupsIds { get; set; }
    }
}
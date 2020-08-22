using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Application.Core.UI.HtmlHelpers;
using LMP.Models;
using LMP.Models.KnowledgeTesting;
using Microsoft.AspNetCore.Html;

namespace WebAPI.ViewModels.KnowledgeTestingViewModels
{
    public class TestResultItemListViewModel : BaseNumberedGridItem
    {
        public string SubGroup { get; set; }

        [DisplayName("Студент")] 
        public string StudentName { get; set; }

        public string StudentShortName { get; set; }

        public string Login { get; set; }

        [DisplayName("Оценки")] 
        public HtmlString Marks { get; set; }

        public TestPassResultViewModel[] TestPassResults { get; set; }

        public static TestResultItemListViewModel FromStudent(Student student, HtmlString marks,
            IEnumerable<Test> tests)
        {
            return new TestResultItemListViewModel
            {
                Login = student.User.UserName,
                StudentName = student.FullName,
                StudentShortName = GetShortStudentName(student),
                Marks = marks,
                TestPassResults = student.User.TestPassResults.Select(x =>
                    TestPassResultViewModel.FromModel(x, tests.FirstOrDefault(y => y.Id == x.TestId))).ToArray()
            };
        }

        public static TestResultItemListViewModel FromStudent(Student student, IEnumerable<Test> tests,
            IList<SubGroup> subGroups)
        {
            return new TestResultItemListViewModel
            {
                Login = student.User.UserName,
                StudentName = student.FullName,
                StudentShortName = GetShortStudentName(student),
                TestPassResults = student.User.TestPassResults.Where(x => tests.Any(y => y.Id == x.TestId))
                    .Select(x => TestPassResultViewModel.FromModel(x, tests.FirstOrDefault(y => y.Id == x.TestId)))
                    .OrderBy(x => x.TestNumber).ToArray(),
                SubGroup = subGroups.FirstOrDefault(x => x.SubjectStudents.Any(y => y.StudentId == student.Id)) != null
                    ? subGroups.FirstOrDefault(x => x.SubjectStudents.Any(y => y.StudentId == student.Id)).Name
                    : ""
            };
        }

        private static string GetShortStudentName(Student student)
        {
            return student.LastName + ' ' + student.FirstName[0] + '.';
        }

        public class TestPassResultViewModel
        {
            public int StudentId { get; set; }

            public int TestId { get; set; }

            public int? Points { get; set; }

            public int? Percent { get; set; }

            public DateTime StartTime { get; set; }

            public User User { get; set; }

            public string Comment { get; set; }

            public int? CalculationType { get; set; }

            public string TestName { get; set; }

            public bool ForSelfStudy { get; set; }

            public bool ForNN { get; set; }

            public bool ForEUMK { get; set; }

            public bool BeforeEUMK { get; set; }

            public int? TestNumber { get; set; }

            public static TestPassResultViewModel FromModel(TestPassResult model, Test test)
            {
                return new TestPassResultViewModel
                {
                    StudentId = model.StudentId,
                    TestId = model.TestId,
                    Points = model.Points,
                    Percent = model.Percent,
                    StartTime = model.StartTime,
                    User = model.User,
                    Comment = model.Comment,
                    CalculationType = model.CalculationType,
                    TestName = model.TestName,
                    ForSelfStudy = test == null || test.ForSelfStudy,
                    ForNN = test?.ForNN ?? false,
                    ForEUMK = test?.ForEUMK ?? true,
                    BeforeEUMK = test?.BeforeEUMK ?? true,
                    TestNumber = test.TestNumber
                };
            }
        }
    }
}
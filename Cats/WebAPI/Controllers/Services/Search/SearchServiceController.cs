﻿using System.Collections.Generic;
using Application.Core;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.LecturerManagement;
using Application.Infrastructure.ProjectManagement;
using Application.Infrastructure.StudentManagement;
using Application.SearchEngine.SearchMethods;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Services.Search
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SearchServiceController : ApiRoutedController
    {
        private readonly LazyDependency<IGroupManagementService> _groupRepository =
            new LazyDependency<IGroupManagementService>();

        private readonly LazyDependency<ILecturerManagementService> _lecturerRepository =
            new LazyDependency<ILecturerManagementService>();

        private readonly LazyDependency<IProjectManagementService> _projectRepository =
            new LazyDependency<IProjectManagementService>();

        private readonly LazyDependency<IStudentManagementService> _studentRepository =
            new LazyDependency<IStudentManagementService>();

        [HttpGet("SearchStudents/{text}")]
        public IActionResult SearchStudents(string text)
        {
            if (string.IsNullOrEmpty(text)) return BadRequest();

            var searchMethod = new StudentSearchMethod();

            if (!searchMethod.IsIndexExist()) searchMethod.AddToIndex(_studentRepository.Value.GetStudents());

            var searchResult = searchMethod.Search(text);

            var data = new Dictionary<string, IEnumerable<Student>> {{"student", searchResult}};
            var result = new Dictionary<string, Dictionary<string, IEnumerable<Student>>> {{"data", data}};

            return Ok(result);
        }

        [HttpGet("SearchProjects/{text}")]
        public IActionResult SearchProjects(string text)
        {
            if (string.IsNullOrEmpty(text)) return BadRequest();

            var searchMethod = new ProjectSearchMethod();

            if (!searchMethod.IsIndexExist()) searchMethod.AddToIndex(_projectRepository.Value.GetProjects());

            var searchResult = searchMethod.Search(text);

            var data = new Dictionary<string, IEnumerable<Project>> {{"project", searchResult}};
            var result = new Dictionary<string, Dictionary<string, IEnumerable<Project>>> {{"data", data}};

            return Ok(result);
        }

        [HttpGet("SearchGroups/{text}")]
        public IActionResult SearchGroups(string text)
        {
            if (string.IsNullOrEmpty(text)) return BadRequest();

            var searchMethod = new GroupSearchMethod();

            if (!searchMethod.IsIndexExist()) searchMethod.AddToIndex(_groupRepository.Value.GetGroups());

            var searchResult = searchMethod.Search(text);

            var data = new Dictionary<string, IEnumerable<Group>> {{"group", searchResult}};
            var result = new Dictionary<string, Dictionary<string, IEnumerable<Group>>> {{"data", data}};

            return Ok(result);
        }

        [HttpGet("SearchLecturers/{text}")]
        public IActionResult SearchLecturers(string text)
        {
            if (string.IsNullOrEmpty(text)) return BadRequest();

            var searchMethod = new LecturerSearchMethod();

            if (!searchMethod.IsIndexExist()) searchMethod.AddToIndex(_lecturerRepository.Value.GetLecturers());

            var searchResult = searchMethod.Search(text);

            var data = new Dictionary<string, IEnumerable<Lecturer>> {{"lecturer", searchResult}};
            var result = new Dictionary<string, Dictionary<string, IEnumerable<Lecturer>>> {{"data", data}};

            return Ok(result);
        }
    }
}
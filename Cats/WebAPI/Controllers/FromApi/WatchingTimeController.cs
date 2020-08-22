using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core;
using Application.Infrastructure.ConceptManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.WatchingTimeManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WatchingTimeController : ApiRoutedController
    {
        private readonly LazyDependency<IConceptManagementService> _conceptManagementService =
            new LazyDependency<IConceptManagementService>();

        private readonly LazyDependency<IStudentManagementService> _studentManagementService =
            new LazyDependency<IStudentManagementService>();

        private readonly LazyDependency<IWatchingTimeService> _watchingTimeService =
            new LazyDependency<IWatchingTimeService>();

        public IStudentManagementService StudentManagementService => _studentManagementService.Value;

        public IWatchingTimeService WatchingTimeService => _watchingTimeService.Value;

        public IConceptManagementService ConceptManagementService => _conceptManagementService.Value;

        private static int? GetConceptParentId(Concept concept)
        {
            var temp = concept;
            while (temp.Parent != null)
            {
                temp = temp.Parent;
            }

            return temp.Id;
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var rootId = -1;
            var studentId = -1;
            var queryParams = Request.Query
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            if (queryParams.ContainsKey("root"))
            {
                int.TryParse(queryParams["root"], out rootId);
            }

            if (queryParams.ContainsKey("studentId"))
            {
                int.TryParse(queryParams["studentId"], out studentId);
            }

            var studentInfo = StudentManagementService.GetStudent(studentId);
            var concepts = ConceptManagementService.GetElementsBySubjectId(id).Where(x => x.Container != null).ToList();
            var viewsResult = new List<WatchingTimeResult>();
            foreach (var concept in concepts)
            {
                if (GetConceptParentId(concept) == rootId)
                {
                    var views = WatchingTimeService.GetAllRecords(concept.Id, studentId).FirstOrDefault();
                    viewsResult.Add(new WatchingTimeResult
                    {
                        ConceptId = concept.Id,
                        ParentId = concept.ParentId,
                        Name = concept.Name,
                        Views = views,
                        Estimated = WatchingTimeService.GetEstimatedTime(concept.Container)
                    });
                }
            }

            var tree = ConceptManagementService.GetTreeConceptByElementId(rootId);
            var treeresult = ConceptResult.GetConceptResultTreeInViews(tree, viewsResult);

            var result = new StudentInfoResult
            {
                Tree = treeresult.Children,
                StudentFullName = studentInfo.FullName,
                GroupNumber = studentInfo.Group.Name
            };

            return Ok(result);
        }

        //// POST api/<controller>
        //public void Post([FromBody] string value)
        //{
        //}

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPut("{id:int}")]
        public void Put(int id)
        {
            var userId = /*todo #auth WebSecurity.CurrentUserId*/2;
            var concept = ConceptManagementService.GetById(id);
            WatchingTimeService.SaveWatchingTime(new WatchingTime(userId, concept.Id, 10));
            //WatchingTimeService.SaveWatchingTime(new WatchingTime(userId, concept, 10));
        }

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }

    public class ConceptResult
    {
        private static readonly LazyDependency<IWatchingTimeService> _watchingTimeService =
            new LazyDependency<IWatchingTimeService>();

        public List<ConceptResult> Children;
        public int ConceptId;
        public int? Estimated;
        public bool IsFile;

        public string Name;
        public int? ViewTime;

        public static IWatchingTimeService WatchingTimeService => _watchingTimeService.Value;

        public static ConceptResult GetConceptResultTreeInViews(Concept concept, List<WatchingTimeResult> views)
        {
            if (concept == null || (concept.Children == null || concept.Children.Count == 0) &&
                views.All(x => x.ConceptId != concept.Id))
                return null;

            var currentConcept = new ConceptResult
            {
                Name = concept.Name,
                IsFile = true
            };

            if (concept.Children != null && concept.Children.Count != 0)
            {
                currentConcept.Children = new List<ConceptResult>();
                foreach (var child in concept.Children)
                {
                    var childConceptResult = GetConceptResultTreeInViews(child, views);
                    if (childConceptResult == null) continue;
                    currentConcept.Children.Add(childConceptResult);
                }

                if (currentConcept.Children.Count == 0) return null;

                currentConcept.IsFile = false;
            }
            else
            {
                var viewInfo = views.Find(x => x.ConceptId == concept.Id).Views;
                if (viewInfo != null)
                    currentConcept.ViewTime = viewInfo.Time;
                else
                    currentConcept.ViewTime = null;

                currentConcept.Estimated = WatchingTimeService.GetEstimatedTime(concept.Container);
            }

            currentConcept.ConceptId = concept.Id;
            return currentConcept;
        }
    }

    public class WatchingTimeResult
    {
        public int ConceptId;
        public int? ParentId;
        public WatchingTime Views;
        public string Name { get; set; }
        public int Estimated { get; set; }
    }

    public class StudentInfoResult
    {
        public string GroupNumber;
        public string StudentFullName;
        public List<ConceptResult> Tree;
    }
}
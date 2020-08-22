using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Application.Core;
using Application.Infrastructure.ConceptManagement;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.KnowledgeTestsManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.SubjectManagement;
using Application.Infrastructure.UserManagement;
using Application.Infrastructure.WatchingTimeManagement;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models;
using WebAPI.Controllers.Services.Models.Concept;
using WebAPI.Controllers.Services.Models.Parental;

namespace WebAPI.Controllers.Services.Concept
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ConceptServiceController : ApiRoutedController
	{
		private const string SuccessMessage = "Операция выполнена успешно";

		private readonly LazyDependency<IConceptManagementService> _conceptManagementService =
			new LazyDependency<IConceptManagementService>();

		private readonly LazyDependency<IFilesManagementService> _filesManagementService =
			new LazyDependency<IFilesManagementService>();

		private readonly LazyDependency<IQuestionsManagementService> _questionsManagementService =
			new LazyDependency<IQuestionsManagementService>();

		private readonly LazyDependency<IStudentManagementService> _studentManagementService =
			new LazyDependency<IStudentManagementService>();

		private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
			new LazyDependency<ISubjectManagementService>();

		private readonly LazyDependency<ITestPassingService> _testPassingService =
			new LazyDependency<ITestPassingService>();

		private readonly LazyDependency<IUsersManagementService> _usersManagementService =
			new LazyDependency<IUsersManagementService>();

		private readonly LazyDependency<IWatchingTimeService> _watchingTimeService =
			new LazyDependency<IWatchingTimeService>();

		public ITestPassingService TestPassingService => _testPassingService.Value;

		public IQuestionsManagementService QuestionsManagementService => _questionsManagementService.Value;

		public IConceptManagementService ConceptManagementService => _conceptManagementService.Value;

		public IStudentManagementService StudentManagementService => _studentManagementService.Value;

		public IWatchingTimeService WatchingTimeService => _watchingTimeService.Value;

		public IUsersManagementService UsersManagementService => _usersManagementService.Value;

		public IFilesManagementService FilesManagementService => _filesManagementService.Value;

		public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

		[HttpPost("AttachSiblings")]
		public IActionResult AttachSiblings(string source, string left, string right)
		{
			try
			{
				var sourceId = int.Parse(source);
				int.TryParse(left, out var leftId);
				int.TryParse(right, out var rightId);

				var concept = ConceptManagementService.AttachSiblings(sourceId, rightId, leftId);

				return Ok(new ConceptResult
				{
					Concept = new ConceptViewData(concept),
					Message = SuccessMessage,
					Code = HttpStatusCode.OK.ToString(),
					SubjectName = concept.Subject.Name
				});
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("CreateRootConcept")]
		public ConceptResult SaveRootConcept(string subject, string name, string container)
		{
			try
			{
				var subjectId = int.Parse(subject);
				var authorId = /*todo #auth WebSecurity.CurrentUserId*/2;

				var root = ConceptManagementService.CreateRootConcept(name, authorId, subjectId);
				var subj = SubjectManagementService.GetSubject(subjectId);
				return new ConceptResult
				{
					Concept = new ConceptViewData(root),
					Message = SuccessMessage,
					SubjectName = subj.Name,
					Code = HttpStatusCode.OK.ToString()
				};
			}
			catch (Exception ex)
			{
				return new ConceptResult
				{
					Message = ex.Message,
					Code = HttpStatusCode.InternalServerError.ToString()
				};
			}
		}

		private bool CurrentUserIsLector()
		{
			return UsersManagementService.CurrentUser.Membership.Roles.Any(r => r.Role.RoleName.Equals("lector"));
		}

		[HttpPost("GetRootConcepts")]
		public ConceptResult GetRootConcepts(string subjectId)
		{
			try
			{
				var valid = int.TryParse(subjectId, out var subject);
				var authorId = /*todo #auth WebSecurity.CurrentUserId*/2;

				var concepts = CurrentUserIsLector() ? ConceptManagementService.GetRootElements(authorId) :
					valid ? ConceptManagementService.GetRootElementsBySubject(subject).Where(c => c.Published) :
					new List<LMP.Models.Concept>();

				if (valid)
					concepts = concepts.Where(c => c.SubjectId == subject);
				var subj = SubjectManagementService.GetSubject(subject);


				return new ConceptResult
				{
					Concepts = concepts.Select(c => new ConceptViewData(c)).ToList(),
					Message = SuccessMessage,
					SubjectName = subj.Name,
					Code = HttpStatusCode.OK.ToString()
				};
			}
			catch (Exception ex)
			{
				return new ConceptResult
				{
					Message = ex.Message,
					Code = HttpStatusCode.InternalServerError.ToString()
				};
			}
		}

		[HttpPost("GetConcepts")]
		public ConceptResult GetConcepts(string parentId)
		{
			try
			{
				var authorId = /*todo #auth WebSecurity.CurrentUserId*/2;
				var parent = int.Parse(parentId);

				var concepts = CurrentUserIsLector()
					? ConceptManagementService.GetElementsByParentId(authorId, parent)
					: ConceptManagementService.GetElementsByParentId(parent);

				var concept = ConceptManagementService.GetById(parent);

				return new ConceptResult
				{
					Concepts = concepts.Select(c => new ConceptViewData(c)).ToList().SortDoubleLinkedList(),
					Concept = new ConceptViewData(concept),
					SubjectName = concept.Subject.Name,
					Message = SuccessMessage,
					Code = HttpStatusCode.OK.ToString()
				};
			}
			catch (Exception ex)
			{
				return new ConceptResult
				{
					Message = ex.Message,
					Code = HttpStatusCode.InternalServerError.ToString()
				};
			}
		}

		[HttpPost("Remove")]
		public ConceptResult Remove(string id)
		{
			try
			{
				var conceptId = int.Parse(id);
				var source = ConceptManagementService.GetById(conceptId);
				var canDelete = source != null && source.Author.Id == /*todo #auth WebSecurity.CurrentUserId*/2;
				if (canDelete) ConceptManagementService.Remove(conceptId, source.IsGroup);

				return new ConceptResult
				{
					Message = SuccessMessage,
					Code = HttpStatusCode.OK.ToString(),
					SubjectName = source.Subject.Name
				};
			}
			catch (Exception ex)
			{
				return new ConceptResult
				{
					Message = ex.Message,
					Code = HttpStatusCode.InternalServerError.ToString()
				};
			}
		}

		[HttpGet("GetConceptTree")]
		public ConceptViewData GetConceptTree(string elementId)
		{
			try
			{
				var parentId = int.Parse(elementId);

				var tree = ConceptManagementService.GetTreeConceptByElementId(parentId);

				return new ConceptViewData(tree, true);
			}
			catch (Exception)
			{
				return null;
			}
		}

		[HttpGet("GetNextConceptData")]
		public AttachViewData GetNextConceptData(string elementId)
		{
			var id = int.Parse(elementId);
			var concept = ConceptManagementService.GetByIdFixed(id);
			return GetNeighborConceptData(concept.NextConcept.GetValueOrDefault());
		}

		[HttpGet("GetPrevConceptData")]
		public AttachViewData GetPrevConceptData(string elementId)
		{
			var id = int.Parse(elementId);
			var concept = ConceptManagementService.GetByIdFixed(id);
			return GetNeighborConceptData(concept.PrevConcept.GetValueOrDefault());
		}

		[HttpGet("GetConceptViews")]
		public MonitoringData GetConceptViews(int conceptId, int groupId)
		{
			var concept = ConceptManagementService.GetById(conceptId);
			var list = WatchingTimeService.GetAllRecords(conceptId);
			var viewRecords = new List<ViewsWorm>();
			foreach (var item in list)
			{
				var student = StudentManagementService.GetStudent(item.UserId);
				if (student != null && student.GroupId == groupId)
					viewRecords.Add(new ViewsWorm
					{
						Name = UsersManagementService.GetUser(item.UserId).FullName,
						Seconds = item.Time
					});
			}

			var views = viewRecords.OrderBy(x => x.Name).ToList();
			var estimated = WatchingTimeService.GetEstimatedTime(concept.Container);
			return new MonitoringData {Views = views, Estimated = estimated};
		}

		private AttachViewData GetNeighborConceptData(int neighborId)
		{
			var neighbor = ConceptManagementService.GetById(neighborId);
			if (neighbor == null)
				return new AttachViewData(0, string.Empty, null);
			var att = FilesManagementService.GetAttachments(neighbor.Container).FirstOrDefault();
			return new AttachViewData(neighbor.Id, neighbor.Name, att);
		}

		[HttpGet("GetConcept")]
		public ConceptViewData GetConcept(string elementId)
		{
			return new ConceptViewData(ConceptManagementService.GetById(Convert.ToInt32(elementId)));
		}

		[HttpGet("GetConceptTitleInfo")]
		public ConceptPageTitleData GetConceptTitleInfo(string subjectId)
		{
			var valid = int.TryParse(subjectId, out var subject);
			var lecturer = SubjectManagementService.GetSubject(subject).SubjectLecturers.FirstOrDefault().Lecturer;
			var subj = SubjectManagementService.GetSubject(subject);
			return new ConceptPageTitleData
			{
				Lecturer = new LectorViewData(lecturer, true),
				Subject = new SubjectViewData(subj)
			};
		}

		[HttpPost("GetRootConceptsMobile")]
		public IActionResult GetRootConceptsMobile(string subjectId, string userId, string identityKey)
		{
			//try
			//{
			if (identityKey != "7e13f363-2f00-497e-828e-49e82d8b4223") throw new UnauthorizedAccessException();

			var subject = 0;
			var valid = int.TryParse(subjectId, out subject);
			if (!int.TryParse(userId, out var authorId)) throw new ArgumentException();

			var user = UsersManagementService.GetUser(authorId);

			var concepts = user.Lecturer != null ? ConceptManagementService.GetRootElements(authorId) :
				valid ? ConceptManagementService.GetRootElementsBySubject(subject).Where(c => c.Published) :
				new List<LMP.Models.Concept>();

			if (valid)
				concepts = concepts.Where(c => c.SubjectId == subject);
			var subj = SubjectManagementService.GetSubject(subject);


			return Ok(new ConceptResult
			{
				Concepts = concepts.Select(c => new ConceptViewData(c)).ToList(),
				SubjectName = subj.Name
			});
			//}
			//catch (Exception ex)
			//{

			//	return new ConceptResult
			//	{
			//		Message = ex.Message,
			//		Code = ServerErrorCode
			//	};
			//}
		}


		public class MonitoringData
		{
			public List<ViewsWorm> Views { get; set; }
			public int Estimated { get; set; }
		}

		public class ViewsWorm
		{
			public string Name { get; set; }
			public int Seconds { get; set; }
		}
	}
}
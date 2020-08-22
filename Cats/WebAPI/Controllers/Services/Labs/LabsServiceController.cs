using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Core.Data;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.GroupManagement;
using Application.Infrastructure.KnowledgeTestsManagement;
using Application.Infrastructure.StudentManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using LMP.PlagiarismNet.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models;
using WebAPI.Controllers.Services.Models.CoreModels;
using WebAPI.Controllers.Services.Models.Labs;

namespace WebAPI.Controllers.Services.Labs
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LabsServiceController : ApiRoutedController
	{
		private readonly LazyDependency<IFilesManagementService> filesManagementService =
			new LazyDependency<IFilesManagementService>();

		private readonly LazyDependency<IGroupManagementService> groupManagementService =
			new LazyDependency<IGroupManagementService>();

		private readonly LazyDependency<IStudentManagementService> studentManagementService =
			new LazyDependency<IStudentManagementService>();

		private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
			new LazyDependency<ISubjectManagementService>();

		private readonly LazyDependency<ITestPassingService> testPassingService =
			new LazyDependency<ITestPassingService>();

		private readonly LazyDependency<ITestsManagementService> testsManagementService =
			new LazyDependency<ITestsManagementService>();

		public string PlagiarismUrl => ConfigurationManager.AppSettings["PlagiarismUrl"];

		public string PlagiarismTempPath => ConfigurationManager.AppSettings["PlagiarismTempPath"];

		public string FileUploadPath => ConfigurationManager.AppSettings["FileUploadPath"];

		public ITestPassingService TestPassingService => testPassingService.Value;

		public IGroupManagementService GroupManagementService => groupManagementService.Value;

		public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

		public IFilesManagementService FilesManagementService => filesManagementService.Value;

		public ITestsManagementService TestsManagementService => testsManagementService.Value;

		public IStudentManagementService StudentManagementService => studentManagementService.Value;

		[HttpGet("GetLabs/{subjectId}")]
		public IActionResult GetLabs(string subjectId)
		{
			try
			{
				var model = SubjectManagementService.GetSubject(int.Parse(subjectId)).Labs.OrderBy(e => e.Order)
					.Select(e => new LabsViewData(e)).ToList();
				return Ok(model);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpGet("GetMarks")]
		public IActionResult GetMarks(int subjectId, int groupId)
		{
			try
			{
				var group = GroupManagementService.GetGroups(
					new Query<Group>(e => e.SubjectGroups.Any(x => x.SubjectId == subjectId && x.GroupId == groupId))
						.Include(e => e.Students.Select(x => x.StudentLabMarks))
						.Include(e => e.Students.Select(x => x.User))).ToList()[0];

				var labsData = SubjectManagementService.GetSubject(subjectId).Labs.OrderBy(e => e.Order).ToList();

				var controlTests = TestsManagementService.GetTestsForSubject(subjectId).Where(x => !x.ForSelfStudy);

				var students = group.Students.Select(student =>
						new StudentsViewData(
							TestPassingService.GetStidentResults(subjectId, student.Id)
								.Where(x => controlTests.Any(y => y.Id == x.TestId)).ToList(), student, labs: labsData))
					.ToList();

				var result = students.Select(e => new StudentMark
				{
					FullName = e.FullName,
					StudentId = e.StudentId,
					LabsMarkTotal = e.LabsMarkTotal,
					TestMark = e.TestMark,
					Marks = e.StudentLabMarks
				}).ToList();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("Save")]
		public IActionResult Save(string subjectId, string id, string theme, string duration, string order,
			string shortName, string pathFile, string attachments)
		{
			try
			{
				var attachmentsModel = JsonSerializer.Deserialize<List<Attachment>>(attachments).ToList();
				var subject = int.Parse(subjectId);
				SubjectManagementService.SaveLabs(new LMP.Models.Labs
				{
					SubjectId = subject,
					Duration = int.Parse(duration),
					Theme = theme,
					Order = int.Parse(order),
					ShortName = shortName,
					Attachments = pathFile,
					Id = int.Parse(id)
				}, attachmentsModel, /*todo #auth WebSecurity.CurrentUserId*/2);

				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpDelete("Delete")]
		public IActionResult Delete(string id, string subjectId)
		{
			try
			{
				SubjectManagementService.DeleteLabs(int.Parse(id));
				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("SaveScheduleProtectionDate")]
		public IActionResult SaveScheduleProtectionDate(string subGroupId, string date)
		{
			try
			{
				SubjectManagementService.SaveScheduleProtectionLabsDate(int.Parse(subGroupId),
					DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture));
				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("SaveLabsVisitingDataSingle")]
		public IActionResult SaveLabsVisitingDataSingle(int dateId, string mark, string comment, int studentsId, int id)
		{
			try
			{
				SubjectManagementService.SaveLabsVisitingData(
					new ScheduleProtectionLabMark(id, studentsId, comment, mark, dateId));

				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		//todo # need to fix parameters
		//[HttpPost("SaveLabsVisitingData")]
		//public IActionResult SaveLabsVisitingData(int dateId, List<string> marks, List<string> comments,
		//	List<int> studentsId, List<int> Id, List<StudentsViewData> students)
		//{
		//	try
		//	{
		//		var count = studentsId.Count;

		//		for (var i = 0; i < count; i++)
		//		{
		//			var currentMark = marks[i];
		//			var currentComment = comments[i];
		//			var currentStudentId = studentsId[i];
		//			var currentId = Id[i];

		//			foreach (var student in students)
		//				if (student.StudentId == currentStudentId)
		//					foreach (var labVisiting in student.LabVisitingMark)
		//						if (labVisiting.ScheduleProtectionLabId == dateId)
		//							SubjectManagementService.SaveLabsVisitingData(
		//								new ScheduleProtectionLabMark(currentId, currentStudentId, currentComment,
		//									currentMark, dateId));
		//		}

		//		return Ok();
		//	}
		//	catch (Exception ex)
		//	{
		//		return ServerError500(ex.Message);
		//	}
		//}

		[HttpPost("SaveStudentLabsMark")]
		public IActionResult SaveStudentLabsMark(int studentId, int labId, string mark, string comment, string date,
			int id, List<StudentsViewData> students)
		{
			try
			{
				SubjectManagementService.SaveStudentLabsMark(new StudentLabMark(labId, studentId, mark, comment, date,
					id));

				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpDelete("DeleteVisitingDate")]
		public IActionResult DeleteVisitingDate(string id)
		{
			try
			{
				SubjectManagementService.DeleteLabsVisitingDate(int.Parse(id));

				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpGet("GetFilesLab")]
		public IActionResult GetFilesLab(string userId, string subjectId, bool isCoursPrj = false)
		{
			try
			{
				var model = new List<UserlabFilesViewData>();
				var data = SubjectManagementService.GetUserLabFiles(int.Parse(userId), int.Parse(subjectId));
				model = data.Select(e => new UserlabFilesViewData
				{
					Comments = e.Comments,
					Id = e.Id,
					PathFile = e.Attachments,
					IsReceived = e.IsReceived,
					IsReturned = e.IsReturned,
					IsCoursProject = e.IsCoursProject,
					Date = e.Date != null ? e.Date.Value.ToString("dd.MM.yyyy HH:mm") : string.Empty,
					Attachments = FilesManagementService.GetAttachments(e.Attachments).ToList()
				}).Where(x => x.IsCoursProject == isCoursPrj).ToList();
				return Ok(model);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("SendFile")]
		public IActionResult SendFile(string subjectId, string userId, string id, string comments, string pathFile,
			string attachments, bool isCp = false, bool isRet = false)
		{
			try
			{
				var attachmentsModel = JsonSerializer.Deserialize<List<Attachment>>(attachments).ToList();

				SubjectManagementService.SaveUserLabFiles(new UserLabFiles
				{
					SubjectId = int.Parse(subjectId),
					Date = DateTime.Now,
					UserId = int.Parse(userId),
					Comments = comments,
					Attachments = pathFile,
					Id = int.Parse(id),
					IsCoursProject = isCp,
					IsReceived = false,
					IsReturned = isRet
				}, attachmentsModel);

				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpDelete("DeleteUserFile")]
		public IActionResult DeleteUserFile(string id)
		{
			try
			{
				SubjectManagementService.DeleteUserLabFile(int.Parse(id));
				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpGet("GetMarksV2")]
		public IActionResult GetMarksV2(int subjectId, int groupId)
		{
			try
			{
				var group = GroupManagementService.GetGroups(
					new Query<Group>(e => e.SubjectGroups.Any(x => x.SubjectId == subjectId && x.GroupId == groupId))
						.Include(e => e.Students.Select(x => x.StudentLabMarks))
						.Include(e => e.Students.Select(x => x.ScheduleProtectionLabMarks))
						.Include(e => e.Students.Select(x => x.User))).ToList()[0];

				var subGroups = SubjectManagementService.GetSubGroupsV2(subjectId, group.Id);

				IList<SubGroup> subGroupsWithSchedule = SubjectManagementService
					.GetSubGroupsV2WithScheduleProtectionLabs(subjectId, group.Id).ToList();

				var labsData = SubjectManagementService.GetSubject(subjectId).Labs.OrderBy(e => e.Order).ToList();

				var students = new List<StudentsViewData>();

				var controlTests = TestsManagementService.GetTestsForSubject(subjectId).Where(x => !x.ForSelfStudy);

				foreach (var student in group.Students.Where(e => e.Confirmed == null || e.Confirmed.Value)
					.OrderBy(e => e.LastName))
				{
					var scheduleProtectionLabs = subGroups.Any()
						? subGroups.FirstOrDefault(x => x.Name == "first").SubjectStudents
							.Any(x => x.StudentId == student.Id)
							? subGroupsWithSchedule.FirstOrDefault(x => x.Name == "first").ScheduleProtectionLabs
								.OrderBy(
									x => x.Date).ToList()
							: subGroups.FirstOrDefault(x => x.Name == "second").SubjectStudents
								.Any(x => x.StudentId == student.Id)
								? subGroupsWithSchedule.FirstOrDefault(x => x.Name == "second").ScheduleProtectionLabs
									.OrderBy(
										x => x.Date).ToList()
								: subGroups.FirstOrDefault(x => x.Name == "third").SubjectStudents
									.Any(x => x.StudentId == student.Id)
									? subGroupsWithSchedule.FirstOrDefault(x => x.Name == "third")
										.ScheduleProtectionLabs.OrderBy(
											x => x.Date).ToList()
									: new List<ScheduleProtectionLabs>()
						: new List<ScheduleProtectionLabs>();
					students.Add(new StudentsViewData(
						TestPassingService.GetStidentResults(subjectId, student.Id)
							.Where(x => controlTests.Any(y => y.Id == x.TestId)).ToList(), student,
						scheduleProtectionLabs, labs: labsData));
				}

				var result = students.Select(e => new StudentMark
				{
					FullName = e.FullName,
					Login = e.Login,
					SubGroup =
						subGroups.FirstOrDefault(x => x.Name == "first").SubjectStudents
							.Any(x => x.StudentId == e.StudentId) ? 1 :
						subGroups.FirstOrDefault(x => x.Name == "second").SubjectStudents
							.Any(x => x.StudentId == e.StudentId) ? 2 :
						subGroups.FirstOrDefault(x => x.Name == "third").SubjectStudents
							.Any(x => x.StudentId == e.StudentId) ? 3 : 4,
					StudentId = e.StudentId,
					LabsMarkTotal = e.LabsMarkTotal,
					TestMark = e.TestMark,
					LabVisitingMark = e.LabVisitingMark,
					Marks = e.StudentLabMarks
				}).ToList();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpGet("GetFilesV2")]
		public IActionResult GetFilesV2(int subjectId, int groupId, bool isCp)
		{
			try
			{
				var group = GroupManagementService.GetGroups(
					new Query<Group>(e => e.SubjectGroups.Any(x => x.SubjectId == subjectId && x.GroupId == groupId))
						.Include(e => e.Students.Select(x => x.User))).ToList()[0];
				var subGroups = SubjectManagementService.GetSubGroupsV2(subjectId, group.Id);
				var students = new List<StudentMark>();

				foreach (var student in group.Students.Where(e => e.Confirmed == null || e.Confirmed.Value)
					.OrderBy(e => e.LastName))
				{
					var files =
						SubjectManagementService.GetUserLabFiles(student.Id, subjectId).Select(
							t =>
								new UserlabFilesViewData
								{
									Comments = t.Comments,
									Date = t.Date != null ? t.Date.Value.ToString("dd.MM.yyyy HH:mm") : string.Empty,
									Id = t.Id,
									PathFile = t.Attachments,
									IsReceived = t.IsReceived,
									IsReturned = t.IsReturned,
									IsCoursProject = t.IsCoursProject,
									Attachments = FilesManagementService.GetAttachments(t.Attachments).ToList()
								}).Where(x => x.IsCoursProject == isCp).ToList();
					students.Add(new StudentMark
					{
						StudentId = student.Id,
						FullName = student.FullName,
						SubGroup =
							subGroups.FirstOrDefault(x => x.Name == "first").SubjectStudents
								.Any(x => x.StudentId == student.Id) ? 1 :
							subGroups.FirstOrDefault(x => x.Name == "second").SubjectStudents
								.Any(x => x.StudentId == student.Id) ? 2 :
							subGroups.FirstOrDefault(x => x.Name == "third").SubjectStudents
								.Any(x => x.StudentId == student.Id) ? 3 : 4,
						FileLabs = files
					});
				}

				return Ok(students);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpGet("GetLabsV2")]
		public IActionResult GetLabsV2(string subjectId, int groupId)
		{
			try
			{
				var id = int.Parse(subjectId);
				var labs = SubjectManagementService.GetLabsV2(id).OrderBy(e => e.Order);

				var subGroups = SubjectManagementService.GetSubGroupsV2WithScheduleProtectionLabs(id, groupId);

				var labsSubOne = labs.Select(e => new LabsViewData
				{
					Theme = e.Theme,
					Order = e.Order,
					Duration = e.Duration,
					ShortName = e.ShortName,
					LabId = e.Id,
					SubjectId = e.SubjectId,
					SubGroup = 1,
					ScheduleProtectionLabsRecomend = subGroups.Any()
						? subGroups.FirstOrDefault(x => x.Name == "first").ScheduleProtectionLabs.OrderBy(x => x.Date)
							.Select(x => new ScheduleProtectionLab {ScheduleProtectionId = x.Id, Mark = string.Empty})
							.ToList()
						: new List<ScheduleProtectionLab>()
				}).ToList();


				var durationCount = 0;

				foreach (var lab in labsSubOne)
				{
					var mark = 10;
					durationCount += lab.Duration / 2;
					for (var i = 0; i < lab.ScheduleProtectionLabsRecomend.Count; i++)
						if (i + 1 > durationCount - lab.Duration / 2)
						{
							lab.ScheduleProtectionLabsRecomend[i].Mark = mark.ToString(CultureInfo.InvariantCulture);

							if (i + 1 >= durationCount)
								if (mark != 1)
									mark -= 1;
						}
				}

				var labsSubTwo = labs.Select(e => new LabsViewData
				{
					Theme = e.Theme,
					Order = e.Order,
					Duration = e.Duration,
					ShortName = e.ShortName,
					LabId = e.Id,
					SubjectId = e.SubjectId,
					SubGroup = 2,
					ScheduleProtectionLabsRecomend = subGroups.Any()
						? subGroups.FirstOrDefault(x => x.Name == "second").ScheduleProtectionLabs.OrderBy(x => x.Date)
							.Select(x => new ScheduleProtectionLab {ScheduleProtectionId = x.Id, Mark = string.Empty})
							.ToList()
						: new List<ScheduleProtectionLab>()
				}).ToList();

				durationCount = 0;
				foreach (var lab in labsSubTwo)
				{
					var mark = 10;
					durationCount += lab.Duration / 2;
					for (var i = 0; i < lab.ScheduleProtectionLabsRecomend.Count; i++)
						if (i + 1 > durationCount - lab.Duration / 2)
						{
							lab.ScheduleProtectionLabsRecomend[i].Mark = mark.ToString(CultureInfo.InvariantCulture);

							if (i + 1 >= durationCount)
								if (mark != 1)
									mark -= 1;
						}
				}

				var labsSubThird = labs.Select(e => new LabsViewData
				{
					Theme = e.Theme,
					Order = e.Order,
					Duration = e.Duration,
					ShortName = e.ShortName,
					LabId = e.Id,
					SubjectId = e.SubjectId,
					SubGroup = 3,
					ScheduleProtectionLabsRecomend = subGroups.Any()
						? subGroups.FirstOrDefault(x => x.Name == "third").ScheduleProtectionLabs.OrderBy(x => x.Date)
							.Select(x => new ScheduleProtectionLab {ScheduleProtectionId = x.Id, Mark = string.Empty})
							.ToList()
						: new List<ScheduleProtectionLab>()
				}).ToList();

				durationCount = 0;
				foreach (var lab in labsSubThird)
				{
					var mark = 10;
					durationCount += lab.Duration / 2;
					for (var i = 0; i < lab.ScheduleProtectionLabsRecomend.Count; i++)
						if (i + 1 > durationCount - lab.Duration / 2)
						{
							lab.ScheduleProtectionLabsRecomend[i].Mark = mark.ToString(CultureInfo.InvariantCulture);

							if (i + 1 >= durationCount)
								if (mark != 1)
									mark -= 1;
						}
				}

				labsSubOne.AddRange(labsSubTwo);
				labsSubOne.AddRange(labsSubThird);

				var scheduleProtectionLabsOne =
					subGroups.FirstOrDefault() != null
						? subGroups.FirstOrDefault(e => e.Name == "first").ScheduleProtectionLabs.OrderBy(e => e.Date)
							.Select(
								e => new ScheduleProtectionLabsViewData(e)).ToList()
						: new List<ScheduleProtectionLabsViewData>();

				scheduleProtectionLabsOne.ForEach(e => e.SubGroup = 1);

				var scheduleProtectionLabsTwo =
					subGroups.LastOrDefault() != null
						? subGroups.FirstOrDefault(e => e.Name == "second").ScheduleProtectionLabs.OrderBy(e => e.Date)
							.Select(
								e => new ScheduleProtectionLabsViewData(e)).ToList()
						: new List<ScheduleProtectionLabsViewData>();

				scheduleProtectionLabsTwo.ForEach(e => e.SubGroup = 2);

				var scheduleProtectionLabsThird =
					subGroups.LastOrDefault() != null
						? subGroups.FirstOrDefault(e => e.Name == "third").ScheduleProtectionLabs.OrderBy(e => e.Date)
							.Select(
								e => new ScheduleProtectionLabsViewData(e)).ToList()
						: new List<ScheduleProtectionLabsViewData>();

				scheduleProtectionLabsThird.ForEach(e => e.SubGroup = 3);

				scheduleProtectionLabsOne.AddRange(scheduleProtectionLabsTwo);

				scheduleProtectionLabsOne.AddRange(scheduleProtectionLabsThird);

				var result = new
				{
					Labs = labsSubOne,
					ScheduleProtectionLabs = scheduleProtectionLabsOne
				};

				return Ok(result);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("ReceivedLabFile")]
		public IActionResult ReceivedLabFile(string userFileId)
		{
			try
			{
				SubjectManagementService.UpdateUserLabFile(userFileId, true);
				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("CancelReceivedLabFile")]
		public IActionResult CancelReceivedLabFile(string userFileId)
		{
			try
			{
				SubjectManagementService.UpdateUserLabFile(userFileId, false);
				return Ok();
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("CheckPlagiarismSubjects")]
		public IActionResult CheckPlagiarismSubjects(string subjectId, string type, string threshold)
		{
			try
			{
				var path = Guid.NewGuid().ToString("N");

				var subjectName = SubjectManagementService.GetSubject(int.Parse(subjectId)).ShortName;

				Directory.CreateDirectory(PlagiarismTempPath + path);

				var usersFiles = SubjectManagementService.GetUserLabFiles(0, int.Parse(subjectId))
					.Where(e => e.IsReceived);

				var filesPaths = usersFiles.Select(e => e.Attachments);

				foreach (var filesPath in filesPaths)
					if (Directory.Exists(FileUploadPath + filesPath))
						foreach (var srcPath in Directory.GetFiles(FileUploadPath + filesPath))
							System.IO.File.Copy(srcPath,
								srcPath.Replace(FileUploadPath + filesPath, PlagiarismTempPath + path), true);

				var plagiarismController = new PlagiarismController();
				var result = plagiarismController.CheckByDirectory(new[] {PlagiarismTempPath + path}.ToList(),
					int.Parse(threshold), 10, int.Parse(type));

				var data = new ResultPlagSubjectClu
				{
					clusters = new ResultPlagSubject[result.Count]
				};

				for (var i = 0; i < result.Count; ++i)
				{
					data.clusters[i] = new ResultPlagSubject
					{
						correctDocs = new List<ResultPlag>()
					};
					foreach (var doc in result[i].Docs)
					{
						var resultS = new ResultPlag();
						var fileName = Path.GetFileName(doc);
						var name = FilesManagementService.GetFileDisplayName(fileName);
						resultS.subjectName = subjectName;
						resultS.doc = name;
						var pathName = FilesManagementService.GetPathName(fileName);

						var userFileT = SubjectManagementService.GetUserLabFile(pathName);

						var userId = userFileT.UserId;

						var user = StudentManagementService.GetStudent(userId);

						resultS.author = user.FullName;

						resultS.groupName = user.Group.Name;
						data.clusters[i].correctDocs.Add(resultS);
					}
				}

				var resultData = data.clusters.ToList();
				return Ok(resultData);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}

		[HttpPost("CheckPlagiarism")]
		public IActionResult CheckPlagiarism(string userFileId, string subjectId, bool isCp = false)
		{
			try
			{
				var path = Guid.NewGuid().ToString("N");

				var subjectName = SubjectManagementService.GetSubject(int.Parse(subjectId)).ShortName;

				Directory.CreateDirectory(PlagiarismTempPath + path);

				var userFile = SubjectManagementService.GetUserLabFile(int.Parse(userFileId));

				var usersFiles = SubjectManagementService.GetUserLabFiles(0, int.Parse(subjectId))
					.Where(e => e.IsReceived && e.Id != userFile.Id && e.IsCoursProject == isCp);

				var filesPaths = usersFiles.Select(e => e.Attachments);

				foreach (var filesPath in filesPaths)
				foreach (var srcPath in Directory.GetFiles(FileUploadPath + filesPath))
					System.IO.File.Copy(srcPath, srcPath.Replace(FileUploadPath + filesPath, PlagiarismTempPath + path),
						true);

				var firstFileName =
					Directory.GetFiles(FileUploadPath + userFile.Attachments)
						.Select(fi => fi)
						.FirstOrDefault();

				var plagiarismController = new PlagiarismController();
				var result = plagiarismController.CheckBySingleDoc(firstFileName,
					new[] {PlagiarismTempPath + path}.ToList(), 10, 10);

				var data = new List<ResultPlag>();

				foreach (var res in result)
				{
					var resPlag = new ResultPlag();

					var fileName = Path.GetFileName(res.Doc);

					var name = FilesManagementService.GetFileDisplayName(fileName);

					resPlag.doc = name;

					resPlag.subjectName = subjectName;

					resPlag.coeff = res.Coeff.ToString();

					var pathName = FilesManagementService.GetPathName(fileName);

					var userFileT = SubjectManagementService.GetUserLabFile(pathName);

					var userId = userFileT.UserId;

					var user = StudentManagementService.GetStudent(userId);

					resPlag.author = user.FullName;

					resPlag.groupName = user.Group.Name;

					data.Add(resPlag);
				}

				var resultData = data.ToList();
				return Ok(resultData);
			}
			catch (Exception ex)
			{
				return ServerError500(ex.Message);
			}
		}
	}
}
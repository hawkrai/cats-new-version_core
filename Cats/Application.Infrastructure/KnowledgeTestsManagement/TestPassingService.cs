﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Core.Data;
using LMP.Data;
using LMP.Models;
using LMP.Models.KnowledgeTesting;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.KnowledgeTestsManagement
{
	public class TestPassingService : ITestPassingService
	{
		public NextQuestionResult GetNextQuestion(int testId, int userId, int nextQuestionNumber)
		{
			GheckForTestUnlockedForUser(testId, userId);

			var testAnswers = GetAnswersForTest(testId, userId);

			var questionsStatuses = GetQuestionStatuses(testAnswers);
			var nextQuestion = GetQuestion(testAnswers, nextQuestionNumber, userId);

			var result = new NextQuestionResult
			{
				Question = nextQuestion.Item1,
				Number = nextQuestion.Item1 == null ? 0 : nextQuestion.Item2,
				QuestionsStatuses = questionsStatuses
			};

			if (nextQuestion.Item1 == null)
			{
				result.Mark = nextQuestion.Item2;
				result.Percent = nextQuestion.Item3;
			}

			if (nextQuestion.Item1 != null) result.Seconds = GetRemainingTime(testId, nextQuestion.Item1.Id, userId);

			var test = GetTest(testId);

			result.SetTimeForAllTest = test.SetTimeForAllTest;
			result.ForSelfStudy = test.ForSelfStudy;

			return result;
		}

		public IEnumerable<RealTimePassingResult> GetRealTimePassingResults(int subjectId)
		{
			IEnumerable<TestUnlock> unockResults;
			var results = new List<RealTimePassingResult>();

			var testIds = GetTestsForSubject(subjectId).Select(test => test.Id);

			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				unockResults =
					repositoriesContainer.TestUnlocksRepository.GetAll(new Query<TestUnlock>(
								testUnlock => testIds.Contains(testUnlock.TestId))
							.Include(testUnlock => testUnlock.Student.User.UserAnswersOnTestQuestions))
						.Include(testUnlock => testUnlock.Test)
						.ToList();
			}

			foreach (var unockResult in unockResults)
				results.Add(new RealTimePassingResult
				{
					StudentName = unockResult.Student.FullName,
					PassResults = GetControlItems(unockResult.TestId, unockResult.StudentId).ToList(),
					TestName = unockResult.Test.Title
				});

			return results;
		}

		public IEnumerable<Test> GetTestsForSubject(int subjectId)
		{
			IEnumerable<Test> tests;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				tests = repositoriesContainer.TestsRepository.GetAll(
					new Query<Test>(
						test =>
							test.SubjectId == subjectId)).ToList();
			}

			return tests;
		}

		public TestPassResult GetTestPassingTime(int testId, int studentId)
		{
			var passingResult = GetTestPassResult(testId, studentId);
			return passingResult;
		}

		public List<TestPassResult> GetStidentResults(int subjectId, int studentId)
		{
			var tests = GetTestsForSubject(subjectId);
			var testIds = tests.Select(test => test.Id);
			List<TestPassResult> result;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				result =
					repositoriesContainer.RepositoryFor<TestPassResult>().GetAll(
							new Query<TestPassResult>(res =>
								testIds.Contains(res.TestId) && res.StudentId == studentId))
						.ToList();
			}

			foreach (var testPassResult in result)
				testPassResult.TestName = tests.Single(t => t.Id == testPassResult.TestId).Title;

			return result;
		}

		public void MakeUserAnswer(IEnumerable<Answer> answers, int userId, int testId, int questionNumber)
		{
			var test = GetTest(testId);
			var testPassResult = GetTestPassResult(testId, userId);

			CheckForTimeEndeed(userId, testId, test, testPassResult);

			var answerOnTestQuestion = GetAnswerOntestQuestion(userId, testId, questionNumber);

			if (answers == null)
			{
				answerOnTestQuestion.Points = 0;
			}
			else
			{
				var question = GetQuestionById(answerOnTestQuestion.QuestionId);

				switch (question.QuestionType)
				{
					case QuestionType.HasOneCorrectAnswer:
						ProcessOneVariantAnswer(answers, question, answerOnTestQuestion);
						break;
					case QuestionType.HasManyCorrectAnswers:
						ProcessManyVariantsAnswer(answers, question, answerOnTestQuestion);
						break;
					case QuestionType.TextAnswer:
						ProcessTextAnswer(answers, question, answerOnTestQuestion);
						break;
					case QuestionType.SequenceAnswer:
						ProcessSequenceAnswer(answers.ToList(), question, answerOnTestQuestion);
						break;
				}
			}

			answerOnTestQuestion.Time = DateTime.UtcNow;
			SaveAnswerOnTestQuestion(answerOnTestQuestion);
		}

		public Dictionary<int, double?> GetAverageMarkForTests(int groupId, int subjectId)
		{
			IEnumerable<Student> students;
			List<int> subjectTestIds;

			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				students = repositoriesContainer.StudentsRepository.GetAll(
						new Query<Student>(student => student.GroupId == groupId)
							.Include(student => student.User.TestPassResults))
					.ToList();

				subjectTestIds =
					repositoriesContainer.SubjectRepository.GetBy(
							new Query<Subject>(s => s.Id == subjectId).Include(s => s.SubjectTests))
						.SubjectTests.Select(st => st.Id)
						.ToList();
			}

			var results = students.ToDictionary(student => student.Id,
				student => GetAverage(student.User.TestPassResults.Where(pr => subjectTestIds.Contains(pr.TestId))
					.ToList()));

			return results;
		}

		public IEnumerable<Student> GetPassTestResults(int groupId, int subjectId)
		{
			IEnumerable<Student> students;
			List<int> subjectTestIds;

			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				students = repositoriesContainer.StudentsRepository.GetAll(
						new Query<Student>(student =>
								student.GroupId == groupId && (student.Confirmed == null || student.Confirmed.Value))
							.Include(student => student.User)
							.Include(student => student.User.TestPassResults))
					.ToList();

				subjectTestIds =
					repositoriesContainer.SubjectRepository.GetBy(
							new Query<Subject>(s => s.Id == subjectId).Include(s => s.SubjectTests))
						.SubjectTests.Select(st => st.Id)
						.ToList();
			}

			var testIds =
				students.SelectMany(student =>
						student.User.TestPassResults.Where(pr => subjectTestIds.Contains(pr.TestId))
							.Select(testResult => testResult.TestId))
					.Distinct()
					.ToArray();

			var studentResults = students.Select(rawStudent => new Student
			{
				Id = rawStudent.Id,
				FirstName = rawStudent.FirstName,
				LastName = rawStudent.LastName,
				MiddleName = rawStudent.MiddleName,
				User = new User
				{
					UserName = rawStudent.User.UserName,
					TestPassResults = GetTestPassResultsForStudent(testIds, rawStudent)
				}
			}).ToList();

			return studentResults;
		}

		public IEnumerable<Test> GetAvailableTestsForStudent(int studentId, int subjectId)
		{
			IEnumerable<Test> availableTests;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				availableTests = repositoriesContainer.TestsRepository.GetAll(
						new Query<Test>(
							test =>
								test.SubjectId == subjectId &&
								((test.ForNN || !test.ForEUMK && !test.BeforeEUMK && !test.ForSelfStudy) &&
								 test.TestUnlocks.Any(testUnlock => testUnlock.StudentId == studentId) ||
								 test.ForSelfStudy)))
					.ToList();
			}

			return availableTests;
		}

		public bool CheckForSubjectAvailableForStudent(int studentId, int subjectId)
		{
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				if (
					!repositoriesContainer.SubjectRepository.GetAll(
							new Query<Subject>(
								subject =>
									subject.Id == subjectId &&
									subject.SubjectGroups.Any(sg => sg.Group.Students.Any(st => st.Id == studentId))))
						.Any())
					return false;
			}

			return true;
		}

		/// <summary>
		///     Return records for current test or create
		/// </summary>
		public List<AnswerOnTestQuestion> GetAnswersForTest(int testId, int userId)
		{
			List<AnswerOnTestQuestion> testAnswers;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				var repository = repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>();
				testAnswers =
					repository.GetAll(
						new Query<AnswerOnTestQuestion>(
							testAnswer => testAnswer.TestId == testId && testAnswer.UserId == userId &&
							              !testAnswer.TestEnded)).ToList();
			}

			if (!testAnswers.Any())
			{
				StartNewTest(testId, userId);
				return GetAnswersForTest(testId, userId);
			}

			return testAnswers;
		}

		public List<AnswerOnTestQuestion> GetAnswersForEndedTest(int testId, int userId)
		{
			List<AnswerOnTestQuestion> testAnswers;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				var repository = repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>();
				testAnswers =
					repository.GetAll(
						new Query<AnswerOnTestQuestion>(
							testAnswer => testAnswer.TestId == testId && testAnswer.UserId == userId &&
							              testAnswer.TestEnded)).ToList();
			}

			return testAnswers;
		}

		public int? GetPointsForQuestion(int userId, int questionId)
		{
			int? result = null;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				var repository = repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>();

				var answerOnTestQuestion = repository.GetAll(
					new Query<AnswerOnTestQuestion>(
						testAnswer => testAnswer.QuestionId == questionId && testAnswer.UserId == userId &&
						              !testAnswer.TestEnded)).ToList();

				if (answerOnTestQuestion != null && answerOnTestQuestion.Any())
					result = answerOnTestQuestion.LastOrDefault().Points;
			}

			return result;
		}

		private int GetRemainingTime(int testId, int questionId, int userId)
		{
			var test = GetTest(testId);
			var testPassResult = GetTestPassResult(testId, userId);

			double seconds = 0;

			if (test.SetTimeForAllTest)
			{
				seconds = test.TimeForCompleting * 60 - (DateTime.UtcNow - testPassResult.StartTime).TotalSeconds;
			}
			else
			{
				if (testPassResult.Comment == questionId.ToString())
				{
					seconds = test.TimeForCompleting - (DateTime.UtcNow.Ticks - testPassResult.StartTime.Ticks) /
					          TimeSpan.TicksPerSecond;
				}
				else
				{
					seconds = test.TimeForCompleting;
					testPassResult.StartTime = DateTime.UtcNow;
					testPassResult.Comment = questionId.ToString();
				}

				using var repositoriesContainer = new LmPlatformRepositoriesContainer();
				repositoriesContainer.RepositoryFor<TestPassResult>().Save(testPassResult);
				repositoriesContainer.ApplyChanges();
			}

			return seconds > 0 ? (int) seconds : 0;
		}

		public IEnumerable<Test> GetTests()
		{
			IEnumerable<Test> tests;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				tests = repositoriesContainer.TestsRepository.GetAll().ToList();
			}

			return tests;
		}

		private void CheckForTimeEndeed(int userId, int testId, Test test, TestPassResult testPassResult)
		{
			if (test.SetTimeForAllTest &&
			    (DateTime.UtcNow - testPassResult.StartTime).Seconds > test.TimeForCompleting * 60)
			{
				var testAnswers = GetAnswersForTest(testId, userId);
				foreach (var answer in testAnswers)
					if (!answer.Time.HasValue)
					{
						answer.Time = DateTime.UtcNow;
						answer.Points = 0;
					}

				CloseTest(testAnswers, userId);
			}
		}

		private double? GetAverage(List<TestPassResult> list)
		{
			var resultsWithMarks = list.Where(item => item.Points.HasValue);
			if (!list.Any()) return null;

			return resultsWithMarks.Sum(item => (double) item.Points) / resultsWithMarks.Count();
		}

		private List<TestPassResult> GetTestPassResultsForStudent(int[] testIds, Student rawStudent)
		{
			var tests = GetTests();
			var testPassResults = new List<TestPassResult>();
			foreach (var testId in testIds)
			{
				var t = tests.SingleOrDefault(test => test.Id == testId);
				testPassResults.Add(new TestPassResult
				{
					StudentId = rawStudent.Id,
					TestId = testId,
					TestName = t != null ? t.Title : "Тест",
					Points = GetPoints(rawStudent, testId),
					Percent = GetPercent(rawStudent, testId)
				});
			}

			return testPassResults;
		}

		private int? GetPercent(Student rawStudent, int testId)
		{
			var passResult = rawStudent.User.TestPassResults.Where(result => result.TestId == testId);
			if (passResult.Count() == 1) return passResult.Single().Percent;

			return null;
		}

		private int? GetPoints(Student rawStudent, int testId)
		{
			var passResult = rawStudent.User.TestPassResults.Where(result => result.TestId == testId);
			if (passResult.Count() == 1) return passResult.Single().Points;

			if (passResult.Count() > 1) return passResult.Sum(result => result.Points);

			return null;
		}

		private void ProcessSequenceAnswer(List<Answer> answers, Question question,
			AnswerOnTestQuestion answerOntestQuestion)
		{
			var isCorrect = true;
			if (answers.Count() != question.Answers.Count)
				throw new InvalidDataException("Последовательность не совпадает с исходной");

			var plainAnswers = question.Answers.ToList();
			for (var i = 0; i < answers.Count(); i++) isCorrect = isCorrect && answers[i].Id == plainAnswers[i].Id;

			if (isCorrect) answerOntestQuestion.Points = question.ComlexityLevel;

			using var repositoriesContainer = new LmPlatformRepositoriesContainer();
			foreach (var userAnswerId in answers.Select(x => x.Id))
			{
				var answer = GetAnswerById(userAnswerId);
				answerOntestQuestion.AnswerString += answer.Content;
				answerOntestQuestion.AnswerString += "\n";
			}
		}

		private void ProcessTextAnswer(IEnumerable<Answer> userAnswers, Question question,
			AnswerOnTestQuestion answerOntestQuestion)
		{
			if (userAnswers.Count() != 1)
				throw new InvalidDataException("Пользователь должен указать 1 правильный ответ");

			if (userAnswers.Single().Content == null)
				throw new InvalidDataException("Пользователь должен указать ответ");

			if (question.Answers.Select(answer => answer.Content.ToLower())
				.Contains(userAnswers.Single().Content.ToLower()))
				answerOntestQuestion.Points = question.ComlexityLevel;

			answerOntestQuestion.AnswerString = userAnswers.Single().Content.ToLower();
		}

		private void ProcessManyVariantsAnswer(IEnumerable<Answer> userAnswers, Question question,
			AnswerOnTestQuestion answerOntestQuestion)
		{
			if (userAnswers.Count(answer => answer.СorrectnessIndicator > 0) == 0)
				throw new InvalidDataException("Пользователь должен указать хотя бы 1 правильный ответ");

			var correctAnswers = question.Answers.Where(answer => answer.СorrectnessIndicator > 0);

			var isCorrect = true;
			foreach (var correctAnswer in correctAnswers)
				isCorrect = isCorrect && userAnswers
					            .Where(answer => answer.СorrectnessIndicator > 0)
					            .Any(userAnswer => userAnswer.Id == correctAnswer.Id);

			isCorrect = isCorrect &&
			            userAnswers.Count(answer => answer.СorrectnessIndicator > 0) == correctAnswers.Count();

			if (isCorrect) answerOntestQuestion.Points = question.ComlexityLevel;

			using var repositoriesContainer = new LmPlatformRepositoriesContainer();
			foreach (var userAnswerId in userAnswers.Where(answer => answer.СorrectnessIndicator > 0)
				.Select(x => x.Id))
			{
				var answer = GetAnswerById(userAnswerId);
				answerOntestQuestion.AnswerString += answer.Content;
				answerOntestQuestion.AnswerString += "\n";
			}
		}

		private void ProcessOneVariantAnswer(IEnumerable<Answer> userAnswers, Question question,
			AnswerOnTestQuestion answerOntestQuestion)
		{
			if (userAnswers.Count(answer => answer.СorrectnessIndicator > 0) != 1)
				throw new InvalidDataException("Пользователь должен указать 1 правильный ответ");

			var correctAnswer = question.Answers.Single(answer => answer.СorrectnessIndicator > 0);

			if (correctAnswer.Id == userAnswers.Single(answer => answer.СorrectnessIndicator > 0).Id)
				answerOntestQuestion.Points = question.ComlexityLevel;

			using var repositoriesContainer = new LmPlatformRepositoriesContainer();
			{
				var answer = GetAnswerById(userAnswers.FirstOrDefault(y => y.СorrectnessIndicator > 0).Id);
				answerOntestQuestion.AnswerString = answer.Content;
			}
		}

		private void SaveAnswerOnTestQuestion(AnswerOnTestQuestion answerOnTestQuestion)
		{
			using var repositoriesContainer = new LmPlatformRepositoriesContainer();
			repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>().Save(answerOnTestQuestion);
			repositoriesContainer.ApplyChanges();
		}

		private AnswerOnTestQuestion GetAnswerOntestQuestion(int userId, int testId, int questionNumber)
		{
			AnswerOnTestQuestion answerOnTestQuestion;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				answerOnTestQuestion =
					repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>().GetBy(
						new Query<AnswerOnTestQuestion>(answer =>
							answer.UserId == userId && answer.TestId == testId && answer.Number == questionNumber &&
							!answer.TestEnded));
			}

			return answerOnTestQuestion;
		}

		private Answer GetAnswerById(int id)
		{
			Answer answerResult;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				answerResult =
					repositoriesContainer.RepositoryFor<Answer>().GetBy(
						new Query<Answer>(answer => answer.Id == id));
			}

			return answerResult;
		}

		private Dictionary<int, PassedQuestionResult> GetQuestionStatuses(IEnumerable<AnswerOnTestQuestion> testAnswers)
		{
			return testAnswers.ToDictionary(question => question.Number, GetQuestionStatus);
		}

		private PassedQuestionResult GetQuestionStatus(AnswerOnTestQuestion answer)
		{
			if (!answer.Time.HasValue) return PassedQuestionResult.NotPassed;

			return answer.Points > 0
				? PassedQuestionResult.Success
				: PassedQuestionResult.Error;
		}

		private Tuple<Question, int, int> GetQuestion(IEnumerable<AnswerOnTestQuestion> testAnswers,
			int nextQuestionNumber, int userId)
		{
			var notPassedQuestions = testAnswers.Where(testAnswer => !testAnswer.Time.HasValue).ToList();
			if (notPassedQuestions.Any())
			{
				var nextQuestion = GetNextQuestionsFromNotPassedItems(notPassedQuestions, nextQuestionNumber);
				return nextQuestion;
			}

			var mark = CloseTest(testAnswers, userId);
			return new Tuple<Question, int, int>(null, mark.Item1, mark.Item2);
		}

		private Tuple<int, int> CloseTest(IEnumerable<AnswerOnTestQuestion> testAnswers, int userId)
		{
			var testId = testAnswers.First().TestId;
			var testPassResult = GetTestPassResult(testId, userId);

			var points = GetResultPoints(testAnswers);
			var percent = GetPoints(testAnswers);
			testPassResult.Points = points;
			testPassResult.Percent = percent;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				foreach (var answer in testAnswers)
				{
					answer.TestEnded = true;
					repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>().Save(answer);
				}

				repositoriesContainer.RepositoryFor<TestPassResult>().Save(testPassResult);

				var savedTestUnlock = repositoriesContainer.TestUnlocksRepository.GetAll(new
							Query<TestUnlock>()
						.AddFilterClause(testUnlock => testUnlock.StudentId == userId && testUnlock.TestId == testId))
					.SingleOrDefault();

				if (!GetTest(testId).ForSelfStudy && savedTestUnlock != null)
					repositoriesContainer.TestUnlocksRepository.Delete(savedTestUnlock);

				repositoriesContainer.ApplyChanges();
			}

			return new Tuple<int, int>(points, percent);
		}

		private TestPassResult GetTestPassResult(int testId, int userId)
		{
			TestPassResult result;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				result =
					repositoriesContainer.RepositoryFor<TestPassResult>().GetBy(
						new Query<TestPassResult>(res => res.TestId == testId && res.StudentId == userId));
			}

			return result;
		}

		private int GetResultPoints(IEnumerable<AnswerOnTestQuestion> testAnswers)
		{
			var test = GetTest(testAnswers.First().TestId);
			var result = testAnswers.Sum(testAnswer => testAnswer.Points)
			             / (double) test.Questions.Where(q => testAnswers.Select(a => a.QuestionId).Contains(q.Id))
				             .Sum(question => question.ComlexityLevel) * 10;

			return (int) Math.Round(result);
		}

		private int GetPoints(IEnumerable<AnswerOnTestQuestion> testAnswers)
		{
			var test = GetTest(testAnswers.First().TestId);
			var result = testAnswers.Sum(testAnswer => testAnswer.Points)
			             / (double) test.Questions.Where(q => testAnswers.Select(a => a.QuestionId).Contains(q.Id))
				             .Sum(question => question.ComlexityLevel) * 100;

			return (int) result;
		}

		private Tuple<Question, int, int> GetNextQuestionsFromNotPassedItems(
			List<AnswerOnTestQuestion> notPassedQuestions, int nextQuestionNumber)
		{
			int questionId;
			if (notPassedQuestions.Any(question => question.Number == nextQuestionNumber))
			{
				questionId = notPassedQuestions
					.Single(question => question.Number == nextQuestionNumber)
					.QuestionId;
			}
			else
			{
				var orderedAnswers = notPassedQuestions.OrderBy(question => question.Number)
					.SkipWhile(question => question.Number < nextQuestionNumber).ToList();
				orderedAnswers.AddRange(notPassedQuestions.OrderBy(question => question.Number)
					.TakeWhile(question => question.Number < nextQuestionNumber));

				var answerOnTestQuestion = orderedAnswers.First();

				nextQuestionNumber = answerOnTestQuestion.Number;

				questionId = answerOnTestQuestion.QuestionId;
			}

			var resultQuestion = GetQuestionById(questionId);
			if (resultQuestion.QuestionType == QuestionType.TextAnswer)
			{
				resultQuestion.Answers = resultQuestion.Answers.Take(1).ToList();
			}
			else if (resultQuestion.QuestionType == QuestionType.SequenceAnswer)
			{
				var indicator = 0;
				foreach (var answer in resultQuestion.Answers) answer.СorrectnessIndicator = indicator++;
			}

			var random = new Random();
			resultQuestion.Answers = resultQuestion.Answers.OrderBy(a => random.Next()).ToList();

			return new Tuple<Question, int, int>(resultQuestion, nextQuestionNumber, 0);
		}

		private Question GetQuestionById(int id)
		{
			Question queston;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				queston =
					repositoriesContainer.QuestionsRepository.GetBy(
						new Query<Question>(question => question.Id == id)
							.Include(question => question.Answers));
			}

			return queston;
		}

		/// <summary>
		///     Return records for current test or create
		/// </summary>
		private IEnumerable<PassedQuestionResult> GetControlItems(int testId, int userId)
		{
			List<AnswerOnTestQuestion> testAnswers;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				var repository = repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>();
				testAnswers =
					repository.GetAll(
						new Query<AnswerOnTestQuestion>(
							testAnswer => testAnswer.TestId == testId && testAnswer.UserId == userId &&
							              !testAnswer.TestEnded)).ToList();
			}

			if (!testAnswers.Any()) return new PassedQuestionResult[0];

			return testAnswers.Select(GetQuestionStatus);
		}

		private void StartNewTest(int testId, int userId)
		{
			var test = GetTest(testId);

			var questionsCount = test.CountOfQuestions > test.Questions.Count
				? test.Questions.Count
				: test.CountOfQuestions;
			IEnumerable<Question> includedQuestions = null;

			if (test.ForNN)
			{
				includedQuestions = test.Questions.OrderBy(t => t.ConceptId).ThenBy(a => a.Id).ToList();
			}
			else
			{
				var random = new Random(DateTime.Now.Millisecond);
				includedQuestions = test.Questions.OrderBy(t => random.Next()).Take(questionsCount);
			}

			var answersTemplate = new List<AnswerOnTestQuestion>();

			var counter = 1;
			foreach (var includedQuestion in includedQuestions)
			{
				answersTemplate.Add(new AnswerOnTestQuestion
				{
					QuestionId = includedQuestion.Id,
					TestId = testId,
					UserId = userId,
					Number = counter++
				});
			}

			var testPassResult = GetTestPassResult(testId, userId) ?? new TestPassResult
			{
				TestId = testId,
				StudentId = userId
			};

			testPassResult.StartTime = DateTime.UtcNow;

			using var repositoriesContainer = new LmPlatformRepositoriesContainer();
			var toDelete = repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>()
				.GetAll(new Query<AnswerOnTestQuestion>(x => x.TestId == testId && x.UserId == userId));
			repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>().Delete(toDelete);
			repositoriesContainer.RepositoryFor<AnswerOnTestQuestion>().Save(answersTemplate);
			repositoriesContainer.RepositoryFor<TestPassResult>().Save(testPassResult);
			repositoriesContainer.ApplyChanges();
		}

		private static Test GetTest(int testId)
		{
			Test testResult;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				testResult = repositoriesContainer.TestsRepository.GetBy(new Query<Test>(test => test.Id == testId)
					.Include(test => test.Questions));
			}

			return testResult;
		}

		private void GheckForTestUnlockedForUser(int testId, int userId)
		{
			if (IsTestLockedForUser(testId, userId))
				throw new AccessViolationException("Тест недоступен для текущего пользователя");
		}

		/// <summary>
		///     Returns if test locked for student or user is lecturer
		/// </summary>
		private bool IsTestLockedForUser(int testId, int userId)
		{
			bool isTestLockedForUser;
			using (var repositoriesContainer = new LmPlatformRepositoriesContainer())
			{
				var testObject =
					repositoriesContainer.TestsRepository.GetBy(new Query<Test>(test => test.Id == testId));
				if (testObject != null && (testObject.ForSelfStudy || testObject.BeforeEUMK))
				{
					isTestLockedForUser = false;
				}
				else
				{
					var userResult = repositoriesContainer.UsersRepository.GetBy(
						new Query<User>(user => user.Id == userId)
							.Include(user => user.Lecturer));
					if (userResult.Lecturer != null)
						isTestLockedForUser = false;
					else
						isTestLockedForUser = !repositoriesContainer.TestUnlocksRepository.GetAll(
								new Query<TestUnlock>()
									.AddFilterClause(testUnlock => testUnlock.StudentId == userId)
									.AddFilterClause(testUnlock => testUnlock.TestId == testId))
							.Any();
				}
			}

			return isTestLockedForUser;
		}
	}
}
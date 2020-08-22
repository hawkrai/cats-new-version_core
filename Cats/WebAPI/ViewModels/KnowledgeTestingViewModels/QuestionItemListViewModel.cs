using System.ComponentModel;
using Application.Core.UI.HtmlHelpers;
using LMP.Models.KnowledgeTesting;
using Microsoft.AspNetCore.Html;

namespace WebAPI.ViewModels.KnowledgeTestingViewModels
{
    public class QuestionItemListViewModel : BaseNumberedGridItem
    {
        [DisplayName("Вопрос")] 
        public string Title { get; set; }

        [DisplayName("Действия")] 
        public HtmlString Action { get; set; }

        public int Id { get; set; }

        public bool Selected { get; set; }

        public int ComlexityLevel { get; set; }

        public int? QuestionNumber { get; set; }

        public int? ConceptId { get; set; }

        public static QuestionItemListViewModel FromQuestion(Question question, string htmlString)
        {
            var model = FromQuestion(question);
            model.Action = new HtmlString(htmlString);
            model.QuestionNumber = question.QuestionNumber;
            return model;
        }

        public static QuestionItemListViewModel FromQuestion(Question question)
        {
            return new QuestionItemListViewModel
            {
                Id = question.Id,
                Title = question.Title,
                QuestionNumber = question.QuestionNumber,
                ComlexityLevel = question.ComlexityLevel,
                ConceptId = question.ConceptId
            };
        }
    }
}
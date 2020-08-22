using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAPI.Attributes
{
    public class SubjectNameAttribute : ValidationAttribute //todo #, IClientValidatable
	{
        //todo #
        #region todo

        //private readonly LazyDependency<ISubjectManagementService> subjectManagementService = new LazyDependency<ISubjectManagementService>();

        //public ISubjectManagementService SubjectManagementService
        //{
        //	get
        //	{
        //		return subjectManagementService.Value;
        //	}
        //}

        //private const string ErrorMsg = "Предмет с таким именем уже существует";

        //private String PropertyName { get; set; }
        //private Object DesiredValue { get; set; }

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //	return ValidationResult.Success;
        //}

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //	var rule = new ModelClientValidationRule
        //	{
        //		ErrorMessage = ErrorMsg,
        //		ValidationType = "subjectname",
        //	};

        //	yield return rule;
        //}

        #endregion
	}

	public class SubjectShortNameAttribute : ValidationAttribute// todo # , IClientValidatable
	{
        //todo #
        #region todo

        //private readonly LazyDependency<ISubjectManagementService> subjectManagementService = new LazyDependency<ISubjectManagementService>();

        //public ISubjectManagementService SubjectManagementService
        //{
        //	get
        //	{
        //		return subjectManagementService.Value;
        //	}
        //}

        //private const string ErrorMsg = "Предмет с такой аббревиатурой уже существует";

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //	return ValidationResult.Success;
        //}

        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //	var rule = new ModelClientValidationRule
        //	{
        //		ErrorMessage = ErrorMsg,
        //		ValidationType = "subjectshortname",
        //	};

        //	yield return rule;
        //}

        #endregion
	}
}
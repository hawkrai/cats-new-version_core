using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Application.Core.Data
{
    public class Query<TModel> : IQuery<TModel>
    {
        public Query()
        {
        }

        public Query(params Expression<Func<TModel, bool>>[] filters)
            : this()
        {
	        foreach (var expression in filters)
	        {
				Filters.Add(expression);
	        }
        }

        public IList<Expression<Func<TModel, bool>>> Filters { get; } = new List<Expression<Func<TModel, bool>>>();

        public IList<string> IncludedProperties { get; } = new List<string>();

        public virtual IQuery<TModel> AddFilterClause(Expression<Func<TModel, bool>> filterClause)
        {
            Filters.Add(filterClause);

            return this;
        }

        public IQuery<TModel> Include<TResult>(Expression<Func<TModel, TResult>> includeExpression)
        {
            if (TryParsePath(includeExpression.Body, out var result))
            {
                IncludedProperties.Add(result);
            }

            return this;
        }

        protected static bool TryParsePath(Expression expression, out string path)
        {
            path = null;
            Expression expression2 = RemoveConvert(expression);
            var memberExpression = expression2 as MemberExpression;
            var methodCallExpression = expression2 as MethodCallExpression;
            if (memberExpression is { })
            {
                string name = memberExpression.Member.Name;
                if (!TryParsePath(memberExpression.Expression, out var text))
                {
                    return false;
                }

                path = text is null ? name : (text + "." + name);
            }
            else
            {
                if (methodCallExpression is { })
                { 
                    if (methodCallExpression.Method.Name == "Select" && methodCallExpression.Arguments.Count == 2)
                    {
                        if (!TryParsePath(methodCallExpression.Arguments[0], out var text2))
                        {
                            return false;
                        }

                        if (text2 is { })
                        {
                            if (methodCallExpression.Arguments[1] is LambdaExpression lambdaExpression)
                            {
                                if (!TryParsePath(lambdaExpression.Body, out var text3))
                                {
                                    return false;
                                }

                                if (text3 is { })
                                {
                                    path = text2 + "." + text3;
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        protected static Expression RemoveConvert(Expression expression)
        {
            while (expression is { } && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = RemoveConvert(((UnaryExpression)expression).Operand);
            }

            return expression;
        }
    }
}

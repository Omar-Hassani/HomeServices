using System.Linq.Expressions;

namespace HomeServices.Infrastructure.Data;

/// <summary>
/// دمج شروط LINQ ديناميكياً بطريقة يفهمها EF Core (باستبدال المعامل بدل Expression.Invoke).
/// </summary>
internal static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = left.Parameters[0];
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, rightBody), parameter);
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : base.VisitParameter(node);
    }
}

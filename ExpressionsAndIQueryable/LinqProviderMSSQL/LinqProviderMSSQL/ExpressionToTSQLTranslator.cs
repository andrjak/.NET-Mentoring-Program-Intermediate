using System.Linq.Expressions;
using System.Text;

namespace LinqProviderMSSQL;

public class ExpressionToTSQLTranslator : ExpressionVisitor
{
    readonly StringBuilder _resultStringBuilder;

    public ExpressionToTSQLTranslator()
    {
        _resultStringBuilder = new StringBuilder();
    }

    public string Translate(Expression exp)
    {
        Visit(exp);

        return _resultStringBuilder.ToString();
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.DeclaringType == typeof(Queryable)
            && node.Method.Name == "Where")
        {
            _resultStringBuilder.Append("WHERE ");
            var predicate = node.Arguments[1];
            Visit(predicate);

            return node;
        }

        if (node.Method.DeclaringType == typeof(string)
            && node.Method.Name == "StartsWith")
        {
            Visit(node.Object);

            var param = node.Arguments[0] as ConstantExpression;
            if (param is null)
            {
                throw new NotSupportedException("Only constant");
            }

            _resultStringBuilder.Append(" LIKE ");
            _resultStringBuilder.Append("'");
            _resultStringBuilder.Append(param.Value);
            _resultStringBuilder.Append("%'");

            return node;
        }

        if (node.Method.DeclaringType == typeof(string)
            && node.Method.Name == "Contains")
        {
            Visit(node.Object);

            var param = node.Arguments[0] as ConstantExpression;
            if (param is null)
            {
                throw new NotSupportedException("Only constant");
            }

            _resultStringBuilder.Append(" LIKE ");
            _resultStringBuilder.Append("'%");
            _resultStringBuilder.Append(param.Value);
            _resultStringBuilder.Append("%'");

            return node;
        }

        if (node.Method.DeclaringType == typeof(string)
            && node.Method.Name == "EndsWith")
        {
            Visit(node.Object);

            var param = node.Arguments[0] as ConstantExpression;
            if (param is null)
            {
                throw new NotSupportedException("Only constant");
            }

            _resultStringBuilder.Append(" LIKE ");
            _resultStringBuilder.Append("'%");
            _resultStringBuilder.Append(param.Value);
            _resultStringBuilder.Append("'");

            return node;
        }

        return base.VisitMethodCall(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        switch (node.NodeType)
        {
            case ExpressionType.Equal:
                if (node.Left.NodeType == ExpressionType.MemberAccess
                    && node.Right.NodeType == ExpressionType.Constant)
                {
                    Visit(node.Left);
                    _resultStringBuilder.Append(" IN ");
                    _resultStringBuilder.Append('(');
                    Visit(node.Right);
                    _resultStringBuilder.Append(')');
                    break;
                }
                if (node.Left.NodeType == ExpressionType.Constant
                    && node.Right.NodeType == ExpressionType.MemberAccess)
                {
                    Visit(node.Right);
                    _resultStringBuilder.Append(" IN ");
                    _resultStringBuilder.Append('(');
                    Visit(node.Left);
                    _resultStringBuilder.Append(')');
                    break;
                }

                throw new NotSupportedException($"Left or Right operand should be property or filed and Left or Right operator should be constant. Both operator shouldn't be same type.");

            case ExpressionType.GreaterThan:
                if (node.Left.NodeType == ExpressionType.MemberAccess
                    && node.Right.NodeType == ExpressionType.Constant 
                    || node.Left.NodeType == ExpressionType.Constant
                    && node.Right.NodeType == ExpressionType.MemberAccess)
                {
                    Visit(node.Left);
                    _resultStringBuilder.Append(" > ");
                    Visit(node.Right);
                    break;
                }

                throw new NotSupportedException($"Left or Right operand should be property or filed and Left or Right operator should be constant. Both operator shouldn't be same type.");

            case ExpressionType.LessThan:
                if (node.Left.NodeType == ExpressionType.MemberAccess
                    && node.Right.NodeType == ExpressionType.Constant
                    || node.Left.NodeType == ExpressionType.Constant
                    && node.Right.NodeType == ExpressionType.MemberAccess)
                {
                    Visit(node.Left);
                    _resultStringBuilder.Append(" < ");
                    Visit(node.Right);
                    break;
                }

                throw new NotSupportedException($"Left or Right operand should be property or filed and Left or Right operator should be constant. Both operator shouldn't be same type.");

            case ExpressionType.AndAlso:
                Visit(node.Left);
                _resultStringBuilder.Append(" AND ");
                Visit(node.Right);
                break;

            case ExpressionType.OrElse:
                Visit(node.Left);
                _resultStringBuilder.Append(" OR ");
                Visit(node.Right);
                break;

                throw new NotSupportedException($"Left or Right operand should be property or filed and Left or Right operator should be constant. Both operator shouldn't be same type.");

            default:
                throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
        };

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        _resultStringBuilder
            .Append('[')
            .Append(node.Member.Name)
            .Append(']');

        return base.VisitMember(node);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Type == typeof(string))
        {
            _resultStringBuilder
                .Append('\'')
                .Append(node.Value)
                .Append('\'');

            return node;
        }

        _resultStringBuilder.Append(node.Value);

        return node;
    }
}

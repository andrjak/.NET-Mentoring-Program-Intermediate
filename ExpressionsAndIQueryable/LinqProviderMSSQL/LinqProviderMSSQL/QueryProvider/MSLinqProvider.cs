using LinqProviderMSSQL.Helpers;
using LinqProviderMSSQL.Services;
using System.Linq.Expressions;

namespace LinqProviderMSSQL.QueryProvider;

public class MSLinqProvider(MSDataService dataService) : IQueryProvider
{
    private readonly MSDataService _dataService = dataService;

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new MSQuery<TElement>(expression, this); 
    }

    public object? Execute(Expression expression)
    {
        throw new NotImplementedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        Type? itemType = TypeHelper.GetElementType(expression.Type);

        var translator = new ExpressionToTSQLTranslator();
        string queryString = translator.Translate(expression);

        return (TResult)_dataService.ReadSQL(itemType, queryString).Result;
    }
}

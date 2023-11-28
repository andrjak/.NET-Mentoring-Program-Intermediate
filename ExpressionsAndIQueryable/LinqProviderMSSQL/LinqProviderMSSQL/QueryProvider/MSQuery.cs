using System.Collections;
using System.Linq.Expressions;

namespace LinqProviderMSSQL.QueryProvider;

public class MSQuery<T>(Expression expression, MSLinqProvider provider) : IQueryable<T>
{
    private readonly MSLinqProvider _provider = provider;

    public Expression Expression { get; } = expression;

    public Type ElementType => typeof(T);

    public IQueryProvider Provider => _provider;

    public IEnumerator<T> GetEnumerator()
    {
        return _provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _provider.Execute<IEnumerable>(Expression).GetEnumerator();
    }
}

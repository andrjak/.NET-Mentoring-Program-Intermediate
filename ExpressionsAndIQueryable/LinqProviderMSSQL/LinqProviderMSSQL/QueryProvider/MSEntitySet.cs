using LinqProviderMSSQL.Services;
using System.Collections;
using System.Linq.Expressions;

namespace LinqProviderMSSQL.QueryProvider;

public class MSEntitySet<T> : IQueryable<T> where T : class
{
    protected readonly Expression Expr;
    protected readonly IQueryProvider QueryProvider;

    public MSEntitySet(MSDataService client)
    {
        ArgumentNullException.ThrowIfNull(client);

        Expr = Expression.Constant(this);
        QueryProvider = new MSLinqProvider(client);
    }

    public Type ElementType => typeof(T);

    public Expression Expression => Expr;

    public IQueryProvider Provider => QueryProvider;

    public IEnumerator<T> GetEnumerator()
    {
        return QueryProvider.Execute<IEnumerable<T>>(Expr).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return QueryProvider.Execute<IEnumerable>(Expr).GetEnumerator();
    }
}

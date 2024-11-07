using System.Collections;
using System.Linq.Expressions;

namespace LazyAPI.Database;

public sealed class DisposableQuery<T> : IQueryable<T>, IDisposable
{
    private readonly IQueryable<T> query;
    private readonly IDisposable disposable;

    public DisposableQuery(IQueryable<T> query, IDisposable disposable)
    {
        this.query = query;
        this.disposable = disposable;
    }
    public Expression Expression => this.query.Expression;

    public Type ElementType => this.query.ElementType;

    public IQueryProvider Provider => this.query.Provider;

    public void Dispose()
    {
        this.disposable.Dispose();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return this.query.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.query.GetEnumerator();
    }
}
using System.Linq.Expressions;

namespace PaSharper.Interfaces;

public interface IFileMappable<T>
{ 
    List<(string keyword, Expression<Func<T, object>> property, Func<string, object> transformer)> GetMapping();
}

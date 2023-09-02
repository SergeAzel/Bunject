using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Utility
{
  internal class InjectionDictionary<K, V> : Dictionary<K, V>, IDictionary<K,V> where V : class
  {
    private Func<K, V, V> injector;
    public InjectionDictionary(Func<K, V, V> injector, IDictionary<K, V> source) : base(source)
    {
      this.injector = injector;
    }

    public new V this[K key]
    {
      get 
      {
        V result;
        if (base.TryGetValue(key, out V oldResult))
        {
          result = injector(key, oldResult);
        }
        else
        {
          result = injector(key, null);
        }

        if (result == null)
          throw new KeyNotFoundException();

        return result;
      }
      set
      {
        base[key] = value;
      }
    }

    public new bool TryGetValue(K key, out V result)
    {
      result = default(V);

      if (base.TryGetValue(key, out result))
      {
        result = injector(key, result);
      }
      else
      {
        result = injector(key, null);
      }

      return (result == null);
    }
  }
}

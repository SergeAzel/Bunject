using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunject.Utility
{
  public static class ObjectExtensions
  {
    //Currying.. kinda..
    public static Func<O> Curry<I, O>(this I instance, Func<I, O> innerFunc)
    {
      return () => innerFunc(instance);
    }

    public static Func<O> Curry<I, O>(this Func<I, O> innerFunc, I instance)
    {
      return () => innerFunc(instance);
    }

    public static Func<A, O> Curry<I, A, O>(this I instance, Func<I, A, O> innerFunc)
    {
      return (A argument) => innerFunc(instance, argument);
    }

    public static Func<A, O> Curry<I, A, O>(this Func<I, A, O> innerFunc, I instance)
    {
      return (A argument) => innerFunc(instance, argument);
    }
  }
}

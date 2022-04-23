using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System;

public delegate bool BoolFN<T>(T obj);
public static class Assertions
{
    public static void AssertNotNull<T>(T obj, string propertyName, [CallerMemberName] string callerName = "")
    {
        if (obj == null)
            throw new NullReferenceException($"Expected {propertyName} in call to {callerName}, got null");
    }
    public static void AssertNotEmpty<T>(IEnumerable<T> obj, string propertyName, [CallerMemberName] string callerName = "")
    {
        if (obj.Count() < 1)
            throw new ApplicationException($"{propertyName} is not allowed to be empty in call to {callerName}");
    }
}
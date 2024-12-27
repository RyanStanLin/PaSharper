using System;
using System.Collections.Generic;
using System.Linq;

public static class Pipeline
{
    public static T Pipe<T>(this T input, params Func<T, T>[] operations)
    {
        foreach (var operation in operations)
        {
            input = operation(input);
        }
        return input;
    }
}
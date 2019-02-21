using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ArrayUtil 
{
    public static void Add<T>(this Array ary, T value)
    {
        for (int i = 0; i < ary.Length; i++)
        {
            if(ary.GetValue(i) == null)
            {
                ary.SetValue(value, i);
            }
        }
    }
    public static bool Remove<T>(this Array ary, T value)
    {
        return false;
    }

    public static void AddRange<T>(this Array ary, T[] value)
    {

    }
}

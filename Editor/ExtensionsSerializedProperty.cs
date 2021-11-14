using System;
using System.Reflection;
using UnityEditor;

public static class ExtensionsSerializedProperty
{
    public static T GetValue<T>(this SerializedProperty prop)
    {
        return (T)prop.GetTargetObject();
    }

    /// <summary>
    /// Sets the value of the property on <b>all</b> objects currently targeted by this SerializedProperty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prop"></param>
    /// <param name="value"></param>
    public static void SetValue<T>(this SerializedProperty prop, T value)
    {
        foreach (var target in prop.serializedObject.targetObjects)
            prop.SetTargetObject(target, value);
    }

    // Following methods modified: https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
    public static object GetTargetObject(this SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');

        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, elementName, index);
            }
            else
            {
                obj = GetValue(obj, element);
            }
        }

        return obj;
    }

    public static void SetTargetObject(this SerializedProperty prop, object target, object value)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');

        var hierarchy = GetObjectHierarchy(target, elements);

        // Iterate upwards through the hierarchy, updating each parent's value
        // after the child has been set. This ensures that structs (value types)
        // are set by value, not their boxed object reference.
        for (int i = hierarchy.Length - 1; i >= 0; i--)
        {
            string element = elements[i];

            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                var field = hierarchy[i].GetType().GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                var arr = field.GetValue(hierarchy[i]) as System.Collections.IList;
                arr[index] = value;
            }
            else
            {
                var field = hierarchy[i].GetType().GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                field.SetValue(hierarchy[i], value);
            }
            
            value = hierarchy[i];
        }
    }

    private static object[] GetObjectHierarchy(object root, string[] elements)
    {
        object[] hierarchy = new object[elements.Length];
        hierarchy[0] = root;

        for (int i = 0; i < elements.Length - 1; i++)
        {
            var element = elements[i];

            object obj;
            object source = hierarchy[i];

            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(source, elementName, index);
            }
            else
            {
                obj = GetValue(source, element);
            }

            hierarchy[i + 1] = obj;
        }

        return hierarchy;
    }

    private static object GetValue(object source, string name, int index)
    {
        if (!(GetValue(source, name) is System.Collections.IEnumerable enumerable))
            throw new InvalidCastException();

        var enm = enumerable.GetEnumerator();

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext())
                throw new IndexOutOfRangeException();
        }

        return enm.Current;
    }

    private static object GetValue(object source, string name)
    {
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (f != null)
                return f.GetValue(source);

            type = type.BaseType;
        }

        return null;
    }
}

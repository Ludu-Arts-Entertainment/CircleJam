using System;
using JetBrains.Annotations;

[AttributeUsage(AttributeTargets.Field,
    AllowMultiple = true)]
public class SubcategoryOf : Attribute
{
    [CanBeNull] public Enum Category { get; }

    public SubcategoryOf(object category)
    {
        if (category is Enum categoryEnum) Category = categoryEnum;
    }
}
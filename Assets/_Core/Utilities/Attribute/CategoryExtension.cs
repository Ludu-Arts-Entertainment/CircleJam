using System;
using System.Linq;
using System.Reflection;

public static class CategoryExtensions {
    public static bool IsSubcategoryOf(this Enum sub, Enum cat) {
        Type t = sub.GetType();
        MemberInfo mi = t.GetMember(sub.ToString()).FirstOrDefault(m => m.GetCustomAttribute(typeof(SubcategoryOf)) != null);
        if (mi == null) throw new ArgumentException("Subcategory " + sub + " has no category.");
        SubcategoryOf subAttr = (SubcategoryOf)mi.GetCustomAttribute(typeof(SubcategoryOf));
        return subAttr.Category == cat;
    }
    
    public static object[] GetSubcategories<T>(this Enum cat) {
        Type t = typeof(T);
        return t.GetMembers().Where(m => m.GetCustomAttribute(typeof(SubcategoryOf)) != null).Select(m => {
            SubcategoryOf subAttr = (SubcategoryOf)m.GetCustomAttribute(typeof(SubcategoryOf));
            if (Equals(subAttr.Category, cat)) return Enum.Parse(t, m.Name);
            else return Enum.Parse(t, "None");
        }).ToArray();
    }
}
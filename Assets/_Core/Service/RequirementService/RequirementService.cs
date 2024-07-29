using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RequirementService
{
    private static readonly Dictionary<RequirementType, IRequirement> Requirements = new Dictionary<RequirementType, IRequirement>();

    public static bool Check(List<RequirementTypeValueTuple> listOfRequirementTypeValueTuples)
    {
        return listOfRequirementTypeValueTuples.All(Check);
    }
    public static bool Check(RequirementTypeValueTuple requirementTypeValueTuple)
    {
        var requirement = GetRequirement(requirementTypeValueTuple.RequirementType);
        return requirement.Check(requirementTypeValueTuple.MinValue, requirementTypeValueTuple.MaxValue);
    }
    private static IRequirement GetRequirement(RequirementType requirementType)
    {
        if (Requirements.TryGetValue(requirementType, out var requirement))
        {
            return requirement;
        }
        requirement = GameInstaller.Instance.Requirements.Find(x => x.requirementType == requirementType).requirement;
        if (requirement == null)
        {
            Debug.Log("Requirement not found for " + requirementType);
            return null;
        }
        Requirements.Add(requirementType, requirement);
        return requirement;
    }
}
#if !RequirementService_Modified
public enum RequirementType
{
    Level,
    Coin,
    Gem,
}
#endif
[Serializable]
public class RequirementTypeValueTuple
{
    public RequirementType RequirementType = default;
    public int MinValue;
    public int MaxValue;
}
[Serializable]
public class RequirementTypeRequirementTuple
{
    public RequirementType requirementType;
    public BaseRequirement requirement;
}
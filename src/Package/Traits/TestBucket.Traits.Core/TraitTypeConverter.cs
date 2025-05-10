using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.Traits.Core;

/// <summary>
/// Converts between strings and TraitType (known traits)
/// 
/// The trait strings are used by various unit testing frameworks, such as [Trait] for xUnit, or [TestProperty] for MSTest.
/// 
/// This converter is used when serializing and deserializing xunit/junit XML formats, to support a generic intermediate representation.
/// Some traits are read from the traits or property XML elements, and they will use this conversion while others are part of the specification
/// for the XML formats, such as test result and name of the test.
/// 
/// As not all formats have the same native support, some traits may be written as a trait/property but read from a well-specified XML element/attribute.
/// </summary>
public class TraitTypeConverter
{
    private static readonly FrozenDictionary<TraitType, string> _traitsToStrings = new Dictionary<TraitType, string>
    {
        [TraitType.Component] = TargetTraitNames.Component,
        [TraitType.SoftwareVersion] = TargetTraitNames.SoftwareVersion,
        [TraitType.HardwareVersion] = TargetTraitNames.HardwareVersion,
        [TraitType.Commit] = TargetTraitNames.Commit,
        [TraitType.Release] = TargetTraitNames.Release,
        [TraitType.Milestone] = TargetTraitNames.Milestone,
        [TraitType.Feature] = TargetTraitNames.Feature,

        [TraitType.Assembly] = AutomationTraitNames.Assembly,
        [TraitType.Module] = AutomationTraitNames.Module,
        [TraitType.ClassName] = AutomationTraitNames.ClassName,
        [TraitType.Method] = AutomationTraitNames.MethodName,

        [TraitType.QualityCharacteristic] = TestTraitNames.QualityCharacteristic,
        [TraitType.CoveredRequirement] = TestTraitNames.CoveredRequirement,
        [TraitType.CoveredIssue] = TestTraitNames.CoveredIssue,

        [TraitType.TestCategory] = TestTraitNames.TestCategory,
        [TraitType.TestPriority] = TestTraitNames.TestPriority,
        [TraitType.TestActivity] = TestTraitNames.TestActivity,
        [TraitType.TestState] = TestTraitNames.TestState,
        [TraitType.TestDescription] = TestTraitNames.TestDescription,
        [TraitType.TestId] = TestTraitNames.TestId,
        [TraitType.Browser] = TestTraitNames.Browser,
        [TraitType.Approved] = TestTraitNames.Approved,

        [TraitType.Tag] = TestTraitNames.Tag,
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, TraitType> _stringToTraits;

    static TraitTypeConverter()
    {
        var stringToTraits = new Dictionary<string, TraitType>();
        foreach (var kvp in _traitsToStrings)
        {
            stringToTraits[kvp.Value.ToLower()] = kvp.Key;
        }
        _stringToTraits = stringToTraits.ToFrozenDictionary();
    }

    /// <summary>
    /// Tries to convert from a trait to a string
    /// </summary>
    /// <param name="input"></param>
    /// <param name="traitName"></param>
    /// <returns>true if successful</returns>
    public static bool TryConvert(TraitType? input, [NotNullWhen(true)] out string? traitName)
    {
        traitName = null;
        if (input is not null && _traitsToStrings.TryGetValue(input.Value, out var name))
        {
            traitName = name;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tries to convert from a string to a trait
    /// </summary>
    /// <param name="input"></param>
    /// <param name="traitType"></param>
    /// <returns>true if successful</returns>
    public static bool TryConvert(string? input, [NotNullWhen(true)] out TraitType? traitType)
    {
        traitType = null;
        if(input is not null && _stringToTraits.TryGetValue(input.ToLower(), out var trait))
        {
            traitType = trait;
            return true;
        }
        return false;
    }
}

using iERP.SharedKernel.Exceptions;

namespace iERP.Modules.Platform.Metadata.Application.Layout;

/// <summary>
/// Normalized layout state shared by metadata GenericPage fields and dynamic entity fields.
/// </summary>
public sealed record FieldLayoutState(
    string FieldKey,
    bool Required,
    bool Visible,
    int DisplayOrder);

/// <summary>
/// Persisted / requested per-user preference for one field.
/// </summary>
public sealed record FieldPreferenceValue(
    string FieldKey,
    bool IsVisible,
    int DisplayOrder);

/// <summary>
/// Applies per-user hide/order preferences onto field layout (SOLID: single responsibility, reusable).
/// Required fields cannot be hidden.
/// </summary>
public static class FieldLayoutPreferenceApplier
{
    public static FieldLayoutState Apply(FieldLayoutState field, FieldPreferenceValue? preference)
    {
        if (preference is null)
        {
            return field;
        }

        return field with
        {
            Visible = field.Required || preference.IsVisible,
            DisplayOrder = preference.DisplayOrder
        };
    }

    public static IReadOnlyList<FieldLayoutState> ApplyAll(
        IEnumerable<FieldLayoutState> fields,
        IReadOnlyDictionary<string, FieldPreferenceValue> preferences)
    {
        return fields
            .Select(field =>
            {
                preferences.TryGetValue(field.FieldKey, out var pref);
                return Apply(field, pref);
            })
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.FieldKey, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static void EnsureRequiredRemainVisible(
        IEnumerable<(string FieldKey, bool Required)> fields,
        IEnumerable<FieldPreferenceValue> preferences)
    {
        var required = fields
            .Where(x => x.Required)
            .Select(x => x.FieldKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var pref in preferences)
        {
            if (!pref.IsVisible && required.Contains(pref.FieldKey))
            {
                throw new ValidationException($"Required field '{pref.FieldKey}' cannot be hidden.");
            }
        }
    }
}

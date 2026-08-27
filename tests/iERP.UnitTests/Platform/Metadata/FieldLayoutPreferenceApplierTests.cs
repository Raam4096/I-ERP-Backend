using FluentAssertions;
using iERP.Modules.Platform.Metadata.Application;
using iERP.Modules.Platform.Metadata.Application.Dtos;
using iERP.Modules.Platform.Metadata.Application.Layout;
using iERP.SharedKernel.Exceptions;

namespace iERP.UnitTests.Platform.Metadata;

public sealed class FieldLayoutPreferenceApplierTests
{
    [Fact]
    public void Apply_WhenNoPreference_KeepsDefaults()
    {
        var field = new FieldLayoutState("notes", Required: false, Visible: true, DisplayOrder: 5);

        var result = FieldLayoutPreferenceApplier.Apply(field, preference: null);

        result.Should().Be(field);
    }

    [Fact]
    public void Apply_WhenOptionalHidden_HidesFieldAndUpdatesOrder()
    {
        var field = new FieldLayoutState("notes", Required: false, Visible: true, DisplayOrder: 5);
        var pref = new FieldPreferenceValue("notes", IsVisible: false, DisplayOrder: 99);

        var result = FieldLayoutPreferenceApplier.Apply(field, pref);

        result.Visible.Should().BeFalse();
        result.DisplayOrder.Should().Be(99);
    }

    [Fact]
    public void Apply_WhenRequiredHidden_ForcesVisible()
    {
        var field = new FieldLayoutState("email", Required: true, Visible: true, DisplayOrder: 1);
        var pref = new FieldPreferenceValue("email", IsVisible: false, DisplayOrder: 40);

        var result = FieldLayoutPreferenceApplier.Apply(field, pref);

        result.Visible.Should().BeTrue();
        result.DisplayOrder.Should().Be(40);
    }

    [Fact]
    public void ApplyAll_ReordersByPreferenceDisplayOrder()
    {
        var fields = new[]
        {
            new FieldLayoutState("a", false, true, 1),
            new FieldLayoutState("b", false, true, 2),
            new FieldLayoutState("c", false, true, 3)
        };
        var prefs = new Dictionary<string, FieldPreferenceValue>(StringComparer.OrdinalIgnoreCase)
        {
            ["a"] = new("a", true, 30),
            ["b"] = new("b", true, 10),
            ["c"] = new("c", false, 20)
        };

        var result = FieldLayoutPreferenceApplier.ApplyAll(fields, prefs);

        result.Select(x => x.FieldKey).Should().Equal("b", "c", "a");
        result.Single(x => x.FieldKey == "c").Visible.Should().BeFalse();
    }

    [Fact]
    public void EnsureRequiredRemainVisible_ThrowsWhenHidingRequired()
    {
        var fields = new[] { ("email", true), ("notes", false) };
        var prefs = new[] { new FieldPreferenceValue("email", false, 1) };

        var act = () => FieldLayoutPreferenceApplier.EnsureRequiredRemainVisible(fields, prefs);

        act.Should().Throw<ValidationException>()
            .WithMessage("*email*cannot be hidden*");
    }

    [Fact]
    public void MetadataScreenService_ApplyPreferences_MergesOntoGenericPageFields()
    {
        var fields = new[]
        {
            new GenericPageFieldDto
            {
                FieldKey = "companyName",
                Label = "Company",
                Required = true,
                Visible = true,
                DisplayOrder = 1
            },
            new GenericPageFieldDto
            {
                FieldKey = "notes",
                Label = "Notes",
                Required = false,
                Visible = true,
                DisplayOrder = 2
            }
        };
        var prefs = new Dictionary<string, FieldPreferenceValue>(StringComparer.OrdinalIgnoreCase)
        {
            ["notes"] = new("notes", false, 0),
            ["companyName"] = new("companyName", true, 5)
        };

        var result = MetadataScreenService.ApplyPreferences(fields, prefs);

        result.Should().HaveCount(2);
        result[0].FieldKey.Should().Be("notes");
        result[0].Visible.Should().BeFalse();
        result[1].FieldKey.Should().Be("companyName");
        result[1].Visible.Should().BeTrue();
        result[1].DisplayOrder.Should().Be(5);
    }
}

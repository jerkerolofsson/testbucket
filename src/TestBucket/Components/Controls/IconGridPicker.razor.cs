using System.Reflection;

namespace TestBucket.Components.Controls;

public partial class IconGridPicker
{
    internal record class MudIcons(string Name, string Value, string Category);


    private List<MudIcons> _displayedIcons = new();

    private List<MudIcons> CustomAll { get; } = new();

    private List<MudIcons> CustomBrands { get; } = new();

    private List<MudIcons> CustomFileFormats { get; } = new();

    private List<MudIcons> CustomUncategorized { get; } = new();

    private List<MudIcons> MaterialFilled { get; set; } = new();

    private List<MudIcons> MaterialOutlined { get; set; } = new();

    private List<MudIcons> MaterialRounded { get; set; } = new();

    private List<MudIcons> MaterialSharp { get; set; } = new();

    private List<MudIcons> MaterialTwoTone { get; set; } = new();

    private MudIcons? SelectedIcon { get; set; } = null;

    private Size PreviewIconSize { get; set; } = Size.Medium;

    private Color PreviewIconColor { get; set; } = Color.Dark;

    private IconOrigin SelectedIconOrigin { get; set; } = IconOrigin.Material;

    private string IconCodeOutput { get; set; } = string.Empty;

    private string SearchText { get; set; } = string.Empty;
    private const double IconCardWidth = 136.88; // single icon card width including margins
    private const float IconCardHeight = 144; // single icon card height including margins
    
    private List<MudIcons> SelectedIcons => string.IsNullOrWhiteSpace(SearchText)
         ? _displayedIcons.Take(64).ToList()
         : _displayedIcons.Where(mudIcon => mudIcon.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).Take(64).ToList();


    private readonly IconStorage _iconTypes = new()
        {
            { IconType.Filled, typeof(MudBlazor.Icons.Material.Filled) },
            { IconType.Outlined, typeof(MudBlazor.Icons.Material.Outlined) },
            { IconType.Rounded, typeof(MudBlazor.Icons.Material.Rounded) },
            { IconType.Sharp, typeof(MudBlazor.Icons.Material.Sharp) },
            { IconType.TwoTone, typeof(MudBlazor.Icons.Material.TwoTone) },
            { IconType.Brands, typeof(MudBlazor.Icons.Custom.Brands) },
            { IconType.FileFormats, typeof(MudBlazor.Icons.Custom.FileFormats) },
            { IconType.Uncategorized, typeof(MudBlazor.Icons.Custom.Uncategorized) }
        };

    private void LoadCustomIcons()
    {
        CustomBrands.AddRange(GetMudIconsByTypeCategory(typeof(MudBlazor.Icons.Custom.Brands), IconType.Brands));
        CustomAll.AddRange(CustomBrands);

        CustomFileFormats.AddRange(GetMudIconsByTypeCategory(typeof(MudBlazor.Icons.Custom.FileFormats), IconType.FileFormats));
        CustomAll.AddRange(CustomFileFormats);

        CustomUncategorized.AddRange(GetMudIconsByTypeCategory(typeof(MudBlazor.Icons.Custom.Uncategorized), IconType.Uncategorized));
        CustomAll.AddRange(CustomUncategorized);
    }


    private List<MudIcons> GetMudIconsByTypeCategory(Type iconType, string category)
    {
        return iconType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Select(prop => new MudIcons(prop.Name, GetIconCodeOrDefault(prop), category))
            .ToList();
    }
    private string GetIconCodeOrDefault(FieldInfo fieldInfo) => fieldInfo.GetRawConstantValue()?.ToString() ?? string.Empty;

    private void ChangeIconCategory(string type)
    {
        _displayedIcons = type switch
        {
            IconType.Filled => MaterialFilled,
            IconType.Outlined => MaterialOutlined,
            IconType.Rounded => MaterialRounded,
            IconType.Sharp => MaterialSharp,
            IconType.TwoTone => MaterialTwoTone,
            IconType.All => CustomAll,
            IconType.Brands => CustomBrands,
            IconType.FileFormats => CustomFileFormats,
            IconType.Uncategorized => CustomUncategorized,
            _ => _displayedIcons
        };
    }

    private void OnSelectedValue(IconOrigin origin)
    {
        switch (origin)
        {
            case IconOrigin.Material:
                ChangeIconCategory(IconType.Filled);
                break;
            case IconOrigin.Custom:
                ChangeIconCategory(IconType.All);
                break;
        }

        SelectedIconOrigin = origin;
    }

    private List<MudIcons> LoadMaterialIcons(string type)
    {
        var iconType = _iconTypes[type];
        var result = GetMudIconsByTypeCategory(iconType, type);
        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        _displayedIcons = MaterialFilled = LoadMaterialIcons(IconType.Filled);
        MaterialOutlined = LoadMaterialIcons(IconType.Outlined);
        MaterialRounded = LoadMaterialIcons(IconType.Rounded);
        MaterialSharp = LoadMaterialIcons(IconType.Sharp);
        MaterialTwoTone = LoadMaterialIcons(IconType.TwoTone);

        LoadCustomIcons();
        await base.OnInitializedAsync();
    }

}

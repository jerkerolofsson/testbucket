using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Contracts.Appearance.Models;

namespace TestBucket.Blazor.Components;
public partial class ColorCellPicker
{

    private static readonly List<string> _colors = [];

    private bool _open = false;

    private string? _color = "#000000";

    static ColorCellPicker()
    {
        AddColors("#888888");
        AddColors("#EF476F");
        AddColors("#FFD166");
        AddColors("#06D6A0");
        AddColors("#118AB2");
        _colors.Add("#a4c639");
        _colors.Add("#6200ee");
        _colors.Add("#03dac6");
    }

    private static void AddColors(string baseColorString)
    {
        var baseColor = ThemeColor.Parse(baseColorString);

        for (double x = 0.4; x >= 0.1; x -= 0.1)
        {
            _colors.Add(baseColor.ColorDarken(x).ToString(ColorOutputFormats.Hex));
        }

        _colors.Add(baseColorString);

        for (double x = 0.1; x <= 0.3; x += 0.1)
        {
            _colors.Add(baseColor.ColorLighten(x).ToString(ColorOutputFormats.Hex));
        }
    }

    protected override void OnParametersSet()
    {
        _color = Color;
    }

    private async Task OnTextChanged(string color)
    {
        if(string.IsNullOrWhiteSpace(color))
        {
            _color = null;
            await ColorChanged.InvokeAsync(_color);
            return;
        }

        if (color == "transparent" || ThemeColor.TryParse(color, out var mudColor))
        {
            _color = color;
            await ColorChanged.InvokeAsync(_color);
        }
        _open = false;
    }
}

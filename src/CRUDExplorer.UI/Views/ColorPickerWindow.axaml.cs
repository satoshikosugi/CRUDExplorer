using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace CRUDExplorer.UI.Views;

public partial class ColorPickerWindow : Window
{
    private bool _syncing;

    /// <summary>選択された色（キャンセル時はnull）</summary>
    public Color? SelectedColor { get; private set; }

    // Windows標準カラーダイアログ風の48色パレット
    private static readonly string[] PaletteColors =
    {
        "#FF0000", "#FF8000", "#FFFF00", "#80FF00", "#00FF00", "#00FF80",
        "#00FFFF", "#0080FF", "#0000FF", "#8000FF", "#FF00FF", "#FF0080",
        "#800000", "#804000", "#808000", "#408000", "#008000", "#008040",
        "#008080", "#004080", "#000080", "#400080", "#800080", "#800040",
        "#FF8080", "#FFC080", "#FFFF80", "#C0FF80", "#80FF80", "#80FFC0",
        "#80FFFF", "#80C0FF", "#8080FF", "#C080FF", "#FF80FF", "#FF80C0",
        "#000000", "#202020", "#404040", "#606060", "#808080", "#A0A0A0",
        "#C0C0C0", "#E0E0E0", "#FFFFFF", "#FFC0CB", "#FFD700", "#F0E68C",
    };

    public ColorPickerWindow()
    {
        InitializeComponent();
    }

    public ColorPickerWindow(Color initialColor) : this()
    {
        SelectedColor = null;

        // パレット生成
        var palette = this.FindControl<WrapPanel>("ColorPalette")!;
        foreach (var hex in PaletteColors)
        {
            if (!Color.TryParse(hex, out var c)) continue;
            var btn = new Button
            {
                Width = 28,
                Height = 28,
                Margin = new Thickness(2),
                Padding = new Thickness(2),
                CornerRadius = new CornerRadius(3),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Content = new Border
                {
                    Width = 20,
                    Height = 20,
                    CornerRadius = new CornerRadius(2),
                    Background = new SolidColorBrush(c)
                },
                Tag = c
            };
            btn.Click += OnPaletteClick;
            palette.Children.Add(btn);
        }

        // 初期色をセット
        SetColor(initialColor);
    }

    private void SetColor(Color c)
    {
        _syncing = true;
        try
        {
            var hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
            TxtHex.Text = hex;
            NudR.Value = c.R;
            NudG.Value = c.G;
            NudB.Value = c.B;
            PreviewRect.Background = new SolidColorBrush(c);
        }
        finally
        {
            _syncing = false;
        }
    }

    private Color GetCurrentColor()
    {
        return Color.FromRgb(
            (byte)(NudR.Value ?? 0),
            (byte)(NudG.Value ?? 0),
            (byte)(NudB.Value ?? 0));
    }

    private void OnPaletteClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Color c })
            SetColor(c);
    }

    private void OnHexTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_syncing) return;
        var text = TxtHex.Text ?? "";
        if (Color.TryParse(text, out var c))
        {
            _syncing = true;
            try
            {
                NudR.Value = c.R;
                NudG.Value = c.G;
                NudB.Value = c.B;
                PreviewRect.Background = new SolidColorBrush(c);
            }
            finally { _syncing = false; }
        }
    }

    private void OnRgbChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_syncing) return;
        var c = GetCurrentColor();
        _syncing = true;
        try
        {
            TxtHex.Text = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
            PreviewRect.Background = new SolidColorBrush(c);
        }
        finally { _syncing = false; }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        SelectedColor = GetCurrentColor();
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        SelectedColor = null;
        Close();
    }
}

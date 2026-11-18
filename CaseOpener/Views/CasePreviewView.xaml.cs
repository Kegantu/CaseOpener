using CaseOpener.ViewModels;

namespace CaseOpener.Views;

public partial class CasePreviewView : ContentView
{
    // Must match the item template: WidthRequest="150" and the stack Spacing="8".
    private const double ItemWidth = 150;
    private const double ItemSpacing = 8;
    private const double ItemStep = ItemWidth + ItemSpacing;

    private const uint SpinDurationMs = 7000;
    private const int PauseOnWinnerMs = 900;

    // Quartic ease-out: fast start, long slow crawl onto the winner (the CS2 feel).
    private static readonly Easing SlowDown = new(t => 1 - Math.Pow(1 - t, 4));

    private CasePreviewViewModel? _vm;

    private const double PanelMaxWidth = 720;
    private const double PanelMaxHeight = 780;
    private const double PanelMargin = 20;

    public CasePreviewView()
    {
        InitializeComponent();
        SizeChanged += (_, _) => UpdatePanelSize();
    }

    /// <summary>Panel = window size minus margin, capped at the maximum, always centred.</summary>
    private void UpdatePanelSize()
    {
        if (Width <= 0 || Height <= 0) return;

        var width = Math.Min(PanelMaxWidth, Width - PanelMargin * 2);
        var height = Math.Min(PanelMaxHeight, Height - PanelMargin * 2);

        if (width > 0) Panel.WidthRequest = width;
        if (height > 0) Panel.HeightRequest = height;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (_vm is not null) _vm.ReelPlayer = null;
        _vm = BindingContext as CasePreviewViewModel;
        if (_vm is not null) _vm.ReelPlayer = PlayReelAsync;
    }

    private async Task PlayReelAsync(int winnerIndex)
    {
        var count = _vm?.ReelItems.Count ?? 0;
        if (count == 0) return;

        Reel.WidthRequest = count * ItemStep - ItemSpacing;
        Reel.TranslationX = 0;

        // The roulette layer was just made visible; wait for its first layout pass.
        for (var i = 0; i < 30 && ReelViewport.Width <= 0; i++)
            await Task.Delay(30);

        var viewport = ReelViewport.Width;

        // Stop at a random point inside the winning tile (not always dead centre).
        var jitter = (Random.Shared.NextDouble() - 0.5) * (ItemWidth - 24);
        var winnerCentre = winnerIndex * ItemStep + ItemWidth / 2 + jitter;
        var target = -(winnerCentre - viewport / 2);

        await AnimateTranslationAsync(0, target);
        await Task.Delay(PauseOnWinnerMs);
    }

    private Task AnimateTranslationAsync(double from, double to)
    {
        var tcs = new TaskCompletionSource();
        var animation = new Animation(v => Reel.TranslationX = v, from, to, SlowDown);
        animation.Commit(Reel, "ReelSpin", 16, SpinDurationMs, finished: (_, _) => tcs.TrySetResult());
        return tcs.Task;
    }
}

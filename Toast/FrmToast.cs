using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace KorzhLocker.Toast;

public partial class FrmToast : Form
{
    private int duration;
    private bool isInitialized = false;

    public FrmToast()
    {
        InitializeComponent();
    }

    public FrmToast(string message, int durationMs) : this()
    {
        label1.Text = message;
        duration = durationMs;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (isInitialized) return;
        isInitialized = true;
        _ = FadeLoop();
    }

    private async Task FadeLoop()
    {
        try
        {
            await Task.Delay(duration);

            const int fadeDuration = 300;
            const int steps = 30;
            int delay = fadeDuration / steps;

            for (int i = 0; i < steps; i++)
            {
                if (IsDisposed) return;

                Opacity = Math.Max(0, Opacity - 1.0 / steps);
                await Task.Delay(delay);
            }

            Close();
        }
        catch
        {
        }
    }
}

namespace KorzhLocker.Toast;

internal sealed class ToastManager
{
    public static ToastManager Instance => field ??= new();

    private const int MaybeTaskbarHeight = 40;
    private readonly int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
    private readonly int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;

    private List<FrmToast> activeToasts = new();

    private ToastManager() 
    { 
    
    }

    public void ShowToast(string message, int durationMs)
    {
        FrmToast toast = new(message, durationMs);
        toast.Top = ScreenHeight - toast.Height * (activeToasts.Count + 1) - MaybeTaskbarHeight;
        toast.Left = ScreenWidth - toast.Width;
        toast.Show();
        toast.FormClosed += Toast_FormClosed;

        UpdateToastPositions();
    }

    private void Toast_FormClosed(object? sender, FormClosedEventArgs e)
    {
        FrmToast? toast = sender as FrmToast;
        if (toast is null) return;

        activeToasts.Remove(toast);
        toast.Dispose();

        UpdateToastPositions();
    }

    private void UpdateToastPositions()
    {
        for (int i = 0; i < activeToasts.Count; i++)
        {
            var toast = activeToasts[i];
            toast.Top = ScreenHeight - toast.Height * (i + 1) - MaybeTaskbarHeight;
            toast.Left = ScreenWidth - toast.Width;
        }
    }
}

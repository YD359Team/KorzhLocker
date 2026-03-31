using KorzhLocker.Toast;

namespace KorzhLocker;

public partial class Form1 : Form
{
    private readonly ToastManager toastManager = ToastManager.Instance;

    private bool isHidden = false;
    private bool needToClose = false;
    private readonly System.Windows.Forms.Timer timer1 = new()
    {
        Interval = 1000
    };
    private readonly NotifyIcon notifyIcon = new();

    public Form1()
    {
        InitializeComponent();
        if (DesignMode) return;

        #region notifyIcon
        notifyIcon.Icon = this.Icon;
        notifyIcon.ContextMenuStrip = new();
        notifyIcon.MouseClick += NotifyIcon_MouseClick;
        notifyIcon.ContextMenuStrip.Items.Add("Show\\Hide", null, (s, e) =>
        {
            if (isHidden) ShowWindow();
            else HideWindow();
        });
        notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) =>
        {
            needToClose = true;
            this.Close();
        });
        notifyIcon.Visible = false;
        #endregion

        this.Deactivate += Form1_Deactivate; 
        label1.MouseClick += (s, e) => this.OnMouseClick(e);
        timer1.Tick += Timer1_Tick;
        timer1.Enabled = true;

        ShowToast("KorzhLocker is launched");
    }

    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) ShowWindow();
    }

    private void Form1_Deactivate(object? sender, EventArgs e)
    {
        if (isHidden) return;

        this.Activate();
    }

    private void Timer1_Tick(object? sender, EventArgs e)
    {
        this.Activate();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.LWin)
        {
            SendKeys.Send("^");
        }
        return true;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (isHidden)
        {
            needToClose = true;
            base.OnFormClosing(e);
            return;
        }

        if (!needToClose)
        {
            e.Cancel = true;
        }
        else
        {
            base.OnFormClosing(e);
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (needToClose) base.OnFormClosed(e);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        HideWindow();
    }

    private void HideWindow()
    {
        this.Hide();
        isHidden = true;
        timer1.Enabled = false;
        notifyIcon.Visible = true;

        ShowToast("KorzhLocker is hidden");
    }

    private void ShowWindow()
    {
        this.Show();
        isHidden = true;
        timer1.Enabled = true;
        notifyIcon.Visible = false;

        ShowToast("KorzhLocker is showing");
    }

    private void ShowToast(string message)
    {
        toastManager.ShowToast(message, 1000);
    }
}
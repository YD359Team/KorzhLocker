using KorzhLocker.Input;
using KorzhLocker.Toast;
using System.Media;

namespace KorzhLocker;

public partial class Form1 : Form
{
    private readonly ToastManager toastManager = ToastManager.Instance;
    private readonly ModifierKeyMonitor keyMonitor = ModifierKeyMonitor.Instance;

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

        label1.Text = SimpleEnvironment.LockWindowHint;

        #region notifyIcon
        notifyIcon.Icon = this.Icon;
        notifyIcon.ContextMenuStrip = new();
        notifyIcon.MouseClick += NotifyIcon_MouseClick;
        notifyIcon.ContextMenuStrip.Items.Add(SimpleEnvironment.NotifyContextMenuShowHide, null, (s, e) =>
        {
            if (isHidden) ShowWindow();
            else HideWindow();
        });
        notifyIcon.ContextMenuStrip.Items.Add(SimpleEnvironment.NotifyContextMenuExit, null, (s, e) =>
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

        keyMonitor.Start();

        ShowToast(SimpleEnvironment.LaunchToast);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.LimeGreen, ButtonBorderStyle.Solid);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        const int HOTKEY_ID = 1;
        const uint MOD_CONTROL = 0x0002;
        const uint MOD_ALT = 0x0001;
        const uint VK_K = 0x4B;
        HotkeyManager.RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_K);
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

    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;
        const int HOTKEY_ID = 1;

        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            ShowWindow();
        }

        base.WndProc(ref m);
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
        const int HOTKEY_ID = 1;

        if (isHidden)
        {
            needToClose = true;
            HotkeyManager.UnregisterHotKey(this.Handle, HOTKEY_ID);
            base.OnFormClosing(e);
            return;
        }

        if (!needToClose)
        {
            e.Cancel = true;
        }
        else
        {
            HotkeyManager.UnregisterHotKey(this.Handle, HOTKEY_ID);
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
        keyMonitor.Stop();
        notifyIcon.Visible = true;

        ShowToast(SimpleEnvironment.HiddenToast);
    }

    private void ShowWindow()
    {
        this.Show();
        isHidden = true;
        timer1.Enabled = true;
        keyMonitor.Start();
        notifyIcon.Visible = false;

        ShowToast(SimpleEnvironment.ShowingToast);
    }

    private void ShowToast(string message)
    {
        toastManager.ShowToast(message, 1000);
        SystemSounds.Asterisk.Play();
    }
}
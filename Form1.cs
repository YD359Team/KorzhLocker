namespace KorzhLocker;

public partial class Form1 : Form
{
    private bool needToClose = false;
    private System.Windows.Forms.Timer timer1 = new()
    {
        Enabled = true,
        Interval = 1000
    };

    public Form1()
    {
        InitializeComponent();
        this.Deactivate += (s, e) => this.Activate();
        label1.MouseClick += (s, e) => this.OnMouseClick(e);
        timer1.Tick += Timer1_Tick;
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
        needToClose = true;
        this.Close();
    }
}

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KorzhLocker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                //ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance);
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", $"An error occurred: {ex.Message} in {ex.StackTrace} inner exception {ex.InnerException?.Message}");
                if (Debugger.IsAttached)
                {
                    MessageBox.Show($"An error occurred: {ex.Message} in {ex.StackTrace} inner exception {ex.InnerException?.Message}");
                }
            }
        }
    }
}
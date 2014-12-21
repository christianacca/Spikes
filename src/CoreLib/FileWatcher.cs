using System;
using System.IO;
using System.Timers;

namespace Eca.Commons
{
    /// <summary>
    /// Watches for changes to the file supplied
    /// </summary>
    public interface IFileWatcher : IDisposable
    {
        event FileSystemEventHandler OnFileChanged;
    }



    public class FileChangedWatcher : IFileWatcher
    {
        #region Member Variables

        private readonly Timer timer;
        private readonly FileSystemWatcher watcher;

        #endregion


        #region Constructors

        public FileChangedWatcher(string filename)
        {
            watcher = new FileSystemWatcher
                          {Filter = Path.GetFileName(filename), Path = new FileInfo(filename).DirectoryName};
            watcher.Changed += HandleFileChanged;
            watcher.Renamed += HandleFileChanged;
            watcher.Created += HandleFileChanged;

            timer = new Timer(500) {AutoReset = false};
            timer.Elapsed += HandleTimerElapsed;

            watcher.EnableRaisingEvents = true;
        }

        #endregion


        #region IFileWatcher Members

        void IDisposable.Dispose()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }


        public event FileSystemEventHandler OnFileChanged;

        #endregion


        private void HandleFileChanged(object sender, FileSystemEventArgs args)
        {
            timer.Start();
        }


        private void HandleFileChanged(object sender, RenamedEventArgs e)
        {
            timer.Start();
        }


        private void HandleTimerElapsed(object sender, ElapsedEventArgs args)
        {
            timer.Stop();
            OnFileChanged(sender, null);
        }
    }
}
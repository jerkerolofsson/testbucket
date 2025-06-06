﻿namespace TestBucket.MudBlazorExtensions.Markdown.EventsArgs
{
    /// <summary>
    /// Provides the progress state of uploaded file.
    /// </summary>
    public class FileProgressedEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="FileProgressedEventArgs"/> constructor.
        /// </summary>
        /// <param name="file">File that is being processed.</param>
        /// <param name="progress">Percentage of file upload process.</param>
        public FileProgressedEventArgs(FileEntry file, double progress)
        {
            File = file;
            Progress = progress;
        }

        /// <summary>
        /// Gets the file currently being uploaded.
        /// </summary>
        public FileEntry File { get; }

        /// <summary>
        /// Gets the total progress in the range from 0 to 1.
        /// </summary>
        public double Progress { get; }

        /// <summary>
        /// Gets the total progress in the range from 0 to 100.
        /// </summary>
        public double Percentage => Progress * 100d;
    }
}
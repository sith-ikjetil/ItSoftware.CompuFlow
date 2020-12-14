using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace ItSoftware.CompuFlow.Util
{
    public static class FileSystem
    {
        #region Public Static Methods
        /// <summary>
        /// Normalize a directory path. Make sure it ends with a directory separator.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string NormalizeDirectoryPath( string directory )
        {             
            if ( !string.IsNullOrEmpty( directory ) ) {
                if ( directory[directory.Length - 1] != Path.DirectorySeparatorChar ) {
                    directory += Path.DirectorySeparatorChar;
                }
            }
            return directory;
        }
        /// <summary>
        /// Validate filename.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidFilename( string name )
        {
            if ( name == null )
                return false;

            if ( name.Length == 0 || name.Length > 250 )
                return false;

            if ( name.IndexOfAny( Path.GetInvalidFileNameChars( ) ) != -1 ) {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Delete a directory structure.
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDirectoryStructure(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (string dir in directories)
            {
                FileSystem.DeleteDirectoryStructure(dir);
            }

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if ( fi.Attributes.HasFlag(FileAttributes.ReadOnly) ) {
                    fi.Attributes &= ~FileAttributes.ReadOnly;
                }
                File.Delete(file);
            }

            Directory.Delete(path);
        }
        #endregion
    }// class
}// namespace

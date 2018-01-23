using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
namespace ItSoftware.CompuFlow.Util
{
    public static class Zip
    {
        #region Public Static Methods
        /// <summary>
        /// Unpack a zip file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="destinationDirectory"></param>
        public static void Unpack(string filename, string destinationDirectory)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (destinationDirectory == null)
            {
                throw new ArgumentNullException("destinationDirectory");
            }
            if (!File.Exists(filename))
            {
                throw new ArgumentException("File does not exist", "filename");
            }
            if (!Directory.Exists(destinationDirectory))
            {
                throw new ArgumentException("Directory does not exist", "destinationDirectory");
            }

            using (ZipInputStream zis = new ZipInputStream(File.OpenRead(filename)))
            {
                ZipEntry ze;
                while ((ze = zis.GetNextEntry()) != null)
                {
                    string directory = Path.Combine(destinationDirectory, Path.GetDirectoryName(ze.Name));
                    string file = Path.GetFileName(ze.Name);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (!string.IsNullOrEmpty(file))
                    {
                        using (FileStream fs = File.Create(Path.Combine(directory, file)))
                        {
                            if (ze.Size > 0)
                            {
                                int size = 2048;
                                byte[] data = new byte[size];
                                while (true)
                                {
                                    size = zis.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        fs.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }// using ( FileStream
                    }
                }// while
            }// using ( ZipInputStream
        }// Unpack
        /// <summary>
        /// Zip a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="appendZipExtension"></param>
        public static void PackFile(string filename, bool appendZipExtension)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (!File.Exists(filename))
            {
                throw new ArgumentException(string.Format("File does not exist: {0}.", filename), "filename");
            }

            string newFilename = filename + ".zip";
            if (!appendZipExtension)
            {
                newFilename = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".zip";
            }
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }

            FileStream fileStreamOut = File.Create(newFilename);

            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fileStreamOut);
            zipOutputStream.SetLevel(9);

            ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(Path.GetFileName(filename));
            zipOutputStream.PutNextEntry(zipEntry);

            FileStream fileStreamIn = File.OpenRead(filename);
            const long BUFFER_SIZE = 8192;
            long currentIndex = 0;
            byte[] buffer = new byte[BUFFER_SIZE];
            if (fileStreamIn.Length <= BUFFER_SIZE)
            {
                fileStreamIn.Read(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
                zipOutputStream.Write(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
            }
            else
            {
                do
                {
                    long remaining = BUFFER_SIZE;
                    if (currentIndex + BUFFER_SIZE >= fileStreamIn.Length)
                    {
                        remaining = fileStreamIn.Length - currentIndex;
                    }
                    fileStreamIn.Read(buffer, 0, Convert.ToInt32(remaining));
                    currentIndex += remaining;

                    zipOutputStream.Write(buffer, 0, Convert.ToInt32(remaining));
                } while (currentIndex < fileStreamIn.Length);
            }
            fileStreamIn.Close();

            zipOutputStream.Flush();
            zipOutputStream.Finish();
            zipOutputStream.Close();

            fileStreamOut.Close();
        }// ZipFile
        /// <summary>
        /// Zip multiple files(fileNames) in directory(directory) to (flowName).zip.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileNames"></param>
        /// <param name="reportName"></param>
        public static void PackFilesIntoOne(string directory, string[] fileNames, string outputFilename)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }
            if (fileNames == null)
            {
                throw new ArgumentNullException("fileNames");
            }
            if (outputFilename == null)
            {
                throw new ArgumentNullException("outputFilename");
            }
            if (fileNames.Length == 0)
            {
                throw new ArgumentException("Length cannot be 0.", "fileNames");
            }

            string newFilename = Path.Combine(directory, outputFilename);
            if (File.Exists(newFilename))
            {
                File.Delete(newFilename);
            }

            using (FileStream fileStreamOut = File.Create(newFilename))
            {

                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fileStreamOut))
                {
                    zipOutputStream.SetLevel(9);

                    foreach (string filename in fileNames)
                    {
                        ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(filename);
                        zipOutputStream.PutNextEntry(zipEntry);

                        using (FileStream fileStreamIn = File.OpenRead(Path.Combine(directory, filename)))
                        {
                            const long BUFFER_SIZE = 8192;
                            long currentIndex = 0;
                            byte[] buffer = new byte[BUFFER_SIZE];
                            if (fileStreamIn.Length <= BUFFER_SIZE)
                            {
                                fileStreamIn.Read(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
                                zipOutputStream.Write(buffer, 0, Convert.ToInt32(fileStreamIn.Length));
                            }
                            else
                            {
                                do
                                {
                                    long remaining = BUFFER_SIZE;
                                    if (currentIndex + BUFFER_SIZE >= fileStreamIn.Length)
                                    {
                                        remaining = fileStreamIn.Length - currentIndex;
                                    }
                                    fileStreamIn.Read(buffer, 0, Convert.ToInt32(remaining));
                                    currentIndex += remaining;

                                    zipOutputStream.Write(buffer, 0, Convert.ToInt32(remaining));
                                } while (currentIndex < fileStreamIn.Length);
                            }
                        }// using ( FileStream fileStreamIn = File.OpenRead( Path.Combine( directory, filename ) ...
                    }// foreach

                    zipOutputStream.Flush();
                    zipOutputStream.Finish();
                }//using ( ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream( fileStreamOut ) ) ...

            }// using ( FileStream fileStreamOut = File.Create( newFilename ) ...
        }
        #endregion
    }// class        
}// namespace

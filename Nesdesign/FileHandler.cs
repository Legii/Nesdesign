using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesdesign
{
    public enum FileOperationStatus
    {
        Success,
        FileNotFound,
        AccessDenied,
        UnknownError,
        AlreadyExists
    }
    public class FileHandler
    {
        public static String BASE_PATH = "C:\\Nesdesign\\";
        public static String PROJECTS_PATH = "projekty\\";
        public static String OFFERS_PATH = "zapytania\\";
        public static String DIR_TEMPLATE = "szablon\\";
        public static FileOperationStatus CreateDir(string dirname, bool isProject=false) 
        {

            try
            {
                string dirPath = BASE_PATH + (isProject ? PROJECTS_PATH : OFFERS_PATH);
                string fullPath = dirPath + dirname;
    
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                if(!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                else
                                    {
                    return FileOperationStatus.AlreadyExists;
                }

            }
            catch (UnauthorizedAccessException)
            {
                return FileOperationStatus.AccessDenied;
            }
            catch (Exception)
            {
                return FileOperationStatus.UnknownError;
            }
            return FileOperationStatus.Success;
        }


        public static FileOperationStatus CopyTemplate(string dirname)
        {
            try
            {
                string sourcePath = BASE_PATH + DIR_TEMPLATE;
                string destinationPath = BASE_PATH + OFFERS_PATH + dirname;
                if (!Directory.Exists(sourcePath))
                {
                    return FileOperationStatus.FileNotFound;
                }
                foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }
                foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return FileOperationStatus.AccessDenied;
            }
            catch (Exception)
            {
                return FileOperationStatus.UnknownError;
            }
            return FileOperationStatus.Success;
        }





        public static void OpenFolder(string dirname, bool isProject = false)
        {
            string dirPath = BASE_PATH + (isProject ? PROJECTS_PATH : OFFERS_PATH);
            string fullPath = dirPath + dirname;
            var psi = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = @""+fullPath,
                UseShellExecute = true
            };

            Process.Start(psi);
        }
    }
}

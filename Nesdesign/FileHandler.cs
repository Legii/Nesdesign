using Nesdesign.Models;
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
    public enum DIR_TYPE {Offer,Order,Project}
    public class FileHandler
    {
        public static String BASE_PATH = "C:\\Nesdesign\\";
        public static String PROJECTS_PATH = "projekty\\";
        public static String OFFERS_PATH = "zapytania\\";
        public static String ORDERS_PATH = "zamowienia\\";
        public static String DIR_OFFER_TEMPLATE = "szablon_oferty\\";
        public static String DIR_PROJECT_TEMPLATE = "szablon_projektu\\";

        public static string CreateBasePath(DIR_TYPE dir_type)
        {
            string dirPath = BASE_PATH;
            switch (dir_type)
            {
                case (DIR_TYPE.Offer):
                    dirPath += OFFERS_PATH;
                    break;
                case (DIR_TYPE.Order):
                    dirPath += ORDERS_PATH;
                    break;
                case (DIR_TYPE.Project):
                    dirPath += PROJECTS_PATH;
                    break;
                default:
                    dirPath += OFFERS_PATH;
                    break;
                   
            }

            return dirPath;
        }

        public static FileOperationStatus CreateDir(string dirname, DIR_TYPE dir_type) 
        {

            try
            {
                
                string dirPath = CreateBasePath(dir_type);
                
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


        public static FileOperationStatus CopyTemplate(string dirname, DIR_TYPE dir_type)

        {
            string basePath = CreateBasePath(dir_type);

            string destinationPath = basePath + dirname +"//";
            string DIR_TEMPLATE = dir_type == DIR_TYPE.Offer ? DIR_OFFER_TEMPLATE : DIR_PROJECT_TEMPLATE;
            try
            {
                string sourcePath = BASE_PATH + DIR_TEMPLATE;
              
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

        public static void setupProjectFolder(Offer offer)
        {
            CreateDir(offer.projectPath, DIR_TYPE.Project);
            CopyTemplate(offer.projectPath, DIR_TYPE.Project);
        }



        public static void OpenFolder(string dirname,DIR_TYPE dir_type = DIR_TYPE.Offer)
        {
            string dirPath = CreateBasePath(dir_type);
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

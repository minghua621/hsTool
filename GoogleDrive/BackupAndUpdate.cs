using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Common.Logger;

namespace GoogleDrive
{
    public static class BackupAndUpdate
    {
        public static bool UpdateDB(string localPath)
        {
            bool isSuccess = false;
            string fileName = System.IO.Path.GetFileName(localPath);
            string localDir = System.IO.Path.GetDirectoryName(localPath) + @"\";

            string query = string.Format("mimeType != 'application/vnd.google-apps.folder' and title contains '{0}'", fileName);
            IList<File> _Files = GoogleDriveHelper.RetrieveAllFiles(Settings.DriveService, query, "modifiedDate desc");
            if (_Files.Count > 0)
            {
                string downloadPath = string.Format("{0}{1}", localDir, _Files[0].Title);
                DateTime googleFileDate = DateTime.ParseExact(_Files[0].Title.Replace(string.Format("{0}_", fileName), ""), "yyyyMMddHHmmss", null);
                DateTime localFileDate = System.IO.File.GetLastWriteTime(localPath);
                localFileDate = new DateTime(localFileDate.Year, localFileDate.Month, localFileDate.Day, localFileDate.Hour, localFileDate.Minute, localFileDate.Second);

                if (localFileDate > googleFileDate) //upload
                {
                    query = "mimeType = 'application/vnd.google-apps.folder' and title = 'DB'";
                    _Files = GoogleDriveHelper.RetrieveAllFiles(Settings.DriveService, query);
                    if (_Files.Count > 0)
                    {
                        Log.Info("Local DB is newer than DB in google drive.");
                        string newPath = string.Format("{0}_{1}", localPath, localFileDate.ToString("yyyyMMddHHmmss"));
                        System.IO.File.Copy(localPath, newPath);
                        File upload = GoogleDriveHelper.UploadFile(Settings.DriveService, newPath, _Files[0].Id);
                        if (upload != null)
                        {
                            System.IO.File.Delete(newPath);
                            Log.Info("The lastest DB file completed uploading to google drive.");
                            isSuccess = true;
                        }
                        else
                        {
                            Log.Error("Uploading DB file failed");
                        }
                    }
                    else
                    {
                        Log.Error("DB folder doesn't exist.");
                    }
                }
                else if (localFileDate < googleFileDate) //download and replace
                {
                    if (GoogleDriveHelper.DownloadFile(Settings.DriveService, _Files[0], downloadPath))
                    {
                        Log.Info("Local DB is older than DB in google drive.");                        
                        System.IO.File.Delete(localPath);
                        System.IO.File.Move(downloadPath, localPath);
                        Log.Info("The lastest DB file completed downloading to local.");
                        isSuccess = true;
                    }
                    else
                    {
                        Log.Error("Downloading DB file failed");
                    }
                }
                else
                {
                    Log.Info("The lastest DB file exists.");
                    isSuccess = true;
                }
            }
            else 
            {
                Log.Error("Cannot find a DB file in google drive.");
            }
            return isSuccess;
        }
    }
}

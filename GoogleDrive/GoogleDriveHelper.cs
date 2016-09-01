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
    public class GoogleDriveHelper
    {
        public static List<File> RetrieveAllFiles(DriveService service, string search = "", string order = "", int maxResult = 0)
        {
            List<File> result = new List<File>();
            FilesResource.ListRequest request = service.Files.List();
            if (!string.IsNullOrEmpty(search))
            {
                request.Q = search;
            }
            if (maxResult != 0)
            {
                request.MaxResults = maxResult;
            }
            if (!string.IsNullOrEmpty(order))
            {
                request.OrderBy = order;
            }
            do
            {
                try
                {
                    FileList files = request.Execute();

                    result.AddRange(files.Items);
                    request.PageToken = files.NextPageToken;
                }
                catch (Exception e)
                {
                    Log.Error("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
            return result;
        }

        public static Boolean DownloadFile(DriveService _service, File _fileResource, string _saveTo)
        {
            if (!String.IsNullOrEmpty(_fileResource.DownloadUrl))
            {
                try
                {
                    var x = _service.HttpClient.GetByteArrayAsync(_fileResource.DownloadUrl);
                    byte[] arrBytes = x.Result;
                    System.IO.File.WriteAllBytes(_saveTo, arrBytes);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("An error occurred: " + e.Message);
                    return false;
                }
            }
            else
            {
                // The file doesn't have any content stored on Drive.
                return false;
            }
        }

        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        public static File UploadFile(DriveService _service, string _uploadFile, string _parent)
        {

            if (System.IO.File.Exists(_uploadFile))
            {
                File body = new File();
                body.Title = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "File uploaded by Diamto Drive Sample";
                body.MimeType = GetMimeType(_uploadFile);
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.InsertMediaUpload request = _service.Files.Insert(body, stream, GetMimeType(_uploadFile));
                    //request.Convert = true;   // uncomment this line if you want files to be converted to Drive format
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    Log.Error("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                Log.Error("File does not exist: " + _uploadFile);
                return null;
            }
        }


        public static string GetFolderID(DriveService service, String parentfolderId, string FolderName)
        {
            ChildrenResource.ListRequest request = service.Children.List(parentfolderId);
            request.Q = "mimeType='application/vnd.google-apps.folder' and title='" + FolderName + "' ";
            do
            {
                try
                {
                    ChildList children = request.Execute();

                    if (children != null && children.Items.Count > 0)
                    {

                        return children.Items[0].Id;
                    }

                    foreach (ChildReference child in children.Items)
                    {
                        Console.WriteLine("File Id: " + child.Id);
                    }
                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return string.Empty;
        }

    }
}

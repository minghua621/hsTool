using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Common.Logger;

namespace GoogleDrive
{
    public class MyOAuth2WebAuthorizationBroker : GoogleWebAuthorizationBroker
    {
        public new static async Task<UserCredential> AuthorizeAsync(ClientSecrets clientSecrets,
            IEnumerable<string> scopes, string user, CancellationToken taskCancellationToken,
            IDataStore dataStore = null)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
            };
            return await AuthorizeAsyncCore(initializer, scopes, user, taskCancellationToken, dataStore)
                .ConfigureAwait(false);
        }

        private static async Task<UserCredential> AuthorizeAsyncCore(
            GoogleAuthorizationCodeFlow.Initializer initializer, IEnumerable<string> scopes, string user,
            CancellationToken taskCancellationToken, IDataStore dataStore = null)
        {
            initializer.Scopes = scopes;
            initializer.DataStore = dataStore ?? new FileDataStore(Folder);
            var flow = new MyAuthorizationCodeFlow(initializer, user);

            // Create an authorization code installed app instance and authorize the user.
            return await new AuthorizationCodeInstalledApp(flow, new LocalServerCodeReceiver()).AuthorizeAsync
                (user, taskCancellationToken).ConfigureAwait(false);
        }
    }

    public class MyAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        private readonly string userId;

        /// <summary>Constructs a new Google authorization code flow.</summary>
        public MyAuthorizationCodeFlow(Initializer initializer, string userId)
            : base(initializer)
        {
            this.userId = userId;
        }

        public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
        {
            return new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
            {
                ClientId = ClientSecrets.ClientId,
                Scope = string.Join(" ", Scopes),
                //append user to url
                LoginHint = userId,
                RedirectUri = redirectUri
            };
        }
    }

    public static class Settings
    {
        public static DriveService DriveService { get { return driveService; } }
        private static DriveService driveService;

        public static bool Initialize(string filePath)
        {
            driveService = AuthenticateOauth(filePath);
            if (driveService == null)
            {
                Log.Logger.Info("AuthenticateOauth failed");
                return false; 
            }
            return true;
        }

        public static DriveService AuthenticateOauth(string jsonFile)
        {
            //Google Drive scopes Documentation:   https://developers.google.com/drive/web/scopes
            string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveAppsReadonly,   // view your drive apps
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts,  //// modify your app scripts
                                             "https://www.googleapis.com/auth/contacts.readonly"};

            try
            {
                // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
                UserCredential credential;
                using (var stream = new FileStream(jsonFile, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        "Admin",
                        CancellationToken.None,
                        new FileDataStore("Google.Drive.Auth.Store"),new LocalServerCodeReceiver()).Result;
                }

                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "hsTool",
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                return null;
            }
        }
    }
}

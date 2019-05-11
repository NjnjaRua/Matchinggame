using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FacebookManager : MonoBehaviour
{
    private static FacebookManager instance;
    
    private void Awake()
    {
        instance = this;
        Initialized();
    }

    public static FacebookManager GetInstance()
    {
        return instance;
    }

    #region Initialized

    public void Initialized()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Facebook couldn't initialize");
            },
            isGameShow =>
            {
                if (isGameShow)
                    Time.timeScale = 1;
                else
                    Time.timeScale = 0;
            });
        }
        else
        {
            FB.ActivateApp();
        }
    }
    #endregion

    #region Login
    
    public void FBLogin(System.Action<bool> callback)
    {
        var permissions = new List<string>()
        {
            "public_profile",
            "email",
            "user_friends"
        };
        if (!FB.IsLoggedIn || !CheckPermissionToken(permissions))
        {
            FB.LogInWithReadPermissions(permissions, result => {
                if (result.Error != null)
                {
                    Debug.Log("LOGIN is FAIL");
                    print(result.Error);
                    callback(false);
                    return;
                }
                if (FB.IsLoggedIn)
                {
                    // AccessToken class will have session details
                    var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                    // Print current access token's User ID
                    Debug.Log("aToken = " + aToken.UserId);
                    // Print current access token's granted permissions
                    foreach (string perm in aToken.Permissions)
                    {
                        Debug.Log(perm);
                    }
                    callback(true);
                }
                else
                {
                    Debug.Log("User cancelled login");
                }
            });
        }
    }

    public bool CheckPermissionToken(List<string> permms)
    {
        int count = permms.Count;
        for (int i = 0; i < count; i++)
        {
            if (!AccessToken.CurrentAccessToken.Permissions.Contains(permms[i]))
                return false;
        }
        return true;
    }


    #endregion

    #region LogOut
    public void FBLogOut()
    {
        FB.LogOut();
    }
    #endregion

    #region Share
    public void Share()
    {
        FB.ShareLink(
            new Uri("https://developers.facebook.com/"),
             "Title Test",
            "Content Test",
            callback: ShareCallback
        );
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }
    #endregion

    #region Inviting
    public void FBGameRequest()
    {
        FB.AppRequest("Come and play this awesome game", title: "Game request Title");
    }

    public void FBInvite(Action<bool> callback)
    {
        FB.Mobile.AppInvite(new Uri("https://play.google.com/store/apps/details?id=com.nntcollection.lonely"), null, result => {
            //call back
            if (result.Error != null)
            {
                print(result.Error);
                callback(false);
                return;
            }
            callback(true);
        });
    }
    #endregion

    #region Log an App Event
    public void LogEvent(string Id, string desc, string result)
    {
        var tutParams = new Dictionary<string, object>();
        tutParams[AppEventParameterName.ContentID] = Id;
        tutParams[AppEventParameterName.Description] = desc;
        tutParams[AppEventParameterName.Success] = result;

        FB.LogAppEvent(
            AppEventName.CompletedTutorial,
            parameters: tutParams
        );
    }
    #endregion

    #region GetData
    public bool IsFBConnected()
    {
        return FB.IsLoggedIn;
    }

    public void GetAvatar(Action<Texture2D> callBack)
    {
        if (!IsFBConnected())
            FBLogin(isSuccess=> { });
        //Get Avatar
        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, result => {
            if (result.Error != null)
            {
                print(result.Error);
                return;
            }
            callBack(result.Texture);
        });
    }

    public void GetFacebookName(Action<string> callBack)
    {
        if (!IsFBConnected())
            FBLogin(isSuccess => { });
        FB.API("/me", HttpMethod.GET, result => {
            if (result.Error != null)
            {
                print(result.Error);
                return;
            }
            callBack((string)result.ResultDictionary["name"]);
        });
    }

    public List<string> GetFriendsPlayingGame()
    {
        if (!IsFBConnected())
            FBLogin(isSuccess => { });
        List<string> friendName = new List<string>();
        string query = "me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendList = (List<object>)dictionary["data"];
            string strFriend = string.Empty;
            foreach (var dic in friendList)
            {
                friendName.Add((string)((Dictionary<string, object>)dic)["name"]);
            }            
        });
        return friendName;
    }
    #endregion
}

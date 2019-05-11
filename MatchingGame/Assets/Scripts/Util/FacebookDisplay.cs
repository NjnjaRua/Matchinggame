using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacebookDisplay : MonoBehaviour {

    [SerializeField]
    private Image avatar;

    [SerializeField]
    private Text txtFBName;

    FacebookManager fbManager;
    private void Start()
    {
        fbManager = FacebookManager.GetInstance();
    }

    public void UpdateFacebookUI()
    {
        if(fbManager == null)
            fbManager = FacebookManager.GetInstance();
        if (fbManager == null)
            return;
        Debug.Log("<color=green>UpdateFacebookUI </color>");
        if (fbManager.IsFBConnected())
        {
            fbManager.GetFacebookName(name => {
                txtFBName.text = name;
            });

            fbManager.GetAvatar(texture2D =>
            {
                avatar.sprite = Sprite.Create(texture2D, new Rect(0, 0, 128, 128), new Vector2(1f, 0.5f));
            });
        }
    }
}

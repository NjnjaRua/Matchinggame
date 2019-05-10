using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public struct AssetInfo
{
    public string key;
    public TextAsset value;
}

public class ConstantManager : MonoBehaviour {

    private static ConstantManager instance;

    [Header("Json Files")]
    [SerializeField]
    private List<AssetInfo> localTextAssets;
    public const string MAP_INFO = "MAP_INFO";
    public const string HINT_INFO = "HINT_INFO";
    static public string[] CONST_KEY_TO_LOAD = new string[]
    {
        MAP_INFO,
        HINT_INFO
    };
    private static Dictionary<string, JSONNode> constantsMapInfoDef = null;
    private static Dictionary<int, List<Dictionary<int, bool>>> dicMapIndexStatus = null;

    private static List<string> hintsDef = null;

    [Header("Cached Constants")]
    public Dictionary<string, byte[]> LocalConstantAsset;
    public Dictionary<string, byte[]> CachedConstantAsset;
    public Dictionary<string, string> CachedConstantMD5;
    public Dictionary<string, bool> ConstantDownloaded;
    public Dictionary<string, string> CachedFinalString = new Dictionary<string, string>();
    public static JSONNode cachedJson;

    [Header("Constant")]
    public const int MAX_ITERATION = 100;
    public const int MAX_TEXTURE_SIZE = 128;
    public const float DELAY_TIME = 0.4f;
    public const int NODE_SIZE = 72;

    [Header("Saved Keys")]
    public const string SAVE_BEST_SCORE = "SAVE_BEST_SCORE";

    [Header("Others")]
    public const string HINT_DEFAULT = "Looking for Three same nodes";

    [Header("EVENT")]
    public const string EVENT_UPDATE_SCORE = "EVENT_UPDATE_SCORE";

    private void Awake()
    {
        LocalConstantAsset = new Dictionary<string, byte[]>();
        CachedConstantAsset = new Dictionary<string, byte[]>();
        CachedConstantMD5 = new Dictionary<string, string>();
        ConstantDownloaded = new Dictionary<string, bool>();
        instance = this;
        Initialize();
        StartCoroutine(LoadAllConstant());
    }
	
	public static ConstantManager GetInstance()
    {
        return instance;
    }

    public void Initialize()
    {
        if (localTextAssets != null)
        {
            for (int i = 0, len = localTextAssets.Count; i < len; i++)
            {
                AddLocalConstantText(localTextAssets[i].key, localTextAssets[i].value);
            }
        }
    }

    private void AddLocalConstantText(string key, TextAsset content)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("AddLocalConstantText can not add null key");
            return;
        }

        if (content == null)
        {
            Debug.LogError("AddLocalConstantText can not add content null of key = " + key);
            return;
        }

        CachedConstantMD5[key] = Util.GetMd5Hash(content.bytes);
        LocalConstantAsset[key] = content.bytes;
    }

    private IEnumerator LoadAllConstant()
    {
        cachedJson = null;
        int count = CONST_KEY_TO_LOAD.Length;
        for (int i = 0; i < count; i++)
        {
            LoadConstantByKey(CONST_KEY_TO_LOAD[i]);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void LoadConstantByKey(string _key)
    {
        if (Array.IndexOf(CONST_KEY_TO_LOAD, _key) < 0)
            return;
        string cachedString = GetConstantString(_key, true);
        switch (_key)
        {
            case MAP_INFO:
                LoadMapInfo(cachedString);
                break;

            case HINT_INFO:
                LoadHintInfo(cachedString);
                break;

            default:
                break;
        }
        cachedString = null;
    }
       
    public string GetConstantString(string key, bool returnNullIfMatch = false)
    {
        string md5Hash = "", finalStr = "";
        if (CachedConstantMD5.TryGetValue(key, out md5Hash))
        {
            if (CachedFinalString.TryGetValue(md5Hash, out finalStr))
                return finalStr;
            byte[] bytes = GetConstantBytes(key, returnNullIfMatch);
            if (bytes != null && bytes.Length > 0)
            {
                finalStr = new string(Encoding.UTF8.GetChars(bytes));
                CachedFinalString[md5Hash] = finalStr;
            }
        }
        return finalStr;
    }

    public byte[] GetConstantBytes(string key, bool returnNullIfMatch = false)
    {
        byte[] bytes1 = null, bytes2 = null;

        if (CachedConstantAsset != null && CachedConstantAsset.Count > 0)
            CachedConstantAsset.TryGetValue(key, out bytes1);

        LocalConstantAsset.TryGetValue(key, out bytes2);

        if (bytes2 != null && bytes2.Length > 0)
            return bytes2;

        if (bytes1 != null && bytes1.Length > 0)
        {
            if ((bytes2 != null && bytes2.Length > 0) && Util.QuickCompare(bytes1, bytes2))
            {
                if (returnNullIfMatch)
                    return null;
                return bytes1;
            }
            else
            {
                return bytes1;
            }
        }
        return bytes2;
    }

    #region MAP INFO
    public static void LoadMapInfo(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText)) return;

        if (constantsMapInfoDef == null)
            constantsMapInfoDef = new Dictionary<string, JSONNode>();
        if (dicMapIndexStatus == null)
            dicMapIndexStatus = new Dictionary<int, List<Dictionary<int, bool>>>();

        JSONClass json = (JSONClass)JSON.Parse(jsonText);
        if (json == null)
            return;
        JSONNode node;
        List<Dictionary<int, bool>> mapIndexStatus = null;
        Dictionary<int, bool> dicRowStatus;
        int row, key;

        foreach (KeyValuePair<string, JSONNode> pair in json)
        {
            node = pair.Value;
            constantsMapInfoDef[pair.Key] = node;

            key = -1;
            int.TryParse(pair.Key, out key);

            JSONClass jRows = node as JSONClass;
            if (jRows != null && jRows.Count > 0)
            {
                mapIndexStatus = new List<Dictionary<int, bool>>();
                foreach (KeyValuePair<string, JSONNode> data in jRows)
                {
                    dicRowStatus = new Dictionary<int, bool>();
                    row = -1;
                    int.TryParse(data.Key, out row);
                    dicRowStatus[row] = data.Value.AsBool;
                    mapIndexStatus.Add(dicRowStatus);
                }
                if(mapIndexStatus != null && mapIndexStatus.Count > 0)
                {
                    dicMapIndexStatus[key] = mapIndexStatus;
                }
            }
        }
    }

    public static int GetWidthMap()
    {
        if (constantsMapInfoDef == null || constantsMapInfoDef.Count <= 0)
            return 0;
        JSONNode node = null;
        if(!constantsMapInfoDef.TryGetValue("width", out node))
        {
            return 0;
        }
        return node.AsInt;
    }

    public static int GetHeightMap()
    {
        if (constantsMapInfoDef == null || constantsMapInfoDef.Count <= 0)
            return 0;
        JSONNode node = null;
        if (!constantsMapInfoDef.TryGetValue("height", out node))
        {
            return 0;
        }
        return node.AsInt;
    }

    public static bool GetMapIndexStatus(int column, int row)
    {
        bool colorCell = true;
        if (column < 0 || row < 0 || dicMapIndexStatus == null || dicMapIndexStatus.Count <= 0)
            return colorCell;
        List<Dictionary<int, bool>> mapIndexStatus = null;
        if(!dicMapIndexStatus.TryGetValue(column, out mapIndexStatus))
        {
            return colorCell;
        }
        Dictionary<int, bool> dicRowStatus;
        bool status;
        for (int i = 0, len = mapIndexStatus.Count; i < len;i++)
        {
            dicRowStatus = mapIndexStatus[i];
            if (dicRowStatus == null || dicRowStatus.Count <= 0)
                continue;
            status = false;
            if(dicRowStatus.TryGetValue(row, out status))
            {
                return status;
            }
        }
        return colorCell;
    }

    #endregion

    #region HINT INFO
    public static void LoadHintInfo(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText)) return;

        if (hintsDef == null)
            hintsDef = new List<string>();

        JSONClass json = (JSONClass)JSON.Parse(jsonText);
        if (json == null)
            return;
        foreach (KeyValuePair<string, JSONNode> pair in json)
        {
            hintsDef.Add(pair.Value);
        }
    }

    public static List<string> GetHintConst()
    {
        return hintsDef;
    }
    #endregion

}

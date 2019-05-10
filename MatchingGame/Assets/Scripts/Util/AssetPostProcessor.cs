#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetPostProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        Debug.Log("<color=green>OnPreprocessTexture </color>" + assetPath);
        if(assetPath.Contains("Images/Nodes"))
        {
            TextureImporter _textureImporter = (TextureImporter)assetImporter;
            _textureImporter.maxTextureSize = ConstantManager.MAX_TEXTURE_SIZE;

            //todo: can define format, compress
        }
    }


}
#endif

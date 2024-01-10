using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : MonoBehaviour
{
    private const string BundleVersionKey = "Version";

    private AssetBundle _loadedBundle;
    private int _versionToSave = -1;

    public void DownloadAssetBundle(string assetBundleName, Action onSuccess, Action<float> onProgress,
        Action<string> onError)
    {
        StartCoroutine(DownloadAssetBundleAsync(assetBundleName, onSuccess, onProgress, onError));
    }

    private IEnumerator DownloadAssetBundleAsync(string assetBundleName, Action onSuccess, Action<float> onProgress,
        Action<string> onError)
    {
        yield return CheckBundleVersion(error =>
        {
            if (!IsFileExists(assetBundleName))
            {
                StopAllCoroutines();
                onError?.Invoke(error);
            }
        });
        if (IsFileExists(assetBundleName))
        {
            StartCoroutine(LoadAssetFromFiles(assetBundleName, onSuccess, onProgress, onError));
        }
        else
        {
            StartCoroutine(DownloadAssetBundleFromServerAsync(assetBundleName, onSuccess, onProgress, onError));
        }
    }

    private IEnumerator CheckBundleVersion(Action<string> onError)
    {
        var bundleVersionURL = NetworkConfig.GetWithDomain("/AssetBundles/BundleVersion.json");
        Debug.Log("try send request to :" + bundleVersionURL);
        UnityWebRequest request = UnityWebRequest.Get(bundleVersionURL);

        yield return request.SendWebRequest();

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            var serverVersion = JsonConvert.DeserializeObject<BundleVersion>(request.downloadHandler.text);
            var currentVersion = PlayerPrefs.GetInt(BundleVersionKey, -1);
            Debug.Log("server bundle version : " + serverVersion.Version);
            Debug.Log("local bundle version : " + currentVersion);
            if (currentVersion == -1 || serverVersion.Version != currentVersion)
            {
                ClearCache();
                _versionToSave = serverVersion.Version;
            }
        }

        request.Dispose();
    }

    private IEnumerator DownloadAssetBundleFromServerAsync(string assetBundleName, Action onSuccess,
        Action<float> onProgress,
        Action<string> onError)
    {
        var bundleURL = NetworkConfig.GetServerPath(assetBundleName);
        Debug.Log("try send request to :" + bundleURL);
        UnityWebRequest request = UnityWebRequest.Get(bundleURL);

        var progress = 0f;
        request.SendWebRequest();

        while (!request.isDone)
        {
            if (progress < request.downloadProgress)
            {
                progress = request.downloadProgress;
                onProgress?.Invoke(progress / 2f);
                yield return null;
            }
        }

        if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            onError?.Invoke(request.error);
        }
        else
        {
            SaveBundleToFiles(assetBundleName, request.downloadHandler.data);
            SaveBundleVersion();
            StartCoroutine(LoadAssetFromFiles(assetBundleName, onSuccess, onProgress, onError));
        }

        request.Dispose();
    }

    private void SaveBundleVersion()
    {
        if (_versionToSave != -1)
        {
            PlayerPrefs.SetInt(BundleVersionKey, _versionToSave);
            PlayerPrefs.Save();
            _versionToSave = -1;
        }
    }

    private void SaveBundleToFiles(string assetName, byte[] downloadHandlerData)
    {
        var platformFolder = NetworkConfig.GetPlatformFolder();
        var directoryPath = Application.persistentDataPath + $"/AssetBundles/{platformFolder}";
        var filePath = directoryPath + $"/{assetName}";

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllBytes(filePath, downloadHandlerData);
    }

    private IEnumerator LoadAssetFromFiles(string assetName, Action onSuccess, Action<float> onProgress, 
        Action<string> onError)
    {
        var platformFolder = NetworkConfig.GetPlatformFolder();
        var directoryPath = Application.persistentDataPath + $"/AssetBundles/{platformFolder}";
        var filePath = directoryPath + $"/{assetName}";

        AssetBundle.UnloadAllAssetBundles(true);

        var assetBundle = AssetBundle.LoadFromFileAsync(filePath);
        var progress = 0.5f;
        
        while (!assetBundle.isDone)
        {
            onProgress?.Invoke(Mathf.Clamp01(progress + 0.05f));
            yield return null;
        }
        
        onProgress?.Invoke(1f);
        yield return null;

        if (assetBundle.assetBundle == null)
        {
            var error = "Не удалось загрузить AssetBundle из файла: " + filePath;
            onError?.Invoke(error);
            Debug.LogError(error);
        }
        else
        {
            Debug.Log("bundle loaded from files");
            _loadedBundle = assetBundle.assetBundle;
            onSuccess?.Invoke();
        }
    }

    public GameObject LoadAsset(string assetName)
    {
        if (_loadedBundle != null)
        {
            return _loadedBundle.LoadAsset<GameObject>(assetName);
        }

        return null;
    }


    public Sprite LoadAssetSprite(string assetName)
    {
        if (_loadedBundle != null)
        {
            return _loadedBundle.LoadAsset<Sprite>(assetName);
        }

        Debug.LogError("bundle dont contains background named sprite");

        return null;
    }


    public bool IsFileExists(string assetName)
    {
        var platformFolder = NetworkConfig.GetPlatformFolder();
        var filePath = Application.persistentDataPath + $"/AssetBundles/{platformFolder}/{assetName}";
        return File.Exists(filePath);
    }

    public bool CurrentBundleIsLoaded(string assetName)
    {
        return _loadedBundle != null && _loadedBundle.name == assetName;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/ResourceManager/Clear Cache")]
#endif
    public static void ClearCache()
    {
        var dirPath = Application.persistentDataPath + "/AssetBundles";

        if (Directory.Exists(dirPath))
        {
            Debug.Log($"Delete dir {dirPath}");
            Directory.Delete(dirPath, true);
        }
        else
        {
            Debug.Log($"Dir {dirPath} already cleared!");
        }
    }
}
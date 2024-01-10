using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public partial class NetworkManager : Singleton<NetworkManager>
{
    private const string ApiHost = "";
    private const int UnauthorizedCode = 401;
    private const int TimeoutInSeconds = 10;

    public static IEnumerator SendGetRequestAsync<T>(string urlMethod,
        Action<T> onSuccess = null,
        Action<long> onError = null)
    {
        yield return CreateRequest(urlMethod, "GET", onSuccess, onError);
    }

    public static IEnumerator SendPostRequestAsync<T>(string urlMethod,
        Action<T> onSuccess = null,
        Action<long> onError = null,
        string payload = null)
    {
        yield return CreateRequest(urlMethod, "POST", onSuccess, onError, payload);
    }
    
    public static IEnumerator SendPutRequestAsync<T>(string urlMethod,
        Action<T> onSuccess = null,
        Action<long> onError = null,
        string payload = null)
    {
        yield return CreateRequest(urlMethod, "PUT", onSuccess, onError, payload);
    }

    public static IEnumerator SendPatchRequestAsync<T>(string urlMethod,
        Action<T> onSuccess = null,
        Action<long> onError = null,
        string payload = null)
    {
        yield return CreateRequest(urlMethod, "PATCH", onSuccess, onError, payload);
    }

    public static IEnumerator SendDeleteRequestAsync<T>(string urlMethod,
        Action<T> onSuccess = null,
        Action<long> onError = null,
        string payload = null)
    {
        yield return CreateRequest(urlMethod, "DELETE", onSuccess, onError, payload);
    }
    
    private static IEnumerator SendMultiformRequest<T>(string urlMethod, Action<T> onSuccess = null, Action<long> onError = null,  List<IMultipartFormSection> formData = null)
    {
        yield return CreateRequest(urlMethod, onSuccess, onError, formData);
        
    }
    
    private static IEnumerator CreateRequest<T>(string urlMethod,
        string httpVerb,
        Action<T> onSuccess = null,
        Action<long> onError = null,
        string serializedField = null)
    {
        var url = $"{ApiHost}{urlMethod}";
        var webRequest = new UnityWebRequest(url, httpVerb, new DownloadHandlerBuffer(), null);

        if (!string.IsNullOrEmpty(serializedField))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(serializedField));
        }

        Debug.Log($"[NetworkService] Try send request {url}");
        yield return Instance.StartCoroutine(SendRequest(webRequest, onSuccess, onError));
    }
    
    private static IEnumerator CreateRequest<T>(string urlMethod,
        Action<T> onSuccess = null,
        Action<long> onError = null,
        List<IMultipartFormSection> formData = null)
    {
        var url = $"{ApiHost}{urlMethod}";
        var webRequest = UnityWebRequest.Post(url, formData);
        Debug.Log($"[NetworkService] Try send request {url}");
        yield return Instance.StartCoroutine(SendRequest(webRequest, onSuccess, onError));
    }

    private static IEnumerator SendRequest<T>(UnityWebRequest webRequest, Action<T> onSuccess = null,
        Action<long> onError = null)
    {
        using (webRequest)
        {
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.useHttpContinue = false;
            webRequest.timeout = TimeoutInSeconds;

            yield return webRequest.SendWebRequest();

            var text = webRequest.downloadHandler.text;

            if (webRequest.responseCode == UnauthorizedCode)
            {
                Debug.LogWarning($"Authorization error code: {webRequest.error} on request {webRequest.url}");
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                OnNetworkError(webRequest, onError);
                yield break;
            }

            try
            {
                GetResultAsync(text, onSuccess);
                Debug.Log($"Success! {webRequest.url}");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                OnNetworkError(webRequest, onError);
            }
        }
    }
    
    private static async void GetResultAsync<T>(string json, Action<T> onSuccess = null)
    {
        var result = await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
        onSuccess?.Invoke(result);
    }

    private static void OnNetworkError(UnityWebRequest webRequest, Action<long> onError)
    {
        var serverMessage = webRequest.downloadHandler.text;
        Debug.Log($"Request {webRequest.url} <color=red>Error:</color> {webRequest.error} message : {serverMessage}");
        onError?.Invoke(webRequest.responseCode);
    }
}
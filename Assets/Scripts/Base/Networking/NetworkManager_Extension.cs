using System;
using System.Collections;
using Newtonsoft.Json;

public partial class NetworkManager
{
    public static class Example
    {
        private const string Method = "method";

        public static void GetModelData(ExampleNode node, Action<ExampleModel> onSuccess = null,
            Action<long> onError = null)
        {
            Instance.StartCoroutine(GetModelDataAsync(node, onSuccess, onError));
        }

        private static IEnumerator GetModelDataAsync(ExampleNode node,
            Action<ExampleModel> onSuccess = null,
            Action<long> onError = null)
        {
            var payload = JsonConvert.SerializeObject(node);
            yield return SendPostRequestAsync<ExampleModel>(Method, response =>
                {
                    onSuccess?.Invoke(response);
                },
                onError, payload);
        }
    }
}
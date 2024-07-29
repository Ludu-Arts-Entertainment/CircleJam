using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProcessDeepLinkManager : MonoBehaviour
{
    public static ProcessDeepLinkManager Instance { get; private set; }
    public string deeplinkURL;
    public delegate void DeepLinkActivated(Dictionary<string,string> parameters);
    public event DeepLinkActivated OnDeepLinkActivate;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                OnDeepLinkActivated(Application.absoluteURL);
            }
            // Initialize DeepLink Manager global variable.
            else deeplinkURL = "[none]";
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDeepLinkActivated(string url)
    {
        deeplinkURL = url;
        QueryParameters = GetQueryParameters(url);
        
#if !UNITY_EDITOR
        Debug.Log("DeepLink: " + url + "\nParameters:");
        foreach (var parameter in QueryParameters)
        {
            Debug.Log("Key: " + parameter.Key + " Value: " + parameter.Value);
        }
        Debug.Log("End of Parameters");
#endif
        
        OnDeepLinkActivate?.Invoke(QueryParameters);
        // if (_validRequestFunctions.Contains(QueryParameters.FirstOrDefault().Key))
        // {
        //     OnDeepLinkActivate?.Invoke(QueryParameters);
        // }
    }
    public Dictionary<string, string> QueryParameters=new();
    private Dictionary<string,string> GetQueryParameters(string url)
    {
        Dictionary<string,string> parameters = new Dictionary<string,string>();
        string[] urlParts = url.Split('?');
        if (urlParts.Length > 1)
        {
            string query = urlParts[1];
            foreach (string param in query.Split('&'))
            {
                string[] pair = param.Split('=');
                parameters.Add(pair.Length>0? pair[0]:"", pair.Length>1? pair[1]:"");
            }
        }
        return parameters;
    }
    
    [Button]
    public void Test()
    {
        OnDeepLinkActivated(deeplinkURL);
    }
}
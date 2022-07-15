using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.SceneManagement;

public class PanelController : MonoBehaviour
{
    public string assetName;
    public TextMeshProUGUI btnText;
    AsyncOperationHandle<long> getDownloadSize;
    // Start is called before the first frame update
    void Start()
    {
        //Addressables.ClearDependencyCacheAsync(assetName);
        Addressables.InitializeAsync().Completed += checkAssets;
    }

    public void CheckAssetIsDownloadedOrNot<T>(T t)
    {
        getDownloadSize = Addressables.GetDownloadSizeAsync(t);     
    }

    public void checkAssets(AsyncOperationHandle<IResourceLocator> obj)
    {
        CheckAssetIsDownloadedOrNot(assetName);
        print(getDownloadSize.Result);
        if (getDownloadSize.Result == 0)
        {
            Debug.Log("Loading New Scene");
            Addressables.LoadSceneAsync(assetName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Downloading New Scene");
            StartCoroutine(DownloadWithProgress(assetName));
        }

    }

    IEnumerator DownloadWithProgress(string asset)
    {
       AsyncOperationHandle operation = Addressables.DownloadDependenciesAsync(asset);
       float progress = 0;

        while (operation.Status == AsyncOperationStatus.None)
        {
            float percentageComplete = operation.GetDownloadStatus().Percent;
            if (percentageComplete > progress * 1.1) // Report at most every 10% or so
            {
                progress = percentageComplete; // More accurate %
                Debug.Log(progress);
                //ProgressEvent.Invoke(progress);
                btnText.text = "Download (" + progress + "%)";
            }
            yield return null;
        }
        operation.Completed += ResourceDownloadComplete;
    }

    private void ResourceDownloadComplete(AsyncOperationHandle obj)
    {
        Debug.Log("Download Complete");
        btnText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Addressables.LoadSceneAsync(assetName, LoadSceneMode.Single);
    }

    //IEnumerator DownloadAssetsProgress()
    //{
    //    var downloadPanel = myPanel.LoadAssetAsync<GameObject>();
    //    downloadPanel.Completed += AssetDownloaded;
    //    while (!downloadPanel.IsDone)
    //    {
    //        var status = downloadPanel.GetDownloadStatus();
    //        float progress = status.Percent;
    //        Debug.Log((int)progress*100);
    //        yield return null;
    //    }
    //}


    IEnumerator DownloadAsset(AssetReference _asset)
    {
        yield return assetName;
    }

    private void AssetDownloaded(AsyncOperationHandle<GameObject> obj)
    {
        //myPanel.InstantiateAsync(myPanel, transform);
    }

    private GameObject Loading()
    {
        throw new NotImplementedException();
    }
}

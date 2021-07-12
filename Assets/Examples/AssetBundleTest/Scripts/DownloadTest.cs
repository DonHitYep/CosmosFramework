﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Download;
using Cosmos;
using System.IO;
using UnityEngine.UI;
using System;


public class DownloadTest : MonoBehaviour
{
    [SerializeField]string srcUrl;
    [SerializeField]string downloadPath;
    [SerializeField]Slider slider;
    [SerializeField]Text text;
    [SerializeField]Text uriText;
    void  Start()
    {
        if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(downloadPath))
            return;
        if (!Directory.Exists(downloadPath))
            return;
        CosmosEntry.DownloadManager.DownloadSuccess += OnDownloadSucess;
        CosmosEntry.DownloadManager.DownloadFailure += OnDownloadFailure;
        CosmosEntry.DownloadManager.DownloadStart += OnDownloadStart;
        CosmosEntry.DownloadManager.DownloadOverall += OnDownloadOverall;
        CosmosEntry.DownloadManager.DownloadAndWriteFinish += OnDownloadFinish;
        CosmosEntry.DownloadManager.AddUrlDownload(srcUrl, downloadPath);
        CosmosEntry.DownloadManager.LaunchDownload();
    }
    void OnDownloadStart(DownloadStartEventArgs eventArgs)
    {
        if (uriText != null)
            uriText.text = eventArgs.URI;
    }
    void OnDownloadOverall(DonwloadOverallEventArgs eventArgs)
    {
        var overallProgress = (float)Math.Round(eventArgs.OverallProgress, 1);
        if (text != null)
        {
            text.text = overallProgress + "%";
        }
        if (slider != null)
        {
            slider.value = overallProgress;
        }
    }
    void OnDownloadSucess(DownloadSuccessEventArgs eventArgs)
    {
       Utility.Debug.LogInfo($"DownloadSuccess {eventArgs.URI}");
    }
    void OnDownloadFailure(DownloadFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($"DownloadFailure {eventArgs.URI}\n{eventArgs.ErrorMessage}");
    }
    void OnDownloadFinish(DownloadAndWriteFinishEventArgs eventArgs)
    {
        if (text != null)
        {
            text.text = "100%   Done";
        }
        Utility.Debug.LogInfo($"DownloadFinish {eventArgs.DownloadAndWriteTimeSpan}",MessageColor.GREEN);
    }
}
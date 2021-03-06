﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Quark
{
    /// <summary>
    /// Quark配置脚本，挂载到物体上配置即可；
    /// </summary>
    public class QuarkConfig:MonoBehaviour
    {
        [SerializeField] string url;
        [SerializeField] string downloadPath;
        static QuarkConfig instance;
        public static QuarkConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<QuarkConfig>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(QuarkConfig).Name);
                        instance= go.AddComponent<QuarkConfig>();
                    }
                }
                return instance;
            }
        }
         void Awake()
        {
            instance = this;
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("URL is invalid !");
            if(string.IsNullOrEmpty(downloadPath))
                throw new ArgumentNullException("DownloadPath is invalid !");
            QuarkManager.Instance.Initiate(url, downloadPath);
        }
    }
}

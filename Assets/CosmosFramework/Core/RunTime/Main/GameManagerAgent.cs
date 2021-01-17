﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
namespace Cosmos {
    /// <summary>
    /// GameManager是静态的，因此由此类代理完成生命周期
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1000)]
    public sealed class GameManagerAgent :MonoSingleton<GameManagerAgent>,IControllable
    {
        public bool IsPause { get; private set; }
        public bool Pause
        {
            get { return IsPause; }
            set
            {
                if (IsPause == value)
                    return;
                IsPause = value;
                if (IsPause)
                {
                    OnPause();
                }
                else
                {
                    OnUnPause();
                }
            }
        }
        event Action ApplicationQuitHandler;
        public void OnPause()
        {
            GameManager.Instance.OnPause();
        }
        public void OnUnPause()
        {
            GameManager.Instance.OnUnPause();
        }
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }
        public int ModuleCount { get { return GameManager.Instance.ModuleCount; } }
        /// <summary>
        /// 清除单个实例，有一个默认参数。
        /// 默认延迟为0，表示立刻删除、
        /// 仅在场景中删除对应对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t">默认参数，表示延迟</param>
        public static void KillObject(Object obj, float delay = 0)
        {
           GameManager .KillObject(obj, delay);
        }
        /// <summary>
        /// 立刻清理实例对象
        /// 会在内存中清理实例
        /// </summary>
        /// <param name="obj"></param>
        public static void KillObjectImmediate(Object obj)
        {
            GameManager.KillObjectImmediate(obj);
        }
        /// <summary>
        /// 清除一组实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public static void KillObjects<T>(List<T> objs) where T : Object
        {
            for (int i = 0; i < objs.Count; i++)
            {
              GameManager.KillObject (objs[i]);
            }
            objs.Clear();
        }
        public static void KillObjects<T>(HashSet<T> objs) where T : Object
        {
            foreach (var obj in objs)
            {
                GameManager.KillObject(obj);
            }
            objs.Clear();
        }
        protected override void OnDestroy()
        {
            //GameManager.Instance.Dispose();
        }
        private void Update()
        {
            if (IsPause)
                return;
            GameManager.Instance.OnRefresh();
        }
        private void FixedUpdate()
        {
            if (IsPause)
                return;
            GameManager.Instance.OnFixRefresh();
        }
        private void LateUpdate()
        {
            if (IsPause)
                return;
            GameManager.Instance.OnLateRefresh();
        }
        private void OnApplicationQuit()
        {
            try
            {
                GameManager.Instance.Dispose();
            }
            catch{}
            ApplicationQuitHandler?.Invoke();
        }
    }
}
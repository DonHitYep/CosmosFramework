﻿using UnityEngine;
using System.Collections;
using System;
using Cosmos.Event;
namespace Cosmos.Mono
{
    /// <summary>
    /// mono池对象
    /// </summary>
    [DisallowMultipleComponent]
    internal class MonoProvider : MonoBehaviour, IMonoProvider
    {
        public Coroutine PredicateCoroutine(Func<bool> handler, Action callBack)
        {
            return StartCoroutine(EnumPredicateCoroutine(handler, callBack));
        }
        /// <summary>
        /// 嵌套协程；
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        /// <returnsCoroutine></returns>
        public Coroutine PredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
        {
            return StartCoroutine(EnumPredicateNestCoroutine(predicateHandler, nestHandler));
        }
        public Coroutine DelayCoroutine(float delay, Action callBack)
        {
            return StartCoroutine(EnumDelay(delay, callBack));
        }
        public Coroutine StartCoroutine(Action handler)
        {
            return StartCoroutine(EnumCoroutine(handler));
        }
        /// <summary>
        /// 嵌套协程
        /// </summary>
        /// <param name="routine">执行条件</param>
        /// <param name="callBack">执行条件结束后自动执行回调函数</param>
        /// <returns>Coroutine</returns>
        public Coroutine StartCoroutine(Coroutine routine, Action callBack)
        {
            return StartCoroutine(EnumCoroutine(routine, callBack));
        }
        IEnumerator EnumDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
        IEnumerator EnumCoroutine(Coroutine routine, Action callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }
        IEnumerator EnumCoroutine(Action handler)
        {
            handler?.Invoke();
            yield return null;
        }
        IEnumerator EnumPredicateCoroutine(Func<bool> handler, Action callBack)
        {
            yield return new WaitUntil(handler);
            callBack();
        }
        /// <summary>
        /// 嵌套协程执行体；
        /// </summary>
        /// <param name="predicateHandler">条件函数</param>
        /// <param name="nestHandler">条件成功后执行的嵌套协程</param>
        IEnumerator EnumPredicateNestCoroutine(Func<bool> predicateHandler, Action nestHandler)
        {
            yield return new WaitUntil(predicateHandler);
            yield return StartCoroutine(EnumCoroutine(nestHandler));
        }
    }
}
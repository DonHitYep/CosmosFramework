﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 与unity耦合的实体对象，当前版本中使用的是此实体对象
    /// </summary>
    public interface IEntityObject: IReference, IRefreshable, IOperable,IControllable
    {
        void SetEntity(object entity);
       object GetEntity();
        void OnAttach(object parent);
        void OnAttachChild(object child);
        void OnDetach();
        void OnDetachChild(object child);
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
namespace Cosmos.Test
{
    [PrefabAsset("UI/Slot")]
    public class Slot : MonoBehaviour
    {
        public void SetupSlot(ItemDataSet item)
        {
            var itemComp = transform.Find("Item").GetComponent<Item>();
            itemComp.SetItem(item);
        }
        public ItemDataSet GetDateSet()
        {
            var itemComp = transform.Find("Item").GetComponent<Item>();
            return itemComp.ItemDataSet;
        }
    }
}
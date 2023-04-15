using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.InventorySystem.Items
{
    public static class ItemSort
    {
        public static int CompareByID(Item x, Item y)
        {
            if(x == null)
            {
                if(y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if(y==null)
                {
                    return 1;
                }
                else
                {
                    return x.id.CompareTo(y.id);
                }
            }
        }
    }
}

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
                    return 0; // x == y
                }
                else
                {
                    return -1; // y exists, x does not
                }
            }
            else
            {
                if(y==null)
                {
                    return 1; // x exists, y does not
                }
                else
                {
                    return x.id.CompareTo(y.id);
                }
            }
        }
    }
}

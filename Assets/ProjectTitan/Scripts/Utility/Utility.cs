using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Titan.Utility
{
    public static class ColorExtentions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g,color.b, alpha);
        }
    }

    public static class ObjectCloner
    {
        public static T SerializeClone<T>(T source)
        {
            if(!typeof(T).IsSerializable)
            {
                Debug.LogError($"Type must be serializable : {nameof(source)}");
            }

            if(source is null)
            {
                return default;
            }

            IFormatter formatter = new BinaryFormatter();
            using MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}

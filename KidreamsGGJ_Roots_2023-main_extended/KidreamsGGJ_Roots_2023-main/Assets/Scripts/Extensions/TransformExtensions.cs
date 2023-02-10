using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static void SetLocalPosX(this Transform trans, float x)
        {
            var pos = trans.localPosition;
            pos.x = x;
            trans.localPosition = pos;
        }
        
        public static void SetLocalPosY(this Transform trans, float y)
        {
            var pos = trans.localPosition;
            pos.y = y;
            trans.localPosition = pos;
        }
        
        public static void SetLocalPosZ(this Transform trans, float z)
        {
            var pos = trans.localPosition;
            pos.z = z;
            trans.localPosition = pos;
        }

        public static void SetLocalScaleX(this Transform trans, float x)
        {
            var scale = trans.localScale;
            scale.x = x;
            trans.localScale = scale;
        }

        public static void SetLocalScaleY(this Transform trans, float y)
        {
            var scale = trans.localScale;
            scale.y = y;
            trans.localScale = scale;
        }

        public static void SetLocalScaleZ(this Transform trans, float z)
        {
            var scale = trans.localScale;
            scale.z = z;
            trans.localScale = scale;
        }
    }
}
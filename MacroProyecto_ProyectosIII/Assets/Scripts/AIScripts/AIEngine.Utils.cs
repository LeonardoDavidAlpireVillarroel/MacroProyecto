namespace AIEngine.Utils
{
    public static class Vector2ConversionExtensions
    {
        public static System.Numerics.Vector2 ToNumerics(this UnityEngine.Vector2 v)
        {
            return new System.Numerics.Vector2(v.x, v.y);
        }

        public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 v)
        {
            return new UnityEngine.Vector2(v.X, v.Y);
        }

        public static System.Numerics.Vector2[] ToNumericsArray(this UnityEngine.Vector2[] array)
        {
            if (array == null) return null;

            var result = new System.Numerics.Vector2[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i].ToNumerics();
            }
            return result;
        }
    }
}

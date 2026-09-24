using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClockApp.Presentation.Drawing
{
    public static class FeatheredShapes
    {
        private const float MinimumMiterCosine = 0.25f;

        public static void AddConvexPolygon(VertexHelper vertexHelper, IReadOnlyList<Vector2> points, Color32 color, float feather)
        {
            var count = points.Count;
            var start = vertexHelper.currentVertCount;
            var transparent = Transparent(color);

            for (var i = 0; i < count; i++)
            {
                var offset = MiterOffset(points[(i + count - 1) % count], points[i], points[(i + 1) % count], feather * 0.5f);
                vertexHelper.AddVert(points[i] - offset, color, Vector4.zero);
                vertexHelper.AddVert(points[i] + offset, transparent, Vector4.zero);
            }

            for (var i = 1; i < count - 1; i++)
            {
                vertexHelper.AddTriangle(start, start + i * 2, start + (i + 1) * 2);
            }

            for (var i = 0; i < count; i++)
            {
                var inner = start + i * 2;
                var nextInner = start + (i + 1) % count * 2;
                vertexHelper.AddTriangle(inner, inner + 1, nextInner + 1);
                vertexHelper.AddTriangle(inner, nextInner + 1, nextInner);
            }
        }

        public static void AddRing(VertexHelper vertexHelper, Vector2 center, float innerRadius, float outerRadius, int segments, Color32 color, float feather)
        {
            var start = vertexHelper.currentVertCount;
            var transparent = Transparent(color);
            var halfFeather = feather * 0.5f;

            for (var i = 0; i < segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                vertexHelper.AddVert(center + direction * Mathf.Max(0f, innerRadius - halfFeather), transparent, Vector4.zero);
                vertexHelper.AddVert(center + direction * (innerRadius + halfFeather), color, Vector4.zero);
                vertexHelper.AddVert(center + direction * (outerRadius - halfFeather), color, Vector4.zero);
                vertexHelper.AddVert(center + direction * (outerRadius + halfFeather), transparent, Vector4.zero);
            }

            for (var i = 0; i < segments; i++)
            {
                var current = start + i * 4;
                var next = start + (i + 1) % segments * 4;

                for (var layer = 0; layer < 3; layer++)
                {
                    vertexHelper.AddTriangle(current + layer, current + layer + 1, next + layer + 1);
                    vertexHelper.AddTriangle(current + layer, next + layer + 1, next + layer);
                }
            }
        }

        public static void FillCircle(List<Vector2> points, Vector2 center, float radius, int segments)
        {
            points.Clear();

            for (var i = 0; i < segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                points.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
            }
        }

        private static Vector2 MiterOffset(Vector2 previous, Vector2 current, Vector2 next, float distance)
        {
            var incoming = OutwardNormal(previous, current);
            var outgoing = OutwardNormal(current, next);
            var miter = (incoming + outgoing).normalized;
            var cosine = Mathf.Max(Vector2.Dot(miter, incoming), MinimumMiterCosine);

            return miter * (distance / cosine);
        }

        private static Vector2 OutwardNormal(Vector2 from, Vector2 to)
        {
            var direction = (to - from).normalized;
            return new Vector2(direction.y, -direction.x);
        }

        private static Color32 Transparent(Color32 color)
        {
            return new Color32(color.r, color.g, color.b, 0);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClockApp.Presentation.Drawing
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class DiscGraphic : MaskableGraphic
    {
        private const int Segments = 48;

        [SerializeField] private float _feather = 1.25f;

        private readonly List<Vector2> _points = new List<Vector2>(Segments);

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = rectTransform.rect;
            var radius = Mathf.Min(rect.width, rect.height) * 0.5f - _feather;

            FeatheredShapes.FillCircle(_points, rect.center, radius, Segments);
            FeatheredShapes.AddConvexPolygon(vertexHelper, _points, color, _feather);
        }
    }
}

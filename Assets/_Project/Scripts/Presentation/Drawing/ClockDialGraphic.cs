using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClockApp.Presentation.Drawing
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ClockDialGraphic : MaskableGraphic
    {
        private const int TickCount = 60;
        private const int TicksPerHour = 5;
        private const int CircleSegments = 120;
        private const float DegreesPerTick = 6f;

        [SerializeField] private Color _faceColor = new Color32(27, 30, 41, 255);
        [SerializeField] private Color _rimColor = new Color32(47, 52, 69, 255);
        [SerializeField] private float _rimWidth = 8f;
        [SerializeField] private Color _minuteTickColor = new Color32(118, 126, 146, 255);
        [SerializeField] private Color _hourTickColor = new Color32(230, 232, 239, 255);
        [SerializeField] private float _tickInset = 22f;
        [SerializeField] private Vector2 _minuteTickSize = new Vector2(3f, 14f);
        [SerializeField] private Vector2 _hourTickSize = new Vector2(7f, 30f);
        [SerializeField] private float _feather = 1.25f;

        private readonly List<Vector2> _points = new List<Vector2>(CircleSegments);

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = rectTransform.rect;
            var center = rect.center;
            var radius = Mathf.Min(rect.width, rect.height) * 0.5f - _feather;

            FeatheredShapes.FillCircle(_points, center, radius, CircleSegments);
            FeatheredShapes.AddConvexPolygon(vertexHelper, _points, _faceColor * color, _feather);
            FeatheredShapes.AddRing(vertexHelper, center, radius - _rimWidth, radius, CircleSegments, _rimColor * color, _feather);

            var tickEnd = radius - _tickInset;

            for (var i = 0; i < TickCount; i++)
            {
                var isHourTick = i % TicksPerHour == 0;
                var size = isHourTick ? _hourTickSize : _minuteTickSize;
                var tickColor = (isHourTick ? _hourTickColor : _minuteTickColor) * color;
                AddTick(vertexHelper, center, i * DegreesPerTick, tickEnd - size.y, tickEnd, size.x, tickColor);
            }
        }

        private void AddTick(VertexHelper vertexHelper, Vector2 center, float angle, float innerRadius, float outerRadius, float width, Color tickColor)
        {
            var radians = angle * Mathf.Deg2Rad;
            var along = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
            var across = new Vector2(along.y, -along.x) * (width * 0.5f);
            var inner = center + along * innerRadius;
            var outer = center + along * outerRadius;

            _points.Clear();
            _points.Add(inner + across);
            _points.Add(outer + across);
            _points.Add(outer - across);
            _points.Add(inner - across);

            FeatheredShapes.AddConvexPolygon(vertexHelper, _points, tickColor, _feather);
        }
    }
}

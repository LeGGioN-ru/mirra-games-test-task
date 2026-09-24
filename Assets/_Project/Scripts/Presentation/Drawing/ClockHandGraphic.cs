using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClockApp.Presentation.Drawing
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ClockHandGraphic : MaskableGraphic
    {
        [SerializeField] private float _length = 180f;
        [SerializeField] private float _tailLength = 30f;
        [SerializeField] private float _baseWidth = 14f;
        [SerializeField] private float _tipWidth = 6f;
        [SerializeField] private float _feather = 1.25f;

        private readonly List<Vector2> _points = new List<Vector2>(4);

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            _points.Clear();
            _points.Add(new Vector2(_baseWidth * 0.5f, -_tailLength));
            _points.Add(new Vector2(_tipWidth * 0.5f, _length));
            _points.Add(new Vector2(-_tipWidth * 0.5f, _length));
            _points.Add(new Vector2(-_baseWidth * 0.5f, -_tailLength));

            FeatheredShapes.AddConvexPolygon(vertexHelper, _points, color, _feather);
        }
    }
}

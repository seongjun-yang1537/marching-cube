using System;
using UnityEngine;

namespace MCube
{
    public class GizmoContext : IDisposable
    {
        private readonly Color _prevColor;
        private readonly Matrix4x4 _prevMatrix;

        private bool _setColor;
        private bool _setMatrix;

        private GizmoContext()
        {
            _prevColor = Gizmos.color;
            _prevMatrix = Gizmos.matrix;
        }

        public static GizmoContext New() => new GizmoContext();

        public GizmoContext Color(Color color)
        {
            Gizmos.color = color;
            _setColor = true;
            return this;
        }

        public GizmoContext Matrix(Matrix4x4 matrix)
        {
            Gizmos.matrix = matrix;
            _setMatrix = true;
            return this;
        }

        public void Dispose()
        {
            if (_setColor)
                Gizmos.color = _prevColor;

            if (_setMatrix)
                Gizmos.matrix = _prevMatrix;
        }
    }
}

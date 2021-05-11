using u2vis.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace u2vis
{
    public class RevolvedCharts : BaseVisualizationView
    {
        #region Private Fields
        [SerializeField]
        private int _segments = 360;
        [SerializeField]
        private bool _usedCategorical = false;
        [SerializeField]
        private int _dimIndex;
        #endregion

        #region Public Properties
        public int Segments
        {
            get { return _segments; }
            set
            {
                _segments = value;
                Rebuild();
            }
        }

        public int DimIndex
        {
            get { return _dimIndex; }
            set
            {
                _dimIndex = value;
                Rebuild();
            }
        }
        #endregion

        [ContextMenu("Test")]
        protected override void RebuildVisualization()
        {
            if (_presenter == null || _presenter.NumberOfDimensions < 2)
            {
                Debug.LogError("Presenter is either null or has not enough dimensions to represent this visualization");
                return;
            }
            List<Vector3> points = new List<Vector3>();
            int offset = (int)_presenter.SelectedMinItem;
            int length = (int)_presenter.SelectedMaxItem - offset;
            float max = VisViewHelper.GetGlobalMaximum(_presenter);
            int dimNum = _presenter.NumberOfDimensions;

            for (int itemIndex = offset; itemIndex < length; itemIndex++)
            {
                float x = VisViewHelper.GetItemValueAbsolute(_presenter, 1, itemIndex, true) * _size.x;
                float y = VisViewHelper.GetItemValueAbsolute(_presenter, 0, itemIndex, true) * _size.y;
                if (_presenter.AxisPresenters[0].IsCategorical)
                    y = ((float)itemIndex / (float)length)*_size.y;
                points.Add(new Vector3(x,y,0));
                Debug.Log(points[points.Count-1]);
            }
            Debug.Log("points " + points.Count);
            Mesh mesh = CreateGeometry(points.ToArray(), _segments,false,true,_style,_usedCategorical,_dimIndex);
            mesh.name = "RotationalMesh";
            
            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            if (this.gameObject.transform.Find("innerSide") != null)
            {
                GameObject objToDestroy = this.gameObject.transform.Find("innerSide").gameObject;
                Destroy(objToDestroy);

#if UNITY_EDITOR
                DestroyImmediate(objToDestroy);
#endif
            }

            GameObject innerSide = new GameObject("innerSide");
            innerSide.transform.SetParent(this.gameObject.transform);
            innerSide.transform.localPosition=Vector3.zero;
            innerSide.layer = this.gameObject.layer;
            innerSide.transform.localRotation = Quaternion.Euler(0,0,0);
            var mfis = innerSide.AddComponent<MeshFilter>();
            var mris = innerSide.AddComponent<MeshRenderer>();
            mfis.sharedMesh = CreateGeometry(points.ToArray(), _segments, false, false,_style,_usedCategorical,_dimIndex);

            mris.material = GetComponent<MeshRenderer>().material;
        }

        private static Mesh CreateGeometry(Vector3[] points, int sides, bool smooth, bool outside, GenericVisualizationStyle style = null, bool isCateg = false, int dimIndex = 0, Mesh sharedMesh = null) //taken from DesginAR
        {
            if (points.Length < 2)
                return null;
            // smooth meshes have exactly the same points per side as the 2d contour,
            // while for non-smooth every point besides the first and last one is added
            // twice to the list of vertices
            int numPoints = smooth ? points.Length : (points.Length - 1) * 2;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> texcoords = new List<Vector2>();
            List<Color> colors = new List<Color>();
            // algorithm runs from zero to one for the angles, steps are determined by the number of sides
            float angleStep = 1.0f / sides;
            float pi2 = Mathf.PI * 2;
            int length = smooth ? points.Length : points.Length - 1;
            for (float angle = 0; angle <= 1.0f; angle += angleStep)
            {
                for (int i = 0; i < length; i++)
                {
                    float x, z;
                    CoordinateHelper.ToCartesianCoordinates(points[i].x, angle * pi2, out x, out z);
                    vertices.Add(new Vector3(x, points[i].y, z));
                    if (style != null && !isCateg)
                        colors.Add(style.GetColorContinous((float)i / (float)length));
                    if (style != null && isCateg)
                        colors.Add(style.GetColorCategorical(dimIndex, (float)i / (float)length));
                    texcoords.Add(new Vector2(angle, (float)i / (float)length));

                    // for non smooth points also add the next point to the list of vertices to create sharp edges
                    if (!smooth)
                    {
                        CoordinateHelper.ToCartesianCoordinates(points[i + 1].x, angle * pi2, out x, out z);
                        vertices.Add(new Vector3(x, points[i + 1].y, z));
                        if (style != null)
                            colors.Add(style.GetColorContinous((float)i / (float)length));
                        texcoords.Add(new Vector2(angle, (float)(i + 1) / (float)length));
                    }
                }
            }

            List<int> indices = new List<int>();
            length = smooth ? numPoints - 1 : numPoints;
            int step = smooth ? 1 : 2;
            for (int i = 0; i < sides; i++)
            {
                for (int j = 0; j < length; j += step)
                {
                    int s0 = numPoints * i;
                    int s1 = numPoints * (i + 1);
                    //if (i == _sides - 1)
                    //    s1 = 0;
                    if (outside)
                    {
                        indices.Add(s0 + j);
                        indices.Add(s0 + j + 1);
                        indices.Add(s1 + j);

                        indices.Add(s0 + j + 1);
                        indices.Add(s1 + j + 1);
                        indices.Add(s1 + j);
                    }
                    else
                    {
                        indices.Add(s0 + j);
                        indices.Add(s1 + j);
                        indices.Add(s0 + j + 1);

                        indices.Add(s0 + j + 1);
                        indices.Add(s1 + j);
                        indices.Add(s1 + j + 1);
                    }
                }
            }

            if (sharedMesh == null)
                sharedMesh = new Mesh();
            sharedMesh.Clear();
            sharedMesh.vertices = vertices.ToArray();
            sharedMesh.uv = texcoords.ToArray();
            sharedMesh.triangles = indices.ToArray();
            if (style != null)
                sharedMesh.colors = colors.ToArray();
            sharedMesh.RecalculateNormals();
            return sharedMesh;
        }
    }
}
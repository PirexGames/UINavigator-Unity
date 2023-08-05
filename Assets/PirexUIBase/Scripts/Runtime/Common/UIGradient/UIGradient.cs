using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/UIGradient")]
public class UIGradient : BaseMeshEffect
{
    [SerializeField] private Gradient m_gradient = new Gradient();
    [Range(-180f, 180f)]
    public float m_angle = 0f;
    public bool m_ignoreRatio = true;
    
    [SerializeField] private Graphic m_graphic;
    // Use this for initialization
    #if UNITY_EDITOR
    protected override void Reset () {
        m_graphic = GetComponent<Graphic>();
        base.Reset();
    }
    #endif
    
    public Gradient Gradient
    {
        get { return m_gradient; }
        set { m_gradient = value; m_graphic.SetVerticesDirty(); }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if(enabled)
        {
            Rect rect = graphic.rectTransform.rect;
            Vector2 dir = UIGradientUtils.RotationDir(m_angle);

            if (!m_ignoreRatio)
                dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

            UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

            UIVertex vertex = default(UIVertex);
            for (int i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex (ref vertex, i);
                Vector2 localPosition = localPositionMatrix * vertex.position;
                vertex.color *= m_gradient.Evaluate(localPosition.y);
                vh.SetUIVertex (vertex, i);
            }
        }
    }
}
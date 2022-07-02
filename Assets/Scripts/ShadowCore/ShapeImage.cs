using UnityEngine;
using UnityEngine.UI;

public class ShapeImage : Image
{
    // 倾斜偏移
    public float offset;

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);

        UIVertex vertex = new UIVertex();
        toFill.PopulateUIVertex(ref vertex, 1);
        vertex.position += Vector3.right * offset;
        toFill.SetUIVertex(vertex, 1);

        vertex = new UIVertex();
        toFill.PopulateUIVertex(ref vertex, 2);
        vertex.position += Vector3.right * offset;
        toFill.SetUIVertex(vertex, 2);
    }
}
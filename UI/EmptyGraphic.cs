using UnityEngine.UI;

//不产生drawcall 但是 可以接受点击
public class EmptyGraphic : MaskableGraphic
{
    protected EmptyGraphic()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }
}

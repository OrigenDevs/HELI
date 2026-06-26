using UnityEngine;

public class BordeRebote : MonoBehaviour
{
    [Range(0f, 1f)]
    public float rebote = 0.4f;

    void Awake()
    {
        PhysicMaterial material = new PhysicMaterial("BordeRebote");
        material.dynamicFriction = 0f;
        material.staticFriction = 0f;
        material.bounciness = rebote;
        material.frictionCombine = PhysicMaterialCombine.Minimum;
        material.bounceCombine = PhysicMaterialCombine.Maximum;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("suelo"))
        {
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
                col.material = material;
        }
    }
}

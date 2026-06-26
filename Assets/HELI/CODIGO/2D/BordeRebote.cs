using UnityEngine;

public class BordeRebote : MonoBehaviour
{
    [Range(0f, 1f)]
    public float rebote = 0.4f;
    public float fuerzaRebote = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("suelo")) return;

        foreach (ContactPoint contacto in collision.contacts)
        {
            if (contacto.normal.y < 0.5f)
            {
                Vector3 dirRebote = contacto.normal;
                dirRebote.y = 0f;
                dirRebote.Normalize();

                Vector3 v = rb.linearVelocity;
                if (Vector3.Dot(dirRebote, v.normalized) < 0.5f)
                    v += dirRebote * fuerzaRebote * rebote;
                rb.linearVelocity = v;
                return;
            }
        }
    }
}

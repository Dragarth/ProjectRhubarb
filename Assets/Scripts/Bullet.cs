using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private int firedBy = 0;
    [SerializeField]
    float damage = -10.0f;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Ground")
        {
            Destroy(gameObject);
        }
        if (col.transform.tag == "Player" && firedBy == 1)
        {
            col.transform.GetComponent<DroneController>().AdjustHealth(damage);
            Destroy(gameObject);
        }
        if (col.transform.tag == "Boss" && firedBy == 0)
        {
            col.transform.GetComponent<BossController>().AdjustHealth(damage);
            Destroy(gameObject);
        }
    }

    public void SetOrigin(int firedBy)
    {
        this.firedBy = firedBy;
    }
}

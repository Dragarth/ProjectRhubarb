using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class DroneController : MonoBehaviour
{
    [SerializeField]
    private bool effectedByGravity = false;
    [SerializeField]
    private float speed = 7.5f;
    [SerializeField]
    private GameObject droneArm;
    private InputController iC;
    bool setPlayer = false;
    private float vert;
    private float horiz;
    private float mouse_vertical;
    private float mouse_horizontal;
    private float turnAmount;

    float bulletFireDelay = 0.25f;
    bool canFire = true;

    [SerializeField]
    private GameObject bullet;

    private Rigidbody2D rB;

    // Use this for initialization
    void Start()
    {
        iC = this.GetComponent<InputController>();
        rB = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputController();
        if (iC != null && !setPlayer)
        {
            iC.SetPlayer(1);
            setPlayer = true;
        }
        if (effectedByGravity)
        {
            if (iC.LeftVertical() != 0)
            {
                rB.gravityScale = 0;
                rB.velocity = Vector2.zero;
                rB.angularVelocity = 0.0f;
            } else
            {
                rB.gravityScale = 0.2f;
            }
        } else
        {
            rB.gravityScale = 0;
            rB.velocity = Vector2.zero;
            rB.angularVelocity = 0.0f;
        }

        if (vert != 0 || horiz != 0)
        {
            droneArm.transform.eulerAngles = new Vector3(
                droneArm.transform.eulerAngles.x,
                droneArm.transform.eulerAngles.y,
                turnAmount - 90.0f
                );
        } else // the player is using a keyboard, lets follow the mouse instead
        {
            var objectPos = Camera.main.WorldToScreenPoint(droneArm.transform.position);
            var dir = Input.mousePosition - objectPos;
            droneArm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        }

    }

    void FixedUpdate()
    {
        this.transform.Translate(new Vector2(iC.LeftHorizontal() * speed * Time.deltaTime, iC.LeftVertical() * speed * Time.deltaTime));
    }

    void InputController()
    {
        vert = -iC.RightVertical();
        horiz = iC.RightHorizontal();

        var playerScreenPoint = Camera.main.WorldToViewportPoint(transform.position);
        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x < playerScreenPoint.x)
        {
            Debug.Log("to the left of the player");
        }
        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).y < playerScreenPoint.y)
        {
            Debug.Log("below the player");
        }

        turnAmount = (Mathf.Atan2(horiz, vert) * Mathf.Rad2Deg);
        if (horiz < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        } else if (horiz > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if ((iC.RightTrigger() > 0 && canFire) || (Input.GetButton("Fire1") && canFire))
        {
            canFire = false;
            StartCoroutine(HoldingRightTrigger());
        }
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 150, 100),
            "vertical: " + vert +
            "\nhorizontal: " + horiz +
            "\nturn amount: " + turnAmount);
    }

    IEnumerator HoldingRightTrigger()
    {
        FireBullet();
        yield return new WaitForSeconds(bulletFireDelay);
        canFire = true;
    }

    void FireBullet()
    {
        GameObject newBullet = Instantiate(bullet, droneArm.transform.position, droneArm.transform.rotation) as GameObject;
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(5f, 0f), ForceMode2D.Impulse);
        Destroy(newBullet, 5f);
    }


}

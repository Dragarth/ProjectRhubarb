using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    //Boss parts
    private Rigidbody2D rB;
    private InputController iC;
    public GameController gC;
    [SerializeField]
    private Transform[] bossArm;
    [SerializeField]
    private Transform[] bulletSpawn;
    [SerializeField]
    private Transform rocketSpawn;

    private bool setPlayer = false;

    //Control Options
    public int controllerNumber = 0;
    [SerializeField]
    private bool useKeyboard = false;

    //Spawnables for attacks
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject rocket;

    //Adjustable movement variables
    [SerializeField]
    private float speed = 7.5f;

    //Private movement variables
    private float vertRight;
    private float horizRight;
    private float vertLeft;
    private float horizLeft;
    private float arm1TurnAmount;
    private float arm2TurnAmount;

    //Attack cooldowns
    float bulletFireDelay = 0.25f;
    bool canFire1 = true;
    bool canFire2 = true;
    float specialCoolDown = 30.0f;
    float currentSpecialCoolDown = 30.0f;
    bool parked = false;

    float maxHealth = 1000f;
    float currentHealth = 1000f;

    void Start()
    {
        iC = GetComponent<InputController>();
        rB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (iC != null && !setPlayer)
        {
            iC.SetPlayer(controllerNumber);
            setPlayer = true;
        }
        InputController();
    }

    void InputController()
    {
        if (!useKeyboard) //Use XBox controller input
        {
            vertRight = -iC.RightVertical();
            horizRight = iC.RightHorizontal();
            vertLeft = -iC.LeftVertical();
            horizLeft = iC.LeftHorizontal();

            if(iC.PressedX())
            {
                parked = !parked;
            }

            if(!parked)
            {
                arm1TurnAmount = (Mathf.Atan2(horizRight, vertRight) * Mathf.Rad2Deg);
                if (vertRight != 0 || horizRight != 0)
                {
                    bossArm[0].transform.eulerAngles = new Vector3(
                        bossArm[0].transform.eulerAngles.x,
                        bossArm[0].transform.eulerAngles.y,
                        arm1TurnAmount - 90.0f
                        );
                    bossArm[1].transform.eulerAngles = new Vector3(
                        bossArm[1].transform.eulerAngles.x,
                        bossArm[1].transform.eulerAngles.y,
                        arm1TurnAmount - 90.0f
                        );
                }
            }
            else
            {
                arm1TurnAmount = (Mathf.Atan2(horizRight, vertRight) * Mathf.Rad2Deg);
                arm2TurnAmount = (Mathf.Atan2(horizLeft, vertLeft) * Mathf.Rad2Deg);
                if (vertRight != 0 || horizRight != 0)
                {
                    bossArm[0].transform.eulerAngles = new Vector3(
                        bossArm[0].transform.eulerAngles.x,
                        bossArm[0].transform.eulerAngles.y,
                        arm1TurnAmount - 90.0f
                        );
                    bossArm[1].transform.eulerAngles = new Vector3(
                        bossArm[1].transform.eulerAngles.x,
                        bossArm[1].transform.eulerAngles.y,
                        arm2TurnAmount - 90.0f
                        );
                }
            }
            
            if (iC.RightTrigger() > 0 && canFire1)
            {
                canFire1 = false;
                StartCoroutine(HoldingRightTrigger());
            }
            if (iC.LeftTrigger() > 0 && canFire2)
            {
                canFire2 = false;
                StartCoroutine(HoldingLeftTrigger());
            }
        }
        else // the player is using a keyboard, lets follow the mouse instead
        {
            var objectPos = Camera.main.WorldToScreenPoint(bossArm[0].transform.position);
            var dir = Input.mousePosition - objectPos;
            bossArm[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

            objectPos = Camera.main.WorldToScreenPoint(bossArm[1].transform.position);
            dir = Input.mousePosition - objectPos;
            bossArm[1].transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

            //Attack input detection
            if (Input.GetButton("Fire1") && canFire1)
            {
                canFire1 = false;
                StartCoroutine(HoldingRightTrigger());
            }
            if (Input.GetButton("Fire2") && canFire2)
            {
                canFire2 = false;
                StartCoroutine(HoldingLeftTrigger());
            }

            //Flip drone sprite to face direction of arm
            if (dir.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (dir.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    void FixedUpdate()
    {
        if(!parked)
        {
            if (!useKeyboard)
                this.transform.Translate(new Vector2(iC.LeftHorizontal() * speed * Time.deltaTime, 0f));
            else
                this.transform.Translate(new Vector2(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0f));
        }
    }

    IEnumerator HoldingRightTrigger()
    {
        FireBullet(0);
        yield return new WaitForSeconds(bulletFireDelay);
        canFire1 = true;
    }

    IEnumerator HoldingLeftTrigger()
    {
        FireBullet(1);
        yield return new WaitForSeconds(bulletFireDelay);
        canFire2 = true;
    }

    void FireBullet(int armToFireFrom)
    {
        GameObject newBullet = Instantiate(bullet, bulletSpawn[armToFireFrom].transform.position, bossArm[armToFireFrom].transform.rotation) as GameObject;
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(7f, 0f), ForceMode2D.Impulse);
        newBullet.GetComponent<Bullet>().SetOrigin(1);
        Destroy(newBullet, 5f);
    }

    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        gC.UpdatePlayerHealth(controllerNumber, maxHealth, currentHealth);
    }

    public void AdjustHealth(float amountToAdjust)
    {
        currentHealth += amountToAdjust;
        gC.UpdatePlayerHealth(controllerNumber, maxHealth, currentHealth);
    }
}

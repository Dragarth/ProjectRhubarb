using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class DroneController : MonoBehaviour
{
    //Component holders
    private Rigidbody2D rB;
    private InputController iC;
    public GameController gC;
    [SerializeField]
    private GameObject droneArm;

    private bool setPlayer = false;

    //Control Options
    public int controllerNumber = 0;
    [SerializeField]
    private bool canFly = true;
    [SerializeField]
    private bool useKeyboard = false;
    [SerializeField]
    private bool useTwinStick = true;

    //Adjustable movement variables
    [SerializeField]
    private float speed = 7.5f;
    [SerializeField]
    private float jumpStrength = 5f;
    [SerializeField]
    private bool canDoubleJump = false;

    //Private movement variables
    private float vert;
    private float horiz;
    private float turnAmount;
    [SerializeField]
    private float currentGroundDist = 0f;
    private float normalGroundDist = 0.457f;
    private bool grounded = false;
    private bool doubleJump = false;
    
    //Attack cooldowns
    float bulletFireDelay = 0.25f;
    bool canFire = true;
    float maxLaserCharge = 5f;
    float laserCharge = 5f;

    float maxHealth = 100f;
    float currentHealth = 100f;

    //Spawnable attacks
    [SerializeField]
    private GameObject bullet;

    void Start()
    {
        iC = GetComponent<InputController>();
        rB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (iC != null && !setPlayer)
        {
            iC.SetPlayer(controllerNumber);
            setPlayer = true;
        }
        
        if (canFly)
            InputControllerFlying();
        else
        {
            if (useTwinStick)
                InputControllerNoneFlyingTwinStick();
            else
                InputControllerNoneFlyingNoneTwinStick();
        }
            
    }

    void FixedUpdate()
    {
        if(canFly)
        {
            rB.gravityScale = 0f;
            if (!useKeyboard)
                this.transform.Translate(new Vector2(iC.LeftHorizontal() * speed * Time.deltaTime, iC.LeftVertical() * speed * Time.deltaTime));
            else
            {
                
                float horizontal = 0;
                float vertical = 0;
                if (Input.GetKeyDown(KeyCode.D))
                    horizontal = 1;
                if (Input.GetKeyDown(KeyCode.A))
                    horizontal = -1;
                if (Input.GetKeyDown(KeyCode.W))
                    vertical = 1;
                if (Input.GetKeyDown(KeyCode.S))
                    vertical = -1;
                this.transform.Translate(new Vector2(horizontal * speed * Time.deltaTime, vertical * speed * Time.deltaTime));
            }
                
        }
        else
        {
            rB.gravityScale = 3f;
            if (!useKeyboard)
                this.transform.Translate(new Vector2(iC.LeftHorizontal() * speed * Time.deltaTime, 0f));
            else
            {
                float horizontal = 0;
                if (Input.GetKeyDown(KeyCode.D))
                    horizontal = 1;
                if (Input.GetKeyDown(KeyCode.A))
                    horizontal = -1;
                this.transform.Translate(new Vector2(horizontal * speed * Time.deltaTime, 0f));

            }
        }
    }

    void InputControllerFlying()
    {
        if (!useKeyboard) //Use XBox controller input
        {
            vert = -iC.RightVertical();
            horiz = iC.RightHorizontal();
            if (vert != 0 || horiz != 0)
            {
                droneArm.transform.eulerAngles = new Vector3(
                    droneArm.transform.eulerAngles.x,
                    droneArm.transform.eulerAngles.y,
                    turnAmount - 90.0f
                    );
            }
            turnAmount = (Mathf.Atan2(horiz, vert) * Mathf.Rad2Deg);
            if (horiz < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (horiz > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            if (iC.RightTrigger() > 0 && canFire)
            {
                canFire = false;
                StartCoroutine(HoldingRightTrigger());
            }
        }
        else // the player is using a keyboard, lets follow the mouse instead
        {
            var objectPos = Camera.main.WorldToScreenPoint(droneArm.transform.position);
            var dir = Input.mousePosition - objectPos;
            droneArm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
            
            //Attack input detection
            if (Input.GetButton("Fire1") && canFire)
            {
                canFire = false;
                StartCoroutine(HoldingRightTrigger());
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

    void InputControllerNoneFlyingTwinStick()
    {
        //Check if player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Ground")
            {
                currentGroundDist = Mathf.Abs(hit.point.y - transform.position.y);
                if (currentGroundDist < normalGroundDist && rB.velocity.y <= 0)
                {
                    grounded = true;
                    doubleJump = true;
                }
            }
        }
        if (!useKeyboard) //Use XBox controller input
        {
            vert = -iC.RightVertical();
            horiz = iC.RightHorizontal();
            if (vert != 0 || horiz != 0)
            {
                droneArm.transform.eulerAngles = new Vector3(
                    droneArm.transform.eulerAngles.x,
                    droneArm.transform.eulerAngles.y,
                    turnAmount - 90.0f
                    );
            }
            turnAmount = (Mathf.Atan2(horiz, vert) * Mathf.Rad2Deg);
            if (horiz < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (horiz > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            if (iC.RightTrigger() > 0 && canFire)
            {
                canFire = false;
                StartCoroutine(HoldingRightTrigger());
            }
            if (iC.LeftVertical() > 0.25f && grounded)
            {
                grounded = false;
                rB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }   
        }
        else // the player is using a keyboard, lets follow the mouse instead
        {
            var objectPos = Camera.main.WorldToScreenPoint(droneArm.transform.position);
            var dir = Input.mousePosition - objectPos;
            droneArm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

            //Attack input detection
            if (Input.GetButton("Fire1") && canFire)
            {
                canFire = false;
                StartCoroutine(HoldingRightTrigger());
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
            if (Input.GetAxis("Vertical") > 0 && grounded)
            {
                grounded = false;
                rB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
    }

    void InputControllerNoneFlyingNoneTwinStick()
    {
        //Check if player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit.transform != null)
        {
            if (hit.transform.tag == "Ground")
            {
                currentGroundDist = Mathf.Abs(hit.point.y - transform.position.y);
                if (currentGroundDist < normalGroundDist && rB.velocity.y <= 0)
                {
                    grounded = true;
                    doubleJump = true;
                }
            }
        }
        if (!useKeyboard) //Use XBox controller input
        {
            vert = -iC.LeftVertical();
            horiz = iC.LeftHorizontal();
            if (vert != 0 || horiz != 0)
            {
                droneArm.transform.eulerAngles = new Vector3(
                    droneArm.transform.eulerAngles.x,
                    droneArm.transform.eulerAngles.y,
                    turnAmount - 90.0f
                    );
            }
            if (vert > -0.5f && vert < 0.05f)
            {
                vert = 0;
                if (horiz > 0)
                {
                    horiz = 1;
                }
                else if(horiz < 0)
                {
                    horiz = -1;
                }
            }
            else
            {
                horiz = 0f;
                if(vert <= -0.5f)
                {
                    vert = -1f;
                }
                else if(vert >= 0.5f)
                {
                    vert = 1f;
                }
            }
            turnAmount = (Mathf.Atan2(horiz, vert) * Mathf.Rad2Deg);
            if (horiz < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (horiz > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            if (iC.RightTrigger() > 0 && canFire)
            {
                canFire = false;
                StartCoroutine(HoldingRightTrigger());
            }
            if (iC.PressedA() && grounded)
            {
                grounded = false;
                rB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
        else // the player is using a keyboard, lets follow the mouse instead
        {
            var objectPos = Camera.main.WorldToScreenPoint(droneArm.transform.position);
            var dir = Input.mousePosition - objectPos;
            droneArm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

            //Attack input detection
            if (Input.GetButton("Fire1") && canFire)
            {
                canFire = false;
                StartCoroutine(HoldingRightTrigger());
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
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                grounded = false;
                rB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            }
        }
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
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(7f, 0f), ForceMode2D.Impulse);
        newBullet.GetComponent<Bullet>().SetOrigin(0);
        Destroy(newBullet, 5f);
    }

    public void ResetPlayer(bool fly, bool twin)
    {
        currentHealth = maxHealth;
        gC.UpdatePlayerHealth(controllerNumber, maxHealth, currentHealth);
        canFly = fly;
        useTwinStick = twin;
    }

    public void AdjustHealth(float amountToAdjust)
    {
        currentHealth += amountToAdjust;
        gC.UpdatePlayerHealth(controllerNumber, maxHealth, currentHealth);
    }
}

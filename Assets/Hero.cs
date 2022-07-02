using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{

    
    public float m_walkSpeed = 2.0f;
    public float m_jumpForce = 7.5f;

    public bool m_noBlood = false;

    public GameObject myLight;
    public Transform lightPos;
    public Transform lightPos1;
    public Transform lightPos2;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private SpriteRenderer m_SR;
    private SpriteRenderer light_SR;
    private Sensor_Prototype m_groundSensor;
    private Sensor_Prototype m_wallSensorR1;
    private Sensor_Prototype m_wallSensorR2;
    private Sensor_Prototype m_wallSensorL1;
    private Sensor_Prototype m_wallSensorL2;
    public bool carryLight = true;
    public bool Grounded = false;
    public bool Moving = false;
    private bool m_dead = false;
    
    private bool m_crouching = false;

    private int m_facingDirection = 1;

    private float m_gravity;
    public float m_maxSpeed = 4.5f;

    public HeroAnimEvents animEvents;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_SR = GetComponentInChildren<SpriteRenderer>();
        light_SR = myLight.GetComponent<SpriteRenderer>();
        m_gravity = m_body2d.gravityScale;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Prototype>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Prototype>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Prototype>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Prototype>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Prototype>();

        carryLight = true;

        animEvents = GetComponent<HeroAnimEvents>();
    }

    // Update is called once per frame
    void Update()
    {

        // Respawn Hero if dead


        if (m_dead)
            return;

        //Check if character just landed on the ground
        if (!Grounded && m_groundSensor.State())
        {
            Grounded = true;
            m_animator.SetBool("Grounded", Grounded);
        }

        //Check if character just started falling
        if (Grounded && !m_groundSensor.State())
        {
            Grounded = false;
            m_animator.SetBool("Grounded", Grounded);
        }

        // -- Handle input and movement --
        float inputX = 0.0f;

        
        inputX = Input.GetAxis("Horizontal");

        // GetAxisRaw returns either -1, 0 or 1
        float inputRaw = Input.GetAxisRaw("Horizontal");

        // Check if character is currently moving
      /*  if (Mathf.Abs(inputRaw) > Mathf.Epsilon && Mathf.Sign(inputRaw) == m_facingDirection)
            Moving = true;
        else
            Moving = false;*/

        if(inputRaw!=0 && Grounded)
        {
            Moving = true;
        }

        // 改变玩家的方向
        if (inputRaw > 0 )
        {
            m_SR.flipX = true;
            
            if(carryLight)
            {
                light_SR.flipX = false;
                lightPos = lightPos2;
                myLight.transform.position = lightPos.position;
            }
            
            m_facingDirection = 1;
            


        }
        else if (inputRaw < 0 )
        {
            m_SR.flipX = false;
           
            if(carryLight)
            {
                light_SR.flipX = true;
                lightPos = lightPos1;
                myLight.transform.position = lightPos.position;
            }
            
            m_facingDirection = -1;
            
        }

        // SlowDownSpeed helps decelerate the characters when stopping
        float SlowDownSpeed = Moving ? 1.0f : 0.5f;
        // Set movement
            m_body2d.velocity = new Vector2(inputX * m_maxSpeed * SlowDownSpeed, m_body2d.velocity.y);

        // Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // Set Animation layer for hiding sword


        // Check if all sensors are setup properly


       


        //交互键
        if(Input.GetKeyDown("e") )
        {
            if(Mathf.Abs(transform.position.x - myLight.transform.position.x) <1)
            {
                if (carryLight)
                {
                    myLight.transform.SetParent(null);
                    carryLight = false;
                }
                else
                {
                    myLight.transform.SetParent(transform);
                    myLight.transform.position = lightPos.position;
                    carryLight = true;
                }
            }
            
        }
        //影子变实体
        else if (Input.GetKeyDown("q") )
        {
 
        }
        //调节灯的高度
        else if(Input.GetKeyDown("i"))
        {

        }
        else if(Input.GetKeyDown("k"))
        {

        }

        //Jump
        else if (Input.GetButtonDown("Jump") && Grounded )
        {
                 
            m_body2d.velocity = new Vector2(-m_facingDirection * m_jumpForce / 2.0f, m_jumpForce);
            m_facingDirection = -m_facingDirection;
            m_SR.flipX = !m_SR.flipX;
            

            m_animator.SetTrigger("Jump");
            Grounded = false;
            m_animator.SetBool("Grounded", Grounded);
            m_groundSensor.Disable(0.2f);
        }
        else if(Moving)
        {
            
            m_animator.SetInteger("AnimState", 2);

        }
        else
            m_animator.SetInteger("AnimState", 0);
        
        //Crouch / Stand up
        /* else if (Input.GetKeyDown("s") && m_grounded && !m_dodging && !m_ledgeGrab && !m_ledgeClimb && m_parryTimer < 0.0f)
         {
             m_crouching = true;
             m_animator.SetBool("Crouching", true);
             m_body2d.velocity = new Vector2(m_body2d.velocity.x / 2.0f, m_body2d.velocity.y);
         }
         else if (Input.GetKeyUp("s") && m_crouching)
         {
             m_crouching = false;
             m_animator.SetBool("Crouching", false);
         }*/
        //Idle

        

    }

    // Function used to spawn a dust effect
    // All dust effects spawns on the floor
    // dustXoffset controls how far from the player the effects spawns.
    // Default dustXoffset is zero
    public void SpawnDustEffect(GameObject dust, float dustXOffset = 0, float dustYOffset = 0)
    {
        if (dust != null)
        {
            // Set dust spawn position
            Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * m_facingDirection, dustYOffset, 0.0f);
            GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity) as GameObject;
            // Turn dust in correct X direction
            newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(m_facingDirection, 1, 1);
        }
    }


    // Called in AE_resetDodge in PrototypeHeroAnimEvents

    void RespawnHero()
    {
        transform.position = Vector3.zero;
        m_dead = false;
        m_animator.Rebind();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;
    public float power;
    public float sensitivity;
    public Animator jumpSquish;
    public ParticleSystem dust;
    public GameObject explode;
    //public GameObject particleBlast;
    public int dotsValut;

    private float interval = 1 / 30.0f;

    private bool aiming = false;
    private bool isShot = false;

    private Vector3 startPos;
    private List<GameObject> dotsObjects = new List<GameObject>();

    public GameObject dotPrefab;
    public Transform dotsTransform;
    public GameManager gameManager;

    private int basketCounter = 0;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.GetComponent<Rigidbody2D>().isKinematic = true;
        transform.GetComponent<Collider2D>().enabled = false;
        startPos = transform.position;

        float tempValue = 1f;

        for (int i = 0; i < dotsValut; i++)
        {
            GameObject dot = Instantiate(dotPrefab);
            dot.transform.parent = dotsTransform;
            if (i > 10)
            {
                dot.transform.localScale = new Vector2(tempValue, tempValue);
                tempValue -= 0.05f;
            }

            dotsObjects.Add(dot);
        }

        dotsTransform.gameObject.SetActive(false);
        GameManager.isStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShot)
        {
            Aim();

          //  x = transform.localEulerAngles.z;
        }
       //var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
       //var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
       //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

         //float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
       // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
       if (isShot)
        {
            //Vector2 direction = rb.velocity;
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg -90;
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, angleAxis, Time.deltaTime*50);
            //transform.rotation = Quaternion.Euler(0, 0, 5);
        }
    }


    void Aim()
    {
        if (Input.GetMouseButton(0))
        {
            if (!aiming)
            {
                aiming = true;
                startPos = Input.mousePosition;
                CalculatePath();
                ShowPath();
            }
            else
            {
                CalculatePath();
            }

            ///var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            // var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //float flyangle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(flyangle, Vector3.forward); 


            // Vector2 jumbaPosition = transform.position;
            // Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //  Vector2 direction = mousePosition - jumbaPosition;
            //transform.up = direction;

        }
        else if (aiming)
        {
            AudioController.Instance.PlayAudio(AudioController.Instance.shotSound);
            dust.Play();
            transform.GetComponent<Rigidbody2D>().isKinematic = false;
            transform.GetComponent<Collider2D>().enabled = true;
            transform.GetComponent<Rigidbody2D>().AddForce(GetForce(Input.mousePosition));
            isShot = true;
            aiming = false;
            HidePath();
            //stretching animation
            jumpSquish.SetTrigger("jumpStretch");



        }

    }

    Vector2 GetForce(Vector3 mouse)
    {
        return (new Vector2(startPos.x, startPos.y) - new Vector2(mouse.x, mouse.y)) * power;
      
    }

    bool inReleaseZone(Vector2 mouse)
    {
        if (mouse.x <= 70)
            return true;

        return false;
    }

    //Calculate path
    void CalculatePath()
    {
        dotsTransform.gameObject.SetActive(true);

        Vector2 vel = GetForce(Input.mousePosition) * Time.fixedDeltaTime / GetComponent<Rigidbody2D>().mass;
        for (int i = 0; i < dotsValut; i++)
        {
            float t = i / 30f;
            Vector3 point = PathPoint(transform.position, vel, t);
            point.z = -1.0f;
            dotsObjects[i].transform.position = point;


            //this makes character rotate acording to mouse 
            Vector3 jumbaPosition = transform.position;
            Vector2 direction = point - jumbaPosition;
            transform.up = direction;

            //var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            //  var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Vector3 mouse = Input.mousePosition;
            // Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
            // Vector3 offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
            //float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            //transform.eulerAngles = new Vector3(0, 0, angle);
        }

    }

    //Get point position
    Vector2 PathPoint(Vector2 startP, Vector2 startVel, float t)
    {
        return startP + startVel * t + 0.5f * Physics2D.gravity * t * t;
    }

    //Hide all used dots
    void HidePath()
    {
        dotsTransform.gameObject.SetActive(false);
    }

    //Show all used dots
    void ShowPath()
    {
        dotsTransform.gameObject.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            AudioController.Instance.PlayAudio(AudioController.Instance.bounseSound);
            gameManager.GameOver();
        }
        if (collision.gameObject.tag == "Rim")
        {
            AudioController.Instance.PlayAudio(AudioController.Instance.rimSound);
        }
        if (collision.gameObject.tag == "Wall")
        {
            AudioController.Instance.PlayAudio(AudioController.Instance.bounseSound);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Net")
        {
            isShot = false;
          
           // ParticleSystem leaf = collision.gameObject.GetComponent<ParticleSystem>();
            //leaf.Play();
            // collision.gameObject.GetComponent<ParticleSystem>().Play();
            //collision.gameObject.GetComponent<Animator>().SetTrigger("IsGoal");
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            //making it stick to the branch
            transform.GetComponent<Rigidbody2D>().isKinematic = true;
            transform.GetComponent<Collider2D>().enabled = false;
            rb.velocity = Vector3.zero;
            transform.localPosition = collision.gameObject.transform.position;
            //Gives initial position and rotation
            //transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //transform.SetParent(collision.transform);

            ScoreManager.Instance.AddScore(1);
            CameraFollow.cameraIsMove = true;
            AudioController.Instance.PlayAudio(AudioController.Instance.netSound);

            // Create new basket
            /* GameObject basket = BasketPoolManager.Instace.GetPooledObject();
             if (basket != null)
             {
                 switch (basketCounter)
                 {
                     case 0:
                         basket.transform.position = new Vector2(-2.5f, transform.position.y + Random.Range(4f, 6f));
                         basket.transform.rotation = Quaternion.Euler(0, 0, -30f);
                         basket.SetActive(true);
                         basketCounter++;
                         break;
                     case 1:
                         basket.transform.position = new Vector2(2.5f, transform.position.y + Random.Range(4f, 6f));
                         basket.transform.rotation = Quaternion.Euler(0, 0, 30f);
                         basket.SetActive(true);
                         basketCounter = 0;

                         break;
                 }
             } */
        }


        if (collision.gameObject.tag == "Spike")
        {
            //explode.Play();
           Instantiate(explode, transform.position, Quaternion.identity);
          // Instantiate(particleBlast, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }
            // Add star
            if (collision.gameObject.tag == "Star")
        {
            collision.gameObject.SetActive(false);
            MoneyManager.Instance.AddMoney(10);
            AudioController.Instance.PlayAudio(AudioController.Instance.coinSound);
        }
    }
}

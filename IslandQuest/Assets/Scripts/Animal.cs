using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public enum AnimalType { Bear, Crab, Any, None, Type };
    public AnimalType Type;
    public Transform Target = null;

    [Header("Base Stats")]
    public int HealthMax = 200;
    public int Health;
    public int Damage = 50;
    public int SearchRange;
    public int AtkRange;
    public int ChaseRange;
    public float SearchSpeed = 2;
    public float ChaseSpeed = 5;
    public float AttackSpeed = 10;

    [Header("Do not change")]
    public float Speed;
    public Canvas animalUI;
    public Image HealthBarRender1;
    public Image HealthBarRender2;
    public Slider HealthBar;
    public Animator Anim;

    public float CurrentStunTime;
    public bool isAttacking = false;
    public bool isStunned = false;

    public GameObject skinDrop;
    public GameObject meatDrop;

    private BoxCollider2D _hurtBox;
    Vector2 heading;
    float time;
    float stunTime = 0;
    private bool _facingRight;
    private bool _isDead = false;
    public Player PlayerHit;

    public bool inHerd = false;
    [SerializeField]
    private bool _isRetreat = false;
    [SerializeField]
    private bool isIdle = false;
    [SerializeField]
    private Collider2D damageCollider;

    private Transform[] nearbyCreatures;
    Transform tMin = null;
    float dist;

    private bool stopRetreat = false;

    public readonly static HashSet<Animal> Pool = new HashSet<Animal>();

    public AudioClip bearAttack;
    public AudioSource bearAttackSource;
    public AudioClip bearSick;

    private void OnEnable()
    {
        Pool.Add(this);
    }
    private void OnDisable()
    {
        Pool.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _hurtBox = gameObject.GetComponent<BoxCollider2D>();
        animalUI = transform.Find("Canvas").GetComponent<Canvas>();
        animalUI.worldCamera = GameObject.Find("/Player/P1 Camera").GetComponent<Camera>();
        HealthBarRender1.enabled = false;
        HealthBarRender2.enabled = false;
        HealthBar.maxValue = HealthMax;
        Health = HealthMax;
        damageCollider.enabled = false;
        bearAttackSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= HealthMax / 3 && !_isRetreat && !stopRetreat)
        {
            _isRetreat = true;
            StartCoroutine(PanicTimer());
        }

        if (isStunned)
        {
            stunTime -= Time.deltaTime;
            Anim.SetBool("isIdling", true);
            transform.position = Vector2.MoveTowards(transform.position, transform.position, 0 * Time.deltaTime);
            if (stunTime <= 0)
            {
                isStunned = false;
                Anim.SetBool("isIdling", false);
            }
        }
        else if (!_isDead)
        {            
            if (Target == null && heading.x < transform.position.x && _facingRight)
            {
                FlipCharacter("Left");
            }
            else if (Target == null && heading.x > transform.position.x && !_facingRight)
            {
                FlipCharacter("Right");
            }

            if (!_isRetreat)
            {
                if (Target != null && Target.position.x < transform.position.x && _facingRight)
                {
                    FlipCharacter("Left");
                }
                else if (Target != null && Target.position.x > transform.position.x && !_facingRight)
                {
                    FlipCharacter("Right");
                }
            }
            else
            {
                if (Target != null && Target.position.x < transform.position.x && !_facingRight)
                {
                    FlipCharacter("Right");
                }
                else if (Target != null && Target.position.x > transform.position.x && _facingRight)
                {
                    FlipCharacter("Left");
                }
            }
            

            if (Target == null && isIdle)
            {
                transform.position = Vector2.MoveTowards(transform.position, heading, SearchSpeed * Time.deltaTime);
                if (new Vector2(transform.position.x, transform.position.y) == heading)
                {
                    Anim.SetBool("isMoving", false);
                    Anim.SetBool("isIdling", true);
                }
            }
            if (Target == null && !isIdle)
            {   
                isIdle = true;
                StartCoroutine(IdleRoutine(Random.Range(1, 4), SearchSpeed));
                dist = -1;
            }
            else if (Target != null)
            {
                isIdle = false;
                StopCoroutine(IdleRoutine(Random.Range(1, 4), SearchSpeed));                
                Anim.SetBool("isIdling", false);
                dist = Vector2.Distance(transform.position, Target.position);
            }

            if (Target != null && dist > ChaseRange && dist != -1)
            {
                Anim.SetBool("isMoving", true);
                time += Time.deltaTime;

                if (time > 2.0f)
                {
                    CalculateHeading();
                    time = 0f;
                }
                //Debug.Log(heading);
                if (!_isRetreat)
                    transform.position = Vector2.MoveTowards(transform.position, heading, SearchSpeed * Time.deltaTime);
                else
                    transform.position = Vector2.MoveTowards(transform.position, -Target.transform.position, SearchSpeed * Time.deltaTime);
            }

            if (Target != null && dist < ChaseRange && dist > AtkRange)
            {
                Anim.SetBool("isMoving", true);
                if (!_isRetreat)
                    transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, ChaseSpeed * Time.deltaTime);
                else
                    transform.position = Vector2.MoveTowards(transform.position, -Target.transform.position, ChaseSpeed * Time.deltaTime);
            }


            if (Target != null && dist < AtkRange && !isAttacking)
            {
                if (!_isRetreat)
                {                    
                    transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, ChaseSpeed * Time.deltaTime);
                    Anim.SetBool("isMoving", false);
                    Anim.SetBool("isAttacking", true);
                }                   
                else
                    transform.position = Vector2.MoveTowards(transform.position, -Target.transform.position, ChaseSpeed * Time.deltaTime);
                
            }
        }       
    }

    private void FixedUpdate()
    {
        Transform nearestNeighbor;
        if (FindClosestPlayer(transform.position) != null)
        {
            nearestNeighbor = FindClosestPlayer(transform.position).GetComponent<Transform>();
            Target = nearestNeighbor;
        }        
        else if (FindClosestTrap(transform) != null)
        {
            nearestNeighbor = FindClosestTrap(transform).GetComponent<Transform>();
            Target = nearestNeighbor;
        }
        else if (FindClosestAnimal(transform) != null)
        {
            nearestNeighbor = FindClosestAnimal(transform).GetComponent<Transform>();
            Target = nearestNeighbor;
        }
        else    
            Target = null;
    }

    public void TakeDamage(int damage)
    {       
        if (_isDead) return;
        print("animal hurt");
        Health -= damage;
        HealthBar.value = Health;
        if (Health < HealthMax)
        {
            HealthBarRender1.enabled = true;
            HealthBarRender2.enabled = true;
        }       
        if (Health <= 0)
        {
            Anim.SetBool("isAttacking", false);
            Anim.SetBool("isMoving", false);
            Anim.SetBool("isIdling", false);
            Anim.SetTrigger("isDead");

            //ChallengesManager.Instance.CheckForChallenge(Type, PlayerHit);
            _isDead = true;
            transform.position = Vector2.MoveTowards(transform.position, transform.position, 0 * Time.deltaTime);
        }
    }
    public void Stun(float stunValue)
    {
        Anim.SetBool("isAttacking", false);
        Anim.SetBool("isMoving", false);
        Anim.SetBool("isIdling", true);

        stunTime = stunValue;
        isStunned = true;
    }

    public void CalculateHeading()
    {
        heading = new Vector2(Target.position.x, Target.position.y);
        heading += Random.insideUnitCircle * 6;
    }

    public void Idle()
    {
        heading = new Vector2(transform.position.x, transform.position.y);
        heading += Random.insideUnitCircle * Random.Range(5, 10);
    }

    public void AttackEnd()
    {
        isAttacking = false;
        damageCollider.enabled = false;
        Speed = 0;
        Stun(1.5f);
    }

    private void FlipCharacter(string side)
    {
        if (side == "Left")
        {
            _facingRight = false;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (side == "Right")
        {
            _facingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void SetAttack()
    {
        isAttacking = true;
        bearAttackSource.PlayOneShot(bearAttack);
        damageCollider.enabled = true;
    }

    private void CleanUp()
    {
        var dropCheck = Random.Range(0, 99);

        if (dropCheck <= 49)
        {
            GameObject newMeat = Instantiate(meatDrop, transform.position, Quaternion.identity) as GameObject;
        }
        if (dropCheck <= 74 && Type != AnimalType.Crab)
        {
            GameObject newSkin = Instantiate(skinDrop, transform.position, Quaternion.identity) as GameObject;
        }

        Destroy(gameObject);
    }

    public IEnumerator IdleRoutine(int rate, float speed)
    {        
        while (isIdle)
        {
            Idle();
            Anim.SetBool("isIdling", false);
            Anim.SetBool("isMoving", true);
            yield return new WaitForSeconds(rate);
        }        
    }

    public IEnumerator PanicTimer()
    {
        yield return new WaitForSeconds(10);
        stopRetreat = true;
        _isRetreat = false;
    }

    public Animal FindClosestAnimal(Transform pos)
    {
        Animal result = null;
        float dist = Mathf.Infinity;
        var e = Pool.GetEnumerator();
        while (e.MoveNext())
        {
            float d = (e.Current.transform.position - pos.position).sqrMagnitude;
            if (d < dist)
            {
                result = e.Current;
                dist = d;
            }
        }

        if (result != null && result.Type != pos.GetComponent<Animal>().Type && dist < SearchRange)
            return result;
        else
            //Debug.Log(result);
            return null;
    }

    public Player FindClosestPlayer(Vector3 pos)
    {
        Player result = null;
        float dist = Mathf.Infinity;
        var e = Player.PlayerPool.GetEnumerator();
        while (e.MoveNext())
        {
            //Debug.Log(e.Current);
            float d = (e.Current.transform.position - pos).sqrMagnitude;
            if (d < dist)
            {
                result = e.Current;
                dist = d;
            }
        }
        if (result != null && dist < SearchRange)
            return result;
        else
            //Debug.Log(result);
            return null;
    }

    public Trap FindClosestTrap(Transform pos)
    {
        Trap result = null;
        float dist = Mathf.Infinity;
        var e = Trap.TrapPool.GetEnumerator();
        while (e.MoveNext())
        {
            float d = (e.Current.transform.position - pos.position).sqrMagnitude;
            if (d < dist)
            {
                result = e.Current;
                dist = d;
            }
        }

        if (result != null && dist < SearchRange)
            return result;
        else
            //Debug.Log(result);
            return null;
    }
}

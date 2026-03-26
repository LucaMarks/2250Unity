using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : Actor //this also gives us access to MonoBehavoiour
{
    public LayerMask whatIsEnemy;

    public int Currency = 100;//default currency
    private List<string> skills;
    public Inventory inventory = new Inventory();
    private int SkillPoints;
    
    //refernce to the XP system attatched to this game object
    public ProgressionSystem progressionSystem;//this isn't set up rn and is not connected on unity

    //this works in tandem with input listeners in Actor
    private Transform cameraPivot;
    float lookSensitivity = 3f;
    private float pitch;
    private float yaw;
    private int attackAnimationCooldown;

	//reference to the dialogue system in the scene
	private DialogueSystem dialogueSystem;

    public PlayerInput playerMouse;
    public InputAction mouseAction;
    public InputAction numberKeyAction;
    
    // public InputAction

    public GameObject sword;
    public GameObject leftArm;
    public GameObject rightArm;

    private float swordSwingDistanceX = 1f;
    private float swordSwingDistanceY = 0.6f;
    private float swordSwingAngle = 35f;
    private float swordSwingDuration = 0.35f;
    private Coroutine swordSwingRoutine;
    private Vector3 swordStartLocalPos;
    private Quaternion swordStartLocalRot;
    private Vector3 leftArmStartLocalPos;
    private Quaternion leftArmStartLocalRot;
    private Vector3 rightArmStartLocalPos;
    private Quaternion rightArmStartLocalRot;
    private bool swordCached;

    private const string SolidObjectTag = "SolidObject";
    private bool isCollidingSolid;
    private Vector3 preMovePosition;
    private Vector3 lastMoveDelta;

    // private float yRotation;
    // public Player(int speed, int health, int damage, float xRotation , float yRotation) : base(health, damage, xRotation, yRotation) //i don't think this gets called when the game starts
    // {
    //     // Debug.Log("Player created");
    //     //
    // }

    private void Awake()
    {
        Debug.Log("Player created");
        if (sword == null){Debug.Log("Player sword not created. Player sword is equal to null");}
        
        //xp system
        if (progressionSystem == null)
        {
            progressionSystem = GetComponent<ProgressionSystem>();
        }

		//find DialogueSystem
		dialogueSystem = FindFirstObjectByType<DialogueSystem>();

        // base(health, damage, xRotation, yRotation);
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");

        playerOrientation = GetComponent<PlayerInput>();
        orientationAction = playerOrientation.actions.FindAction("Orientation");
        if (cameraPivot == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraPivot = cam.transform;
            }
        }

        playerMouse = GetComponent<PlayerInput>();
        mouseAction = playerMouse.actions.FindAction("MouseClick");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        preMovePosition = transform.position;
        Damage = 50;
    }    

    private void HazardCollide(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent<Hazard>(out var hazard))
        {
            Debug.Log($"Player collided with hazard: {hazard.name}");
            //handle hazard collision cases
        }
    }

    public override void Update()
    {
        base.Update();
        attackCooldown++;
        attackAnimationCooldown++;
        //swing sword
        /*
            this may cause issues in the future since mouseAction can account for any action
                -> mostly including other mouse buttons pressed 
         */
        if (mouseAction.triggered)
        {
            if (attackAnimationCooldown > 60)
            {
                attackAnimationCooldown = 0;
                Attack();
            }
        }

    }

    public override void Attack()
    {
        base.Attack();
        if (sword == null)
        {
            return;
        }

        if (!swordCached)
        {
            swordStartLocalPos = sword.transform.localPosition;
            swordStartLocalRot = sword.transform.localRotation;

            leftArmStartLocalPos = leftArm.transform.localPosition;
            leftArmStartLocalRot = leftArm.transform.localRotation;
            rightArmStartLocalPos = rightArm.transform.localPosition;
            rightArmStartLocalRot = rightArm.transform.localRotation;
            swordCached = true;
            
        }

        if (swordSwingRoutine != null)
        {
            StopCoroutine(swordSwingRoutine);
        }

        swordSwingRoutine = StartCoroutine(SwingSword());
    }
    
    //placeholder method for gaining monster kill XP
    public void OnEnemyKilled()
    {
        if (progressionSystem != null){progressionSystem.AwardCombatXP();}//to avoid crashes
        
    }
    
    //debug method to display current player progression stats in console
    public void DisplayStats()
    {
        Debug.Log($"Level: {progressionSystem.currentLevel} | XP: {progressionSystem.currentXP}/{progressionSystem.xpToNextLevel} | Health: {Health} | Damage: {Damage}");
    }
    
    private IEnumerator SwingSword()
    {
        
        float half = swordSwingDuration * 0.5f;
        float t = 0f;

        Vector3 targetPos = swordStartLocalPos + new Vector3(-swordSwingDistanceX, -swordSwingDistanceY, 0f);
        Quaternion targetRot = swordStartLocalRot * Quaternion.Euler(0f, -swordSwingAngle, -swordSwingAngle);

        Vector3 leftArmTargetPos = leftArmStartLocalPos + new Vector3(-swordSwingDistanceX, -swordSwingDistanceY, 0f);
        Quaternion leftArmTargetRot = leftArmStartLocalRot * Quaternion.Euler(0f, -swordSwingAngle, -swordSwingAngle);

        Vector3 rightArmTargetPos = rightArmStartLocalPos + new Vector3(swordSwingDistanceX, -swordSwingDistanceY, 0f);
        Quaternion rightArmTargetRot = rightArmStartLocalRot * Quaternion.Euler(0f, swordSwingAngle, swordSwingAngle);

        // Swing out
        while (t < half)
        {
            checkCollisionWithEnemy(); 
            float lerp = t / half;
            sword.transform.localPosition = Vector3.Lerp(swordStartLocalPos, targetPos, lerp);
            sword.transform.localRotation = Quaternion.Slerp(swordStartLocalRot, targetRot, lerp);
            leftArm.transform.localPosition = Vector3.Lerp(leftArmStartLocalPos, leftArmTargetPos, lerp);
            leftArm.transform.localRotation = Quaternion.Slerp(leftArmStartLocalRot, leftArmTargetRot, lerp);
            rightArm.transform.localPosition = Vector3.Lerp(rightArmStartLocalPos, rightArmTargetPos, lerp);
            rightArm.transform.localRotation = Quaternion.Lerp(rightArmStartLocalRot, rightArmTargetRot, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        // Swing back
        t = 0f;
        while (t < half)
        {
            checkCollisionWithEnemy();
            float lerp = t / half;
            sword.transform.localPosition = Vector3.Lerp(targetPos, swordStartLocalPos, lerp);
            sword.transform.localRotation = Quaternion.Slerp(targetRot, swordStartLocalRot, lerp);
            leftArm.transform.localPosition = Vector3.Lerp(leftArmTargetPos, leftArmStartLocalPos, lerp);
            leftArm.transform.localRotation = Quaternion.Slerp(leftArmTargetRot, leftArmStartLocalRot, lerp);
            rightArm.transform.localPosition = Vector3.Lerp(rightArmTargetPos, rightArmStartLocalPos, lerp);
            rightArm.transform.localRotation = Quaternion.Lerp(rightArmTargetRot, rightArmStartLocalRot, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        sword.transform.localPosition = swordStartLocalPos;
        sword.transform.localRotation = swordStartLocalRot;
        leftArm.transform.localPosition = leftArmStartLocalPos;
        leftArm.transform.localRotation = leftArmStartLocalRot;
        rightArm.transform.localPosition = rightArmStartLocalPos;
        rightArm.transform.localRotation = rightArmStartLocalRot;
        swordSwingRoutine = null;

    }

    private void checkCollisionWithEnemy()
    {
        //check collision from sword to ennemy/actor
        Vector3 swordScale = sword.transform.localScale;
        double swordRadius = Math.Sqrt(swordScale.x * swordScale.x + swordScale.y * swordScale.y + swordScale.z * swordScale.z);
        swordRadius *= 2;//increase this if colliosn detection with enemy is too buggy
        // Debug.Log(swordRadius);
        Collider[] hits = Physics.OverlapSphere(sword.transform.position, (int)swordRadius, whatIsEnemy);
        if(hits.Length > 0)
        {
            // Debug.Log("We hit something!");
        }
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyAI enemyObject = hits[i].GetComponent<EnemyAI>();
            if (enemyObject != null)
            {
                // enemyObject.Health -= Damage;
                if (attackCooldown > 60)//decrease this value if swings are not doing damage
                {
                    attackCooldown = 0;
                    Debug.Log(enemyObject.name + "at " + enemyObject.Health + " - " + Damage);
                    enemyObject.TakeDamage(Damage);
                }
            }
        }
    }
    public override void Move()
    {
        //camera and player body movement
        Vector2 dir = moveAction.ReadValue<Vector2>();

        Vector3 moveDirection = transform.forward*dir.y + transform.right*dir.x;
        
        preMovePosition = transform.position;
        lastMoveDelta = moveDirection * Speed * Time.deltaTime;
        transform.position += lastMoveDelta;

        Vector2 look = orientationAction.ReadValue<Vector2>() * lookSensitivity;
        yaw += look.x;
        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        if (!isCollidingSolid)
        {
            preMovePosition = transform.position;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected");
        HandleSolidCollision(collision);
    }

    public void OnCollisionStay(Collision collision)
    {
        HandleSolidCollision(collision);
    }
    

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(SolidObjectTag))
        {
            isCollidingSolid = false;
        }

        HazardCollide(collision);        
        // throw new NotImplementedException();
        //check other cases of collision
        // if player walks into an NPC, start dialogue directly
        if (collision.gameObject.TryGetComponent<NPC>(out var npc))
        {
            npc.StartDialogue();
        }

    }

    private void HandleSolidCollision(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        if (!collision.gameObject.CompareTag(SolidObjectTag))
        {
            return;
        }

        isCollidingSolid = true;

        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            if (Vector3.Dot(lastMoveDelta, normal) < 0f)
            {
                transform.position = preMovePosition;
            }
        }
        else
        {
            transform.position = preMovePosition;
        }
    }

    public void UseItem(Item item)
    {
        item.Action();
        
        //award XP for item use (could be moved to on pickup)
        if (progressionSystem != null)
        {
            progressionSystem.AwardItemXP();
        }
    }

    //interact with another actor
    public void Interact(Actor actor)
    {
        //output dialogue for any actor type
        actor.OutputDialogue();
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
    }

    public void UseCurrency(int amount)
    {
        Currency -= amount;
    }

    public void UseSkill(string skill)
    {
        if (skills.Contains(skill))
        {
            skills.Remove(skill);
            /*
                skill would be the one we want to implement
                not really sure how we would do this
                SkillSystem is not setup rn
             */
        }
    }
}

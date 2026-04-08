using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : Actor //this also gives us access to MonoBehavoiour
{
    public GameObject waterLevelShip;
    public LayerMask whatIsEnemy;

    public int Currency = 100;//default currency
    public int jumpHeight = 1;
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
	private UpdatedDialogueSystem dialogueSystem;

    public PlayerInput playerInput;
    public InputAction moveAction;

    public PlayerInput playerOrientation;
    public InputAction orientationAction;

    public PlayerInput jumpKey;
    public InputAction jumpKeyAction;

    public PlayerInput playerMouse;
    public InputAction mouseAction;

    public PlayerInput playerNumberKeys;
    public InputAction numberKeyAction;

    public PlayerInput playerInteract;
    public InputAction InteractAction;



    private bool isInMotion = false;
    private bool onShip = false;
    private bool stairCollision = false;
    private Rigidbody rigidBody;
    private Collider playerCollider;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float groundNormalMinY = 0.5f;
    private bool inRangeOfShip = false;
    private float prevShipYRot = 0;
    
    // public InputAction

    //Physical Objects associated with player
    public GameObject sword;
    public GameObject leftArm;
    public GameObject rightArm;

    //player animation variables
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
    private Renderer swordRenderer;

    public Material[] swordMaterials = new Material[4];
    private int currWeaponIndex = 0;

    //object collision handling variables
    private const string SolidObjectTag = "SolidObject";
    private bool isCollidingSolid;
    private Vector3 preMovePosition;
    private Vector3 lastMoveDelta;


    private UpdatedNPC currentNPC;
    private Item currItem;
    private Rope currRope;

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
        CacheSwordVisuals();

        if (thisObject != null)
        {
            rigidBody = thisObject.GetComponent<Rigidbody>();
            playerCollider = thisObject.GetComponent<Collider>();
            Debug.Log("Rigid body initialized!");
        }
        else{Debug.Log("add PlayerComponents to 'This Object' field");}

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider>();
        }

        //xp system
        if (progressionSystem == null)
        {
            progressionSystem = GetComponent<ProgressionSystem>();
        }

		//find DialogueSystem
		dialogueSystem = FindFirstObjectByType<UpdatedDialogueSystem>();

        // base(health, damage, xRotation, yRotation);
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");

        jumpKey = GetComponent<PlayerInput>();
        jumpKeyAction = playerInput.actions.FindAction("Jump");

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

        playerNumberKeys = GetComponent<PlayerInput>();
        // playerNumberKeys.SwitchCurrentActionMap("NumberKeys");
        numberKeyAction = playerNumberKeys.actions.FindAction("NumberKeys");
        
        
        playerInteract = GetComponent<PlayerInput>();
        InteractAction = playerInteract.actions.FindAction("Interact");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (swordRenderer != null){swordRenderer.material = swordMaterials[currWeaponIndex];}

        preMovePosition = transform.position;
        Damage = 50;

       // MoveToScene(0); 
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
        
        if (collision.gameObject.TryGetComponent<LavaDamage>(out var lava))
        {
            lava.lavaTimer += Time.deltaTime;

            if (lava.lavaTimer >= lava.lavaTickRate)
            {
                lava.lavaTimer = 0f;

                Health -= lava.damagePerSecond;

                Debug.Log("Player took lava damage!");
                Debug.Log(Health);
            }
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
            if (attackAnimationCooldown > 120)
            {
                attackAnimationCooldown = 0;
                Attack();
            }
        }

        if (jumpKeyAction.triggered)
        {
            Jump();
        }

        if (numberKeyAction.triggered)
        {
            // Debug.Log("Key pressed!");
            /*
             This is how the player will teleport between scenes
             Right now unity is set for keys 1-4 -> we can use indexes 0-3
             Can add more later if we need
             To add your scene to the list:
                File -> Build Profiles -> Scene List, then drag & drop your scene
                The index values are also shown there so you know which index to pass for MoveToScene() to switch to your scene
            we should have 6 total scenes in there by the time we're done
             */
            switch (GetPressedNumberKeyIndex())
            {
                case 0: changeWeapon(); break;
                case 1: MoveToScene(0); break;
            }
        }
        
        if (InteractAction.triggered)
        {
            if (InteractAction.activeControl is KeyControl keyControl)
            {
                switch (keyControl.keyCode)
                {
                    case Key.E: 
                        if(currentNPC != null){NPCInteract();} 
                        if(currRope != null){currRope.Interact();}
<<<<<<< HEAD
                        if (Physics.Raycast(cameraPivot.position, cameraPivot.forward, out RaycastHit hit, 5f))
                        {
                            var pipe = hit.collider.GetComponentInParent<PipePiece>();

                            if (pipe != null)
                            {
                                pipe.Rotate();
                            }
                        }
=======
                        if(inRangeOfShip){this.BoardShip();}
>>>>>>> 34f46a2e1c05c2fe0b679300be060b9e9092fe10
                        break;
                    case Key.Q: ItemInteract(); break;

                }
            }
            
            // NPCInteract();
        }

// -> Debug to show all items in player inventory
        // string builder = "";
        // for(int i =0; i < inventory.getLen(); i++)
        // {
        //     builder += inventory.getItem(i).Name;
        //     if(i+1 < inventory.getLen()){builder += ", ";}
        // }
        // if(builder == ""){Debug.Log("No inventoryItems to display");}
        // else{Debug.Log(builder);}
    }

    private void changeWeapon()
    {
        if (sword == null){Debug.LogWarning("Cannot change sword colour because sword is not assigned.");return;}
        CacheSwordVisuals();

        // int colourIndex = GetPressedNumberKeyIndex();
        currWeaponIndex = currWeaponIndex > 2 ? 0 : currWeaponIndex + 1; 
        // if (colourIndex < 0 || colourIndex >= swordMaterials.Length)
        // {
        //     Debug.LogWarning("NumberKeys was triggered, but no supported number key was detected.");
        //     return;
        // }
        if (swordRenderer != null)
        {
            swordRenderer.material = swordMaterials[currWeaponIndex];
            return;
        }

        Debug.LogWarning("Sword has no Renderer or SpriteRenderer to recolour.");
    }
    private void CacheSwordVisuals()
    {
        if (sword == null)
        {
            swordRenderer = null;
            return;
        }

        // Debug.Log("Made it here");
        swordRenderer = sword.GetComponentInChildren<Renderer>();
        // Debug.Log("Name " + swordSpriteRenderer.name);
    }
    private int GetPressedNumberKeyIndex()
    {
        if (numberKeyAction == null || numberKeyAction.activeControl == null)
        {
            return -1;
        }

        if (numberKeyAction.activeControl is KeyControl keyControl)
        {
            switch (keyControl.keyCode)
            {
                case Key.Digit1:
                case Key.Numpad1:
                    return 0;
                case Key.Digit2:
                case Key.Numpad2:
                    return 1;
                case Key.Digit3:
                case Key.Numpad3:
                    return 2;
                case Key.Digit4:
                case Key.Numpad4:
                    return 3;
            }
        }

        string controlText = $"{numberKeyAction.activeControl.displayName}{numberKeyAction.activeControl.name}{numberKeyAction.activeControl.path}";
        for (int i = 0; i < swordMaterials.Length && i < 9; i++)
        {
            string keyNumber = (i + 1).ToString();
            if (controlText.Contains(keyNumber))
            {
                return i;
            }
        }

        return -1;
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
        if (onShip)
        {
            moveShip();
            return;
        }

        if (moveAction.IsPressed()){isInMotion = true;/*Debug.Log("Moving");*/}
        else{isInMotion = false;/*Debug.Log("Not Moving");*/}

        if (!isInMotion && stairCollision)
        {
            //if the player is colliding with the stair but not moving(not pressing a move key), gravity will move then downwards by default                
            //      -> do not let the player move in this case
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            // rigidBody.constraints = RigidbodyConstraints.FreezePositionX;
            //this dosen't stop the player moving tho lol
            // Rigidbody body = gameObject.GetComponent("Rigidbody");
            return;
        }
        rigidBody.constraints = RigidbodyConstraints.None;
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
    public void moveShip()
    {
        // Vector2 dir = moveAction.ReadValue<Vector2>();
        // Debug.Log("moveShip");
        if (moveAction.activeControl is KeyControl keyControl)
        {
            // Debug.Log("KeyControl");
            if (keyControl.keyCode.Equals(Key.W))
            {
                moveForward();
            }
            if (keyControl.keyCode.Equals(Key.A))
            {
                rotateLeft();
            }
            if (keyControl.keyCode.Equals(Key.D))
            {
                rotateRight();
            }
            // switch (keyControl.keyCode)
            // {
            //     case Key.W: moveForward(); 
            //     case Key.A: rotateLeft();  
            //     case Key.D: rotateRight();
            //     default: break;
            //         
            // }
            // if (moveAction.)
        }

        // Vector3 moveDirection = transform.forward*dir.y + transform.right*dir.x;
        //
        // preMovePosition = transform.position;
        // lastMoveDelta = moveDirection * Speed * Time.deltaTime;
        // waterLevelShip.transform.position += lastMoveDelta;
        //
        // Vector2 look = orientationAction.ReadValue<Vector2>() * lookSensitivity;
        // yaw += look.x;
        // pitch -= look.y;
        // pitch = Mathf.Clamp(pitch, -90f, 90f);
        //
        // waterLevelShip.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        // if (cameraPivot != null)
        // {
        //     cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        // }       
    }

    private void moveForward()
    {
        // Debug.Log("moveForward");
        Vector2 dir = moveAction.ReadValue<Vector2>();

        Vector3 moveDirection = transform.forward*dir.y + transform.right*dir.x;
        
        preMovePosition = transform.position;
        lastMoveDelta = moveDirection * Speed * Time.deltaTime;
        waterLevelShip.transform.position += lastMoveDelta;
    }

    private void rotateLeft()
    {
        prevShipYRot -= 0.5f;
        waterLevelShip.transform.rotation = Quaternion.Euler(0f, prevShipYRot, 0f);

        // yaw += 5;
        // pitch -= 5;
        // pitch = Mathf.Clamp(pitch, -90f, 90f);
        //
        // waterLevelShip.transform.rotation = Quaternion.Euler(0f, yaw, 0f);       
        // waterLevelShip.transform.localRotation = Quaternion.Euler(0f, pitch, 0f);
    }

    private void rotateRight()
    {
        prevShipYRot += 0.5f;
        waterLevelShip.transform.rotation = Quaternion.Euler(0f, prevShipYRot, 0f);
        // yaw -= 5;
        // pitch -= 5;
        // pitch = Mathf.Clamp(pitch, -90f, 90f);
        //
        // waterLevelShip.transform.rotation = Quaternion.Euler(0f, yaw, 0f);       
        // waterLevelShip.transform.localRotation = Quaternion.Euler(0f, pitch, 0f);
    }

    private void Jump()
    {
        if (rigidBody == null)
        {
            Debug.LogWarning("Player cannot jump because no Rigidbody was found.");
            return;
        }

        if (!IsGrounded())
        {
            return;
        }

        Vector3 velocity = rigidBody.linearVelocity;
        if (velocity.y < 0f)
        {
            velocity.y = 0f;
            rigidBody.linearVelocity = velocity;
        }

        float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        rigidBody.AddForce(Vector3.up * jumpVelocity * rigidBody.mass, ForceMode.Impulse);
    }
    private bool IsGrounded()
    {
        if (playerCollider == null)
        {
            return false;
        }

        Bounds bounds = playerCollider.bounds;
        Vector3 halfExtents = bounds.extents;
        halfExtents.y = Mathf.Max(halfExtents.y - 0.05f, 0.01f);

        if (!Physics.BoxCast(bounds.center, halfExtents, Vector3.down, out RaycastHit hit, transform.rotation, groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return hit.normal.y >= groundNormalMinY;
    }

    public void MoveToScene(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            return;
        }
        // SceneManager.LoadScene(sceneIndex);
    }


    public void SetCurrentNPC(UpdatedNPC npc)
    {
        currentNPC = npc;
    }
    public void ClearCurrentNPC(UpdatedNPC npc)
    {
        if (currentNPC == npc)
        {
            currentNPC = null;
        }
    }

    public void setCurrItem(Item item)
    {
        currItem = item;
    }
    public void clearCurrItem(Item item)
    {
        if (currItem == item)
        {
            currItem = null;
        }else{Debug.Log("Item we are trying to clear is not the curr item -> " + item.ID);}
    }

    public void setCurrRope(Rope rope)
    {
        currRope = rope;
    }
    public void clearCurrRope(Rope rope)
    {
        if (rope == currRope)
        {
            currRope = null;
        }else{Debug.Log("Item we are trying to clear is no the curr item -> " + rope.name);}
    }

    public void setIsInRangeOfShip(bool val)
    {
        inRangeOfShip = val;
    }

    private void NPCInteract()
    {
        if (dialogueSystem == null)
        {
            Debug.LogWarning("DialogueSystem not found.");
            return;
        }

        // If already talking → go to next line
        if (dialogueSystem.IsTalking())
        {
            dialogueSystem.NextLine();
            return;
        }

        // If near an NPC → start dialogue
        if (currentNPC != null)
        {
            currentNPC.StartDialogue();
        }
        else
        {
            Debug.Log("No NPC in range");
        }
    }

    private void ItemInteract()
    {
        if (currItem != null)
        {
            inventory.addItem(currItem); 
            GameObject.Destroy(currItem.itemContainer);
        }
    }

    private void BoardShip()
    {
        thisObject.transform.SetParent(waterLevelShip.transform);        
        thisObject.transform.localPosition = Vector3.zero;
        thisObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        this.thisObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        thisObject.transform.localPosition -= new Vector3(0, 1, 14);
        //restrict movement 
        // rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        // thisObject.transform.SetParent(null);
        // waterLevelShip.transform.SetParent(thisObject.transform);
        onShip = true;
        // BoardShip();
        // BoardShip.
    }
    public void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Collision detected");
        HandleSolidCollision(collision);
        // Debug.Log("Checking collisions...");
        if (collision.gameObject.CompareTag("Stairs"))
        {
            stairCollision = true;
            if (isInMotion)
            {
                // Debug.Log("Collision with stair!");
                transform.position += new Vector3(0, 0.1f, 0);
            }
            else{
                //if the player is colliding with the stair but not moving(not pressing a move key), gravity will move then downwards by default                
                //      -> do not let the player move in this case
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        HandleSolidCollision(collision);
        HazardCollide(collision);
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

        if (collision.gameObject.CompareTag("Stairs"))
        {
            stairCollision = false;
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

        //i don't know if this does anything, could probably remove it
        // if (collision.contactCount > 0)
        // {
        //     Vector3 normal = collision.GetContact(0).normal;
        //     if (Vector3.Dot(lastMoveDelta, normal) < 0f)
        //     {
        //         transform.position = preMovePosition;
        //     }
        // }
        // else
        // {
        //     transform.position = preMovePosition;
        // }


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
    //this method isn't used with the current NPC system
    //public void Interact(Actor actor)
    //{
        //output dialogue for any actor type
        //actor.OutputDialogue();
   // }

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

using UnityEngine;
using UnityEngine.InputSystem;

public class SkygliderBasic : MonoBehaviour
{
    [Header("Setup")]
    public Transform seatPoint;
    public LayerMask groundMask = ~0;

    [Header("Flight")]
    public float cruiseSpeed = 8f;
    public float turnSpeed = 75f;
    public float pitchSpeed = 35f;
    public float glideFallSpeed = 0.5f;
    public float climbStrength = 2f;
    public float maxVerticalSpeed = 3f;

    [Header("Landing")]
    public float landingCheckDistance = 1f;
    public float landedHeightOffset = 0.5f;
    public float takeoffThreshold = 0.2f;

    [Header("Input")]
    public float mountCooldown = 0.3f;
    public float dismountCooldown = 0.3f;

    private Player nearbyPlayer;
    private Player mountedPlayer;
    private PlayerInput mountedPlayerInput;
    private InputAction mountedMoveAction;
    private InputAction mountedLookAction;
    private InputAction mountedInteractAction;
    private Rigidbody mountedPlayerRigidbody;
    private Collider mountedPlayerCollider;
    private Player mountedPlayerScript;
    private float currentPitch;
    private float mountTime;
    private float lastDismountTime = -999f;
    private bool isLanded;

    private void Awake()
    {
        if (seatPoint == null)
        {
            seatPoint = transform;
        }
    }

    private void Update()
    {
        if (mountedPlayer == null)
        {
            TryMountNearbyPlayer();
            return;
        }

        if (mountedInteractAction != null &&
            mountedInteractAction.triggered &&
            Time.time >= mountTime + dismountCooldown)
        {
            DismountPlayer();
            return;
        }

        if (isLanded)
        {
            Vector2 moveInput = mountedMoveAction != null ? mountedMoveAction.ReadValue<Vector2>() : Vector2.zero;
            if (moveInput.y > takeoffThreshold)
            {
                isLanded = false;
            }

            return;
        }

        HandleFlight(Time.deltaTime);
    }

    private void TryMountNearbyPlayer()
    {
        if (nearbyPlayer == null)
        {
            return;
        }

        if (Time.time < lastDismountTime + mountCooldown)
        {
            Debug.Log("SkygliderBasic -> Mount blocked by cooldown.");
            return;
        }

        PlayerInput playerInput = nearbyPlayer.GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.Log("SkygliderBasic -> Nearby player has no PlayerInput.");
            return;
        }

        InputAction interactAction = playerInput.actions.FindAction("Interact");
        if (interactAction == null)
        {
            Debug.Log("SkygliderBasic -> Could not find Interact action on player.");
            return;
        }

        if (!interactAction.triggered)
        {
            return;
        }

        Debug.Log("SkygliderBasic -> Interact detected. Mounting player " + nearbyPlayer.name);

        MountPlayer(nearbyPlayer, playerInput);
    }

    private void MountPlayer(Player player, PlayerInput playerInput)
    {
        mountedPlayer = player;
        mountedPlayerInput = playerInput;
        mountedMoveAction = playerInput.actions.FindAction("Move");
        mountedLookAction = playerInput.actions.FindAction("Orientation");
        mountedInteractAction = playerInput.actions.FindAction("Interact");
        mountedPlayerScript = player.GetComponent<Player>();
        mountedPlayerRigidbody = GetPlayerRigidbody(player);
        mountedPlayerCollider = GetPlayerCollider(player);
        currentPitch = 0f;
        isLanded = false;
        mountTime = Time.time;

        if (mountedPlayerScript != null)
        {
            mountedPlayerScript.enabled = false;
            if (mountedPlayerScript.sword != null)
            {
                mountedPlayerScript.sword.SetActive(false);
            }
        }

        if (mountedPlayerRigidbody != null)
        {
            mountedPlayerRigidbody.linearVelocity = Vector3.zero;
            mountedPlayerRigidbody.angularVelocity = Vector3.zero;
            mountedPlayerRigidbody.useGravity = false;
            mountedPlayerRigidbody.isKinematic = true;
        }

        if (mountedPlayerCollider != null)
        {
            mountedPlayerCollider.enabled = false;
        }

        mountedPlayer.transform.SetParent(seatPoint);
        mountedPlayer.transform.localPosition = Vector3.zero;
        mountedPlayer.transform.localRotation = Quaternion.identity;

        Debug.Log("SkygliderBasic -> Player mounted successfully.");
    }

    private void DismountPlayer()
    {
        if (mountedPlayer == null)
            return;

        mountedPlayer.transform.SetParent(null);

        Vector3 dismountPosition = transform.position + transform.right * 1.5f + Vector3.up * 0.5f;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 5f, groundMask, QueryTriggerInteraction.Ignore))
        {
            dismountPosition = groundHit.point + Vector3.up * 1f;
        }

        mountedPlayer.transform.position = dismountPosition;

        if (mountedPlayerCollider != null)
        {
            mountedPlayerCollider.enabled = true;
        }

        if (mountedPlayerRigidbody != null)
        {
            mountedPlayerRigidbody.isKinematic = false;
            mountedPlayerRigidbody.useGravity = true;
            mountedPlayerRigidbody.linearVelocity = Vector3.zero;
        }

        if (mountedPlayerScript != null)
        {
            mountedPlayerScript.enabled = true;
            if (mountedPlayerScript.sword != null)
            {
                mountedPlayerScript.sword.SetActive(true);
            }
        }

        mountedPlayer = null;
        mountedPlayerInput = null;
        mountedMoveAction = null;
        mountedLookAction = null;
        mountedInteractAction = null;
        mountedPlayerScript = null;
        mountedPlayerRigidbody = null;
        mountedPlayerCollider = null;
        currentPitch = 0f;
        isLanded = false;
        lastDismountTime = Time.time;

        Debug.Log("SkygliderBasic -> Player dismounted.");
    }

    private void HandleFlight(float deltaTime)
    {
        Vector2 moveInput = mountedMoveAction != null ? mountedMoveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = mountedLookAction != null ? mountedLookAction.ReadValue<Vector2>() : Vector2.zero;

        float yawInput = moveInput.x;
        float pitchInput = Mathf.Abs(lookInput.y) > 0.01f ? -lookInput.y : moveInput.y;

        transform.Rotate(0f, yawInput * turnSpeed * deltaTime, 0f, Space.Self);

        currentPitch += pitchInput * pitchSpeed * deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -20f, 20f);

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentPitch, euler.y, 0f);

        float verticalSpeed = Mathf.Clamp(
            -glideFallSpeed + ((currentPitch / 20f) * climbStrength),
            -maxVerticalSpeed,
            maxVerticalSpeed);

        Vector3 movement = (transform.forward * cruiseSpeed * deltaTime) + (Vector3.up * verticalSpeed * deltaTime);
        transform.position += movement;

        if (verticalSpeed <= 0f &&
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, landingCheckDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = groundHit.point + (groundHit.normal * landedHeightOffset);
            Vector3 landedEuler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, landedEuler.y, 0f);
            currentPitch = 0f;
            isLanded = true;
            Debug.Log("SkygliderBasic -> Glider landed on " + groundHit.collider.name);
        }
    }

    public void SetNearbyPlayer(Player player)
    {
        nearbyPlayer = player;
        Debug.Log("SkygliderBasic -> Nearby player set: " + player.name);
    }

    public void ClearNearbyPlayer(Player player)
    {
        if (nearbyPlayer == player)
        {
            nearbyPlayer = null;
            Debug.Log("SkygliderBasic -> Nearby player cleared.");
        }
    }

    private Rigidbody GetPlayerRigidbody(Player player)
    {
        if (player.thisObject != null)
        {
            return player.thisObject.GetComponent<Rigidbody>();
        }

        return player.GetComponent<Rigidbody>();
    }

    private Collider GetPlayerCollider(Player player)
    {
        if (player.thisObject != null)
        {
            return player.thisObject.GetComponent<Collider>();
        }

        return player.GetComponent<Collider>();
    }
}

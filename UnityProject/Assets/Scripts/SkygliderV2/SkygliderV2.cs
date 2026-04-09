using UnityEngine;
using UnityEngine.InputSystem;

public class SkygliderV2 : MonoBehaviour
{
    [Header("Setup")]
    public Transform seatPoint;
    public LayerMask solidMask = ~0;

    [Header("Flight")]
    public float cruiseSpeed = 8f;
    public float turnSpeed = 70f;
    public float pitchSpeed = 35f;
    public float glideFallSpeed = 0.35f;
    public float climbStrength = 2.5f;
    public float maxVerticalSpeed = 4f;

    [Header("Collision")]
    public float landingCheckDistance = 1.2f;
    public float obstacleCheckPadding = 0.4f;
    public float landedHeightOffset = 0.6f;

    [Header("Input")]
    public float remountDelay = 0.3f;
    public float dismountDelay = 0.3f;

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
            Time.time >= mountTime + dismountDelay)
        {
            DismountPlayer("manual interact");
            return;
        }

        if (!isLanded)
        {
            HandleFlight(Time.deltaTime);
        }
    }

    private void TryMountNearbyPlayer()
    {
        if (nearbyPlayer == null)
            return;

        if (Time.time < lastDismountTime + remountDelay)
            return;

        PlayerInput playerInput = nearbyPlayer.GetComponent<PlayerInput>();
        if (playerInput == null)
            return;

        InputAction interactAction = playerInput.actions.FindAction("Interact");
        if (interactAction == null || !interactAction.triggered)
            return;

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

        if (mountedPlayerScript != null && mountedPlayerScript.sword != null)
        {
            mountedPlayerScript.sword.SetActive(false);
        }

        Debug.Log("SkygliderV2 -> Player mounted.");
    }

    private void DismountPlayer(string reason)
    {
        if (mountedPlayer == null)
            return;

        mountedPlayer.transform.SetParent(null);

        Vector3 dismountPosition = transform.position + transform.right * 1.5f + Vector3.up * 0.5f;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 4f, solidMask, QueryTriggerInteraction.Ignore))
        {
            dismountPosition = groundHit.point + Vector3.up * 1.1f;
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

        Debug.Log("SkygliderV2 -> Player dismounted. Reason: " + reason);

        mountedPlayer = null;
        mountedPlayerInput = null;
        mountedMoveAction = null;
        mountedLookAction = null;
        mountedInteractAction = null;
        mountedPlayerScript = null;
        mountedPlayerRigidbody = null;
        mountedPlayerCollider = null;
        isLanded = false;
        lastDismountTime = Time.time;
        currentPitch = 0f;
    }

    private void HandleFlight(float deltaTime)
    {
        Vector2 moveInput = mountedMoveAction != null ? mountedMoveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = mountedLookAction != null ? mountedLookAction.ReadValue<Vector2>() : Vector2.zero;

        float yawInput = moveInput.x;
        float pitchInput = Mathf.Abs(lookInput.y) > 0.01f ? -lookInput.y : moveInput.y;

        transform.Rotate(0f, yawInput * turnSpeed * deltaTime, 0f, Space.Self);

        currentPitch += pitchInput * pitchSpeed * deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -15f, 15f);

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentPitch, euler.y, 0f);

        float verticalSpeed = Mathf.Clamp(-glideFallSpeed + ((currentPitch / 15f) * climbStrength), -maxVerticalSpeed, maxVerticalSpeed);
        Vector3 movement = (transform.forward * cruiseSpeed * deltaTime) + (Vector3.up * verticalSpeed * deltaTime);

        if (movement.sqrMagnitude > 0.0001f)
        {
            float castDistance = movement.magnitude + obstacleCheckPadding;
            if (Physics.Raycast(transform.position, movement.normalized, out RaycastHit hit, castDistance, solidMask, QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point - (movement.normalized * 0.3f);

                if (hit.normal.y > 0.4f)
                {
                    LandOnSurface(hit);
                }

                return;
            }
        }

        transform.position += movement;

        if (verticalSpeed <= 0f &&
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, landingCheckDistance, solidMask, QueryTriggerInteraction.Ignore))
        {
            LandOnSurface(groundHit);
        }
    }

    private void LandOnSurface(RaycastHit hit)
    {
        transform.position = hit.point + (hit.normal * landedHeightOffset);
        Vector3 landedEuler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, landedEuler.y, 0f);
        isLanded = true;
        currentPitch = 0f;
        Debug.Log("SkygliderV2 -> Landed on " + hit.collider.name);
    }

    public void SetNearbyPlayer(Player player)
    {
        nearbyPlayer = player;
        Debug.Log("SkygliderV2 -> Nearby player set: " + player.name);
    }

    public void ClearNearbyPlayer(Player player)
    {
        if (nearbyPlayer == player)
        {
            nearbyPlayer = null;
            Debug.Log("SkygliderV2 -> Nearby player cleared.");
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
